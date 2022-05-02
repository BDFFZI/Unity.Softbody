using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Assertions;

namespace ParticleSpring
{
    public abstract class Collider<TColliderType> : Solver
        where TColliderType : Collider
    {
        /// <summary>
        /// 注意这不是力这是速度
        /// </summary>
        public Vector3 Resistance { get; private set; }
        public Vector3 ResistancePosition { get; private set; }
        public float FrictionCoefficient => colliderInfo.material.dynamicFriction;
        public float ElasticityCoefficient => colliderInfo.material.bounciness;


        public override void Solve(Softbody softbody, float deltaTime)
        {
            Matrix4x4 currentTransform = transform.worldToLocalMatrix;
            NativeArray<Vector3> resistances = new NativeArray<Vector3>(softbody.Particles.Length, Allocator.TempJob);
            NativeArray<Vector3> collisionVectors = new NativeArray<Vector3>(softbody.Particles.Length, Allocator.TempJob);
            GetCollisionVector(softbody.Particles, collisionVectors);

            Computer computer = new Computer(lastTransform, currentTransform,
                softbody.Particles, collisionVectors,
                resistances,
                FrictionCoefficient, ElasticityCoefficient, deltaTime);
            computer.Schedule(softbody.Particles.Length, 3).Complete();

            Vector3 totalResistances = Vector3.zero;
            for (int i = 0; i < resistances.Length; i++)
            {
                totalResistances += resistances[i];
            }
            if (totalResistances.magnitude > Resistance.magnitude)
                Resistance = totalResistances;

            float totalResistancesMagnitude = totalResistances.magnitude;
            if (totalResistancesMagnitude != 0)
            {
                ResistancePosition = Vector3.zero;
                for (int i = 0; i < resistances.Length; i++)
                {
                    float weight = resistances[i].magnitude / totalResistancesMagnitude;
                    ResistancePosition += softbody.Particles[i].Position * weight;
                }
            }

            Debug.DrawRay(transform.position, Resistance * deltaTime, Color.red);
            Debug.DrawLine(transform.position, ResistancePosition, Color.cyan);

            collisionVectors.Dispose();
            resistances.Dispose();

            lastTransform = currentTransform;
        }

        protected TColliderType ColliderInfo => colliderInfo;

        protected abstract void GetCollisionVector(NativeArray<Particle> particles, NativeArray<Vector3> collisionVectors);

        [SerializeField] TColliderType colliderInfo;

        private struct Computer : IJobParallelFor
        {
            public Computer(Matrix4x4 lastTransform, Matrix4x4 currentTransform,
                NativeArray<Particle> particles, NativeArray<Vector3> collisionVectors,
                NativeArray<Vector3> resistances,
    float frictionCoefficient, float elasticityCoefficient, float deltaTime)
            {
                this.lastTransform = lastTransform;
                this.currentTransform = currentTransform;
                this.particles = particles;
                this.collisionVectors = collisionVectors;
                this.resistances = resistances;
                this.frictionCoefficient = frictionCoefficient;
                this.elasticityCoefficient = elasticityCoefficient;
                this.deltaTime = deltaTime;
            }

            readonly Matrix4x4 lastTransform;
            readonly Matrix4x4 currentTransform;
            NativeArray<Particle> particles;
            readonly NativeArray<Vector3> collisionVectors;
            NativeArray<Vector3> resistances;
            readonly float frictionCoefficient;
            readonly float elasticityCoefficient;
            readonly float deltaTime;

            public void Execute(int index)
            {
                Particle particle = particles[index];
                if (particle.IsKinematic)
                    return;

                Vector3 collisionVector = collisionVectors[index];
                if (collisionVector == Vector3.zero)
                    return;

                Debug.Assert(particle.Force == Vector3.zero);

                //Vector3 normal = collisionVector.normalized;
                //Vector3 motionDirection = Vector3.Cross(Vector3.Cross(normal, particle.Velocity.normalized), normal);
                //Vector3 motionVelocity = Vector3.Project(particle.Velocity, motionDirection);

                //particle.Position += collisionVector - particle.VelocityToPositionOffset(motionVelocity, deltaTime);
                //particle.Velocity = Vector3.zero;

                Debug.DrawRay(particle.Position, collisionVector, Color.green);

                Vector3 normal = collisionVector.normalized;
                Vector3 velocity = particle.Velocity;
                Vector3 motionDirection = Vector3.Cross(Vector3.Cross(normal, velocity.normalized), normal).normalized;
                Vector3 motionVelocity = Vector3.Project(particle.Velocity, motionDirection);
                Vector3 normalVelocity = Vector3.Project(particle.Velocity, -normal);
                //摩擦力
                float frictionmMagnitude = Mathf.Min(motionVelocity.magnitude, normalVelocity.magnitude * frictionCoefficient);
                Vector3 friction = -motionDirection * frictionmMagnitude;
                particle.Velocity += friction;
                particle.Position += particle.VelocityToPositionOffset(friction, deltaTime);
                resistances[index] = -friction;

                //弹力
                Vector3 elasticity = (1 + elasticityCoefficient) * -normalVelocity;
                particle.Velocity += elasticity;
                particle.Position += collisionVector;
                resistances[index] = -elasticity;

                //当碰撞体移动时带动质点
                float frictionRate = /*frictionmMagnitude / motionForceMagnitude*/1;
                Vector3 lastParticleLocalPosition = lastTransform.MultiplyPoint(particle.Position);
                Vector3 currentParticleLocalPosition = currentTransform.MultiplyPoint(particle.Position);
                Vector3 positionOffset = (lastParticleLocalPosition - currentParticleLocalPosition) * frictionRate;
                particle.Position += currentTransform.inverse.MultiplyVector(positionOffset);

                particles[index] = particle;
            }
        }

        private void Awake()
        {
            lastTransform = transform.worldToLocalMatrix;
        }

        private void LateUpdate()
        {
            Resistance = Vector3.zero;
        }

        Matrix4x4 lastTransform;
    }
}

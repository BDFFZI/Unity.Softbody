using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace ParticleSpring
{
    public abstract class Collider<TColliderType> : Solver
        where TColliderType : Collider
    {
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
                Vector3 collisionVector = collisionVectors[index];
                if (collisionVector.magnitude == 0)
                    return;

                Particle particle = particles[index];

                Vector3 normal = collisionVector.normalized;
                Vector3 force = particle.VelocityToForce(particle.Velocity, deltaTime);
                Vector3 normalForce = Vector3.Project(force, normal);
                //摩擦力
                Vector3 frictionDirection = Vector3.Cross(normal, Vector3.Cross(normal, force.normalized));
                float frictionDirectionforceMagnitude = Vector3.Project(force, -frictionDirection).magnitude;
                float frictionmMagnitude = Mathf.Min(frictionDirectionforceMagnitude, normalForce.magnitude * frictionCoefficient);
                Vector3 friction = frictionDirection * frictionmMagnitude;

                //弹力
                Vector3 elasticity = -normalForce * (1 + elasticityCoefficient);

                //当碰撞体移动时带动质点
                if (frictionDirectionforceMagnitude != 0)
                {
                    float frictionRate = frictionmMagnitude / frictionDirectionforceMagnitude;
                    Vector3 lastParticleLocalPosition = lastTransform.MultiplyPoint(particle.Position);
                    Vector3 currentParticleLocalPosition = currentTransform.MultiplyPoint(particle.Position);
                    Vector3 positionOffset = (lastParticleLocalPosition - currentParticleLocalPosition) * frictionRate;
                    particle.Position += currentTransform.inverse.MultiplyVector(positionOffset);
                }

                Vector3 totalForce = friction + elasticity;
                particle.Velocity += particle.ForceToVelocity(totalForce, deltaTime);
                particle.Position += collisionVector;

                resistances[index] = -totalForce;
                particles[index] = particle;
            }
        }

        private void Awake()
        {
            lastTransform = transform.worldToLocalMatrix;
        }

        Matrix4x4 lastTransform;
    }
}

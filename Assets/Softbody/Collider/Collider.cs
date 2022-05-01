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
        public float FrictionCoefficient => colliderInfo.material.dynamicFriction;
        public float ElasticityCoefficient => colliderInfo.material.bounciness;


        public override void Solve(Softbody softbody, float deltaTime)
        {
            NativeArray<Vector3> resistances = new NativeArray<Vector3>(softbody.Particles.Length, Allocator.TempJob);
            NativeArray<Vector3> collisionVectors = new NativeArray<Vector3>(softbody.Particles.Length, Allocator.TempJob);
            GetCollisionVector(softbody.Particles, collisionVectors);

            Computer computer = new Computer(softbody.Particles,
                collisionVectors, resistances,
                FrictionCoefficient, ElasticityCoefficient, deltaTime);
            computer.Schedule(softbody.Particles.Length, 3).Complete();

            collisionVectors.Dispose();
            resistances.Dispose();
        }

        protected TColliderType ColliderInfo => colliderInfo;

        protected abstract void GetCollisionVector(NativeArray<Particle> particles, NativeArray<Vector3> collisionVectors);

        [SerializeField] TColliderType colliderInfo;

        private struct Computer : IJobParallelFor
        {
            public Computer(NativeArray<Particle> particles,
                NativeArray<Vector3> collisionVectors, NativeArray<Vector3> resistances,
    float frictionCoefficient, float elasticityCoefficient, float deltaTime)
            {
                this.particles = particles;
                this.collisionVectors = collisionVectors;
                this.resistances = resistances;
                this.frictionCoefficient = frictionCoefficient;
                this.elasticityCoefficient = elasticityCoefficient;
                this.deltaTime = deltaTime;
            }

            NativeArray<Particle> particles;
            readonly NativeArray<Vector3> collisionVectors;
            NativeArray<Vector3> resistances;
            readonly float frictionCoefficient;
            readonly float elasticityCoefficient;
            readonly float deltaTime;

            public void Execute(int index)
            {
                Particle particle = particles[index];
                Vector3 collisionVector = collisionVectors[index];

                Vector3 normal = collisionVector.normalized;
                Vector3 force = particle.VelocityToForce(particle.Velocity, deltaTime);
                Vector3 normalForce = Vector3.Project(force, normal);
                //Ä¦²ÁÁ¦
                Vector3 frictionDirection = Vector3.Cross(normal, Vector3.Cross(normal, force.normalized));
                float frictionDirectionforceMagnitude = Vector3.Project(force, -frictionDirection).magnitude;
                float frictionmMagnitude = Mathf.Min(frictionDirectionforceMagnitude, normalForce.magnitude * frictionCoefficient);
                Vector3 friction = frictionDirection * frictionmMagnitude;
                //µ¯Á¦
                Vector3 elasticity = normalForce * (1 + elasticityCoefficient);

                resistances[index] = frictionDirection + elasticity;
                particle.Velocity += particle.ForceToVelocity(resistances[index], deltaTime);

                particles[index] = particle;
            }
        }
    }
}

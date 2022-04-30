using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace ParticleSpring
{
    public abstract class Collider : Solver
    {
        public override void Solve(Softbody softbody, float deltaTime)
        {
            NativeArray<Vector3> collisionVectors = new NativeArray<Vector3>(softbody.Particles.Length, Allocator.TempJob);
            GetCollisionVector(softbody.Particles, collisionVectors);


            collisionVectors.Dispose();
        }

        protected abstract void GetCollisionVector(NativeArray<Particle> particles, NativeArray<Vector3> collisionVectors);

        private struct Computer : IJobParallelFor
        {
            public Computer(NativeArray<Vector3> particles, NativeArray<Vector3> collisionVectors)
            {
                this.particles = particles;
                this.collisionVectors = collisionVectors;
            }

            NativeArray<Vector3> particles;
            NativeArray<Vector3> collisionVectors;

            public void Execute(int index)
            {
                //TODO
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace ParticleSpring
{
    [RequireComponent(typeof(UnityEngine.BoxCollider))]
    public class BoxCollider : Collider
    {
        [SerializeField] UnityEngine.BoxCollider boxCollider;

        protected override void GetCollisionVector(NativeArray<Particle> particles, NativeArray<Vector3> collisionVectors)
        {
            throw new System.NotImplementedException();
        }

        private struct Computer : IJobParallelFor
        {
            public Computer(Vector3 position, Vector3 forward, Vector3 up, Vector3 right,
    NativeArray<Particle> particles, NativeArray<Vector3> collisionVectors)
            {
                Position = position;
                Forward = forward;
                Up = up;
                Right = right;
                this.particles = particles;
                this.collisionVectors = collisionVectors;
            }

            readonly Vector3 Position;
            readonly Vector3 Forward;
            readonly Vector3 Up;
            readonly Vector3 Right;
            readonly NativeArray<Particle> particles;
            NativeArray<Vector3> collisionVectors;



            public void Execute(int index)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}

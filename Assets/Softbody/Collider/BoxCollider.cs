using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace ParticleSpring
{
    [RequireComponent(typeof(UnityEngine.BoxCollider))]
    public class BoxCollider : Collider<UnityEngine.BoxCollider>
    {
        protected override void GetCollisionVector(NativeArray<Particle> particles, NativeArray<Vector3> collisionVectors)
        {
            Vector3 position = transform.TransformPoint(ColliderInfo.center);
            Vector3 right = transform.right * ColliderInfo.size.x;
            Vector3 up = transform.up * ColliderInfo.size.y;
            Vector3 forward = transform.forward * ColliderInfo.size.z;

            Computer computer = new Computer(position, right, up, forward, particles, collisionVectors);
            computer.Schedule(collisionVectors.Length, 3).Complete();
        }

        private struct Computer : IJobParallelFor
        {
            public Computer(Vector3 position, Vector3 right, Vector3 up, Vector3 forward,
    NativeArray<Particle> particles, NativeArray<Vector3> collisionVectors)
            {
                this.position = position;
                this.right = right;
                this.up = up;
                this.forward = forward;
                this.particles = particles;
                this.collisionVectors = collisionVectors;
                List<Particle> list = new List<Particle>();
            }

            readonly Vector3 position;
            readonly Vector3 right;
            readonly Vector3 up;
            readonly Vector3 forward;
            readonly NativeArray<Particle> particles;
            NativeArray<Vector3> collisionVectors;

            public void Execute(int index)
            {
                Particle particle = particles[index];

                Vector3 relativePosition = particle.Position - position;

                NativeArray<float> boxlength = new NativeArray<float>(6, Allocator.Temp);
                boxlength[0] = right.sqrMagnitude;
                boxlength[1] = up.sqrMagnitude;
                boxlength[2] = forward.sqrMagnitude;

                NativeArray<float> distanceFromOrigin = new NativeArray<float>(6, Allocator.Temp);
                distanceFromOrigin[0] = Vector3.Project(relativePosition, right).sqrMagnitude;
                distanceFromOrigin[1] = Vector3.Project(relativePosition, up).sqrMagnitude;
                distanceFromOrigin[2] = Vector3.Project(relativePosition, forward).sqrMagnitude;
                distanceFromOrigin[3] = -distanceFromOrigin[0];
                distanceFromOrigin[4] = -distanceFromOrigin[1];
                distanceFromOrigin[5] = -distanceFromOrigin[2];


                NativeArray<float> distanceFromWall = new NativeArray<float>(6, Allocator.Temp);
                distanceFromWall[0] = boxlength[0] - distanceFromWall[0];
                distanceFromWall[1] = boxlength[1] - distanceFromWall[1];
                distanceFromWall[2] = boxlength[2] - distanceFromWall[2];
                distanceFromWall[3] = boxlength[0] - distanceFromWall[3];
                distanceFromWall[4] = boxlength[1] - distanceFromWall[4];
                distanceFromWall[5] = boxlength[2] - distanceFromWall[5];

                int minDistanceIndex = distanceFromWall.Min(out float minDistance);
                if (minDistance < 0)
                {
                    collisionVectors[index] = Vector3.zero;
                    return;
                }


                Vector3 collisionVector = Vector3.zero;
                switch (minDistanceIndex)
                {
                    case 0:
                        collisionVector = right.normalized * minDistance;
                        break;
                    case 1:
                        collisionVector = up.normalized * minDistance;
                        break;
                    case 2:
                        collisionVector = forward.normalized * minDistance;
                        break;
                    case 3:
                        collisionVector = -right.normalized * minDistance;
                        break;
                    case 4:
                        collisionVector = -up.normalized * minDistance;
                        break;
                    case 5:
                        collisionVector = -forward.normalized * minDistance;
                        break;
                    default:
                        throw new System.Exception();
                }

                collisionVectors[index] = collisionVector;
            }
        }
    }
}

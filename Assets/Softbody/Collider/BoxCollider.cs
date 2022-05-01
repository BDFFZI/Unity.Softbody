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
            Vector3 right = 0.5f * ColliderInfo.size.x * transform.right;
            Vector3 up = 0.5f * ColliderInfo.size.y * transform.up;
            Vector3 forward = 0.5f * ColliderInfo.size.z * transform.forward;

            Computer computer = new Computer(position, right, up, forward,
                particles, collisionVectors);
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
                boxlength[0] = right.magnitude;
                boxlength[1] = up.magnitude;
                boxlength[2] = forward.magnitude;

                NativeArray<float> distanceFromOrigin = new NativeArray<float>(6, Allocator.Temp);
                distanceFromOrigin[0] = Vector3.Project(relativePosition, right).magnitude * Mathf.Sign(Vector3.Dot(relativePosition, right));
                distanceFromOrigin[1] = Vector3.Project(relativePosition, up).magnitude * Mathf.Sign(Vector3.Dot(relativePosition, up));
                distanceFromOrigin[2] = Vector3.Project(relativePosition, forward).magnitude * Mathf.Sign(Vector3.Dot(relativePosition, forward));
                distanceFromOrigin[3] = -distanceFromOrigin[0];
                distanceFromOrigin[4] = -distanceFromOrigin[1];
                distanceFromOrigin[5] = -distanceFromOrigin[2];


                NativeArray<float> distanceFromWall = new NativeArray<float>(6, Allocator.Temp);
                distanceFromWall[0] = boxlength[0] - distanceFromOrigin[0];
                distanceFromWall[1] = boxlength[1] - distanceFromOrigin[1];
                distanceFromWall[2] = boxlength[2] - distanceFromOrigin[2];
                distanceFromWall[3] = boxlength[0] - distanceFromOrigin[3];
                distanceFromWall[4] = boxlength[1] - distanceFromOrigin[4];
                distanceFromWall[5] = boxlength[2] - distanceFromOrigin[5];

                int minDistanceIndex = distanceFromWall.Min(out float minDistance);
                if (minDistance < 0)
                {
                    collisionVectors[index] = Vector3.zero;
                    return;
                }

                Vector3 collisionVector;
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
                //Debug.DrawRay(particle.Position, collisionVectors[index], Color.blue);
            }
        }
    }
}

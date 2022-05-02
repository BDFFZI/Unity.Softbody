using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

namespace ParticleSpring
{
    public class Gravity : Solver
    {
        public override void Solve(Softbody softbody, float deltaTime)
        {
            Solver solver = new Solver(softbody.Particles, gravity, deltaTime);
            solver.Schedule(softbody.Particles.Length, 3).Complete();
        }

        struct Solver : IJobParallelFor
        {
            public Solver(NativeArray<Particle> massPoints, Vector3 worldGravity, float deltaTime)
            {
                this.massPoints = massPoints;
                this.worldGravity = worldGravity;
                this.deltaTime = deltaTime;
            }

            NativeArray<Particle> massPoints;
            readonly Vector3 worldGravity;
            readonly float deltaTime;

            public void Execute(int index)
            {
                Particle particle = massPoints[index];

                //ÖØÁ¦
                if (particle.UseGravity)
                {
                    Vector3 gravity = worldGravity * particle.Mass;
                    particle.Velocity += gravity * deltaTime;
                }

                massPoints[index] = particle;
            }


        }

        [SerializeField] Vector3 gravity = new Vector3(0, -9.81f, 0);
    }
}

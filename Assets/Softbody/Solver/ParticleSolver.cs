using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ParticleSolver : Solver
{
    public override void Solve(Softbody softbody, float deltaTime)
    {
        Computer computer = new Computer(softbody.Particles, deltaTime);
        computer.Schedule(softbody.Particles.Length, 3).Complete();
    }

    private struct Computer : IJobParallelFor
    {
        public Computer(NativeArray<Particle> particles, float deltaTime)
        {
            this.particles = particles;
            this.deltaTime = deltaTime;
        }

        NativeArray<Particle> particles;
        readonly float deltaTime;

        public void Execute(int index)
        {
            Particle particle = particles[index];
            if (particle.IsKinematic)
            {
                particle.Force = Vector3.zero;
                particle.Velocity = Vector3.zero;
                return;
            }

            Vector3 acceleration = Vector3.zero;

            //重力
            if (particle.UseGravity)
            {
                Vector3 gravity = Vector3.down * particle.Mass * 9.8f;
                acceleration += gravity;
            }

            //空气阻力
            Vector3 airDrag = particle.Drag * -particle.Velocity.normalized * particle.Velocity.sqrMagnitude;
            particle.Force += airDrag;

            acceleration += particle.Force / particle.Mass;
            particle.Velocity += acceleration * deltaTime;
            particle.Position += particle.Velocity * deltaTime;

            particle.Force = Vector3.zero;

            particles[index] = particle;
        }
    }
}

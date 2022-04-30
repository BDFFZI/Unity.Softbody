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

            particle = particle.GetNextState(deltaTime);

            particles[index] = particle;
        }
    }
}

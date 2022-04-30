using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class PositonSolver : Solver
{
    public override void Solve(Softbody softbody, float deltaTime)
    {

    }

    private class Computer : IJobParallelFor
    {
        public Computer(NativeArray<Particle> particles)
        {
            this.particles = particles;
        }

        NativeArray<Particle> particles;

        public void Execute(int index)
        {
            Particle particle = particles[index];



            //Vector3 acceleration = force / mass;
            //velocity += acceleration * deltaTime;


            particles[index] = particle;
        }
    }
}

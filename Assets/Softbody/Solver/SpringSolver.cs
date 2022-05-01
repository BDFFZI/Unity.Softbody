using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class SpringSolver : Solver
{
    [SerializeField] int maxIterations;

    public override void Solve(Softbody softbody, float deltaTime)
    {
        int totalIterations = Mathf.Min(maxIterations, softbody.Springs.Length);

        float groupSpringCount = softbody.Springs.Length / (float)totalIterations;
        Computer computer = new Computer(softbody.Particles, softbody.Springs, totalIterations, deltaTime);
        for (int iterations = 0; iterations < totalIterations; iterations++)
        {
            int currentSpringCount = Mathf.CeilToInt(groupSpringCount);
            if (iterations + (currentSpringCount - 1) * totalIterations > softbody.Springs.Length - 1)
                currentSpringCount--;

            computer.Iterations = iterations;
            computer.Schedule(currentSpringCount, 3).Complete();
        }
    }

    private struct Computer : IJobParallelFor
    {
        public Computer(NativeArray<Particle> particles, NativeArray<Spring> springs, int maxIterations, float deltaTime)
        {
            this.particles = particles;
            this.springs = springs;
            this.maxIterations = maxIterations;
            this.deltaTime = deltaTime;
            iterations = 0;
        }

        public int Iterations { get => iterations; set => iterations = value; }

        [NativeDisableParallelForRestriction] NativeArray<Particle> particles;
        [NativeDisableParallelForRestriction] readonly NativeArray<Spring> springs;
        readonly int maxIterations;
        int iterations;
        float deltaTime;

        public void Execute(int index)
        {
            Spring spring = springs[iterations + index * maxIterations];

            Particle particleA = particles[spring.ParticleAIndex];
            Particle particleB = particles[spring.ParticleBIndex];

            Vector3 positionAToB = particleB.GetNextState(deltaTime).Position - particleA.GetNextState(deltaTime).Position;
            Vector3 direction = positionAToB.normalized;
            float distance = positionAToB.magnitude;

            Vector3 elasticForce = spring.Elasticity * direction * (distance - spring.Length);

            Vector3 velocityA = particleA.ForceToVelocity(elasticForce, deltaTime);
            Vector3 velocityB = particleB.ForceToVelocity(-elasticForce, deltaTime);
            Vector3 velocityBToA = velocityA - velocityB;
            Vector3 resistance = -spring.Damping * direction * Vector3.Dot(velocityBToA, direction);

            particleA.Force += elasticForce + resistance/** (1 - spring.Damping)*/;
            particleB.Force -= elasticForce + resistance/** (1 - spring.Damping)*/;

            particles[spring.ParticleAIndex] = particleA;
            particles[spring.ParticleBIndex] = particleB;
        }
    }
}

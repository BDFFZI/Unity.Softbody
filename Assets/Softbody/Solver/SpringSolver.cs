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
        int groupCount = softbody.Springs.Length / maxIterations;
        Computer computer = new Computer(softbody.Particles, softbody.Springs, groupCount, deltaTime);
        for (int i = 0; i < maxIterations; i++)
        {
            computer.Iterations = i;
            computer.Schedule(groupCount, 3).Complete();
        }
    }

    private struct Computer : IJobParallelFor
    {
        public Computer(NativeArray<Particle> particles, NativeArray<Spring> springs, int groupCount, float deltaTime)
        {
            this.particles = particles;
            this.springs = springs;
            this.groupCount = groupCount;
            this.deltaTime = deltaTime;
            iterations = 0;
        }

        public int Iterations { get => iterations; set => iterations = value; }

        [NativeDisableParallelForRestriction] NativeArray<Particle> particles;
        readonly NativeArray<Spring> springs;
        readonly int groupCount;
        int iterations;
        float deltaTime;

        public void Execute(int index)
        {
            Spring spring = springs[iterations + index * groupCount];

            Particle particleA = particles[spring.ParticleAIndex];
            Particle particleB = particles[spring.ParticleBIndex];

            Vector3 positionAToB = particleB.Position - particleA.Position;
            Vector3 direction = positionAToB.normalized;
            float distance = positionAToB.magnitude;

            Vector3 elasticForce = spring.Elasticity * direction * (distance - spring.Length);

            Vector3 velocityA = particleA.ForceToVelocity(elasticForce, deltaTime);
            Vector3 velocityB = particleB.ForceToVelocity(-elasticForce, deltaTime);
            Vector3 velocityBToA = velocityA - velocityB;
            Vector3 resistance = -spring.Damping * direction * Vector3.Dot(velocityBToA, direction);

            // Vector3 d_ab = -v_ab.normalized * kd * Mathf.Abs(Vector3.Dot(v_ab, pos_ab.normalized));
            //第二种写法 v normal决定方向 abs dot决定大小，测试出来数据有点小差异 后面验证
            particleA.Force += elasticForce + resistance;
            particleB.Force -= elasticForce + resistance;

            particles[spring.ParticleAIndex] = particleA;
            particles[spring.ParticleBIndex] = particleB;
        }
    }
}

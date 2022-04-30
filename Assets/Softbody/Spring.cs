using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public struct Spring
{
    public Spring(int particleAIndex, int particleBIndex, float length, float elasticity, float damping)
    {
        this.particleAIndex = particleAIndex;
        this.particleBIndex = particleBIndex;
        this.length = length;
        this.elasticity = elasticity;
        this.damping = damping;
        color = new Color(1, 1, 1, 0.3f);
    }

    public Spring(int particleAIndex, int particleBIndex, List<Particle> particles, float elasticity, float damping)
    {
        this.particleAIndex = particleAIndex;
        this.particleBIndex = particleBIndex;

        Vector3 positionA = particles[particleAIndex].Position;
        Vector3 positionB = particles[particleBIndex].Position;

        this.length = Vector3.Distance(positionA, positionB);
        this.elasticity = elasticity;
        this.damping = damping;
        color = new Color(1, 1, 1, 0.3f);
    }


    public int ParticleAIndex { get => particleAIndex; set => particleAIndex = value; }
    public int ParticleBIndex { get => particleBIndex; set => particleBIndex = value; }
    public float Elasticity { get => elasticity; set => elasticity = value; }
    public float Length { get => length; set => length = value; }
    public float Damping { get => damping; set => damping = value; }
    public Color Color { get => color; set => color = value; }

    public void Draw(Vector3 positionA, Vector3 positionB)
    {
        Color source = Gizmos.color;

        Gizmos.color = color;
        Gizmos.DrawLine(positionA, positionB);

        Gizmos.color = source;
    }

    [SerializeField] int particleAIndex;
    [SerializeField] int particleBIndex;
    [SerializeField] float length;
    [SerializeField] float elasticity;
    [SerializeField] float damping;
    [SerializeField] Color color;
}


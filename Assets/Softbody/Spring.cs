using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Spring
{
    public Spring(int particleAIndex, int particleBIndex, float elasticity, float damping)
    {
        this.particleAIndex = particleAIndex;
        this.particleBIndex = particleBIndex;
        this.elasticity = elasticity;
        this.damping = damping;
    }

    public int ParticleAIndex { get => particleAIndex; set => particleAIndex = value; }
    public int ParticleBIndex { get => particleBIndex; set => particleBIndex = value; }

    [SerializeField] int particleAIndex;
    [SerializeField] int particleBIndex;
    [SerializeField] float elasticity;
    [SerializeField] float damping;


}


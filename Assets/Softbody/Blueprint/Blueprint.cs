using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "SoftbodyBlueprint")]
public class Blueprint : ScriptableObject
{
    public List<Particle> Particles { get => particles; set => particles = value; }
    public List<Spring> Springs { get => springs; set => springs = value; }

    [SerializeField] List<Particle> particles;
    [SerializeField] List<Spring> springs;
}
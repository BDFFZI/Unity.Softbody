using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Particle
{
    public Vector3 Position { get => position; set => position = value; }
    public float Mass { get => mass; set => mass = value; }
    public float Drag { get => drag; set => drag = value; }
    public bool UseGravity { get => useGravity; set => useGravity = value; }
    public bool IsKinematic { get => isKinematic; set => isKinematic = value; }
    public Vector3 Force { get => force; set => force = value; }

    [SerializeField] Vector3 position;
    [SerializeField] float mass;
    [SerializeField] float drag;
    [SerializeField] bool useGravity;
    [SerializeField] bool isKinematic;
    Vector3 force;
    Vector3 velocity;

    public void AddForce(Vector3 force)
    {
        force += force;
    }

    public void a()
    {
        Rigidbody a;

    }
}

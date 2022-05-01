using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

[Serializable]
public struct Particle
{
    public Particle(Vector3 position)
    {
        this.position = position;
        this.mass = 1;
        this.drag = 1;
        this.useGravity = true;
        this.isKinematic = false;
        this.force = Vector3.zero;
        this.velocity = Vector3.zero;
        color = new Color(1, 1, 1, 0.3f);
    }

    public Vector3 Position { get => position; set => position = value; }
    public float Mass { get => mass; set => mass = value; }
    public float Drag { get => drag; set => drag = value; }
    public bool UseGravity { get => useGravity; set => useGravity = value; }
    public bool IsKinematic { get => isKinematic; set => isKinematic = value; }
    public Vector3 Force { get => force; set => force = value; }
    public Vector3 Velocity { get => velocity; set => velocity = value; }

    public Vector3 ForceToVelocity(Vector3 force, float deltaTime)
    {
        Vector3 acceleration = force / mass;
        Vector3 velocity = acceleration * deltaTime;

        return velocity;
    }
    public Vector3 VelocityToForce(Vector3 velocity, float deltaTime)
    {
        Vector3 acceleration = velocity / deltaTime;
        Vector3 force = acceleration * mass;
        return force;
    }

    public Vector3 GetNextPosition(float deltaTime)
    {
        return position + velocity * deltaTime;
    }

    public Particle GetNextState(float deltaTime)
    {
        Particle particle = this;

        if (particle.IsKinematic)
        {
            particle.Force = Vector3.zero;
            particle.Velocity = Vector3.zero;
            return particle;
        }

        Vector3 acceleration = Vector3.zero;

        //重力
        if (particle.UseGravity)
        {
            Vector3 gravity = Vector3.down * particle.Mass * 9.8f;
            acceleration += gravity;
        }

        //空气阻力
        Vector3 airDrag = Mathf.Min(particle.Drag * particle.Velocity.sqrMagnitude, particle.Force.magnitude) * -particle.Velocity.normalized;
        particle.Force += airDrag;
        //particle.Force *= 1 - particle.Drag;

        acceleration += particle.Force / particle.Mass;
        particle.Velocity += acceleration * deltaTime;
        particle.Position += particle.Velocity * deltaTime;

        particle.Force = Vector3.zero;

        return particle;
    }

    public void Draw()
    {
        Color source = Gizmos.color;

        Gizmos.color = color;
        Gizmos.DrawSphere(position, 0.01f);
        //if (velocity != Vector3.zero)
        //{
        //    Gizmos.DrawRay(position, velocity);
        //}

        Gizmos.color = source;
    }

    [SerializeField] Vector3 position;
    [SerializeField] float mass;
    [SerializeField] float drag;
    [SerializeField] bool useGravity;
    [SerializeField] bool isKinematic;
    [SerializeField] Vector3 force;
    [SerializeField] Vector3 velocity;
    [SerializeField] Color color;

    public override string ToString()
    {
        return Position.ToString();
    }
}

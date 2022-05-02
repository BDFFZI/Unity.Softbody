using ParticleSpring;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Softbody : MonoBehaviour
{
    public void SetParticle(int index, Particle particle)
    {

        particles[index] = particle;
    }
    public NativeArray<Particle> Particles => particles;
    public NativeArray<Spring> Springs => springs;
    public Blueprint Blueprint { get => blueprint; }

    [SerializeField] MeshUpdater meshUpdater;
    [SerializeField] Blueprint blueprint;

    NativeArray<Particle> particles;
    NativeArray<Spring> springs;

    private void Awake()
    {
        particles = new NativeArray<Particle>(Blueprint.Particles.ToArray(), Allocator.Persistent);
        for (int i = 0; i < particles.Length; i++)
        {
            Particle particle = particles[i];
            particle.Position = transform.localToWorldMatrix.MultiplyPoint(particle.Position);
            particles[i] = particle;
        }

        springs = new NativeArray<Spring>(Blueprint.Springs.ToArray(), Allocator.Persistent);
    }

    private void Update()
    {
        Vector3 center = Vector3.zero;
        for (int i = 0; i < particles.Length; i++)
        {
            center += particles[i].Position;
        }
        center /= particles.Length;
        transform.position = center;

        int length = meshUpdater.Vertices.Length;
        Matrix4x4 worldToLocalMatrix = transform.worldToLocalMatrix;

        for (int i = 0; i < length; i++)
        {
            meshUpdater.Vertices[i] = worldToLocalMatrix.MultiplyPoint3x4(particles[i].Position);
        }
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying == false)
            Gizmos.matrix = transform.localToWorldMatrix;

        if (Application.isPlaying)
        {
            //绘制质点
            int particlesCount = Particles.Length;
            for (int i = 0; i < particlesCount; i++)
            {
                Particles[i].Draw();
            }
            //绘制弹簧
            int springsCount = Springs.Length;
            for (int i = 0; i < springsCount; i++)
            {
                Spring spring = Springs[i];

                Vector3 partcleAPosition = Particles[spring.ParticleAIndex].Position;
                Vector3 partcleBPosition = Particles[spring.ParticleBIndex].Position;

                Gizmos.DrawLine(partcleAPosition, partcleBPosition);
            }
        }
        else
        {
            //绘制质点
            int particlesCount = blueprint.Particles.Count;
            for (int i = 0; i < particlesCount; i++)
            {
                blueprint.Particles[i].Draw();
            }
            //绘制弹簧
            int springsCount = blueprint.Springs.Count;
            for (int i = 0; i < springsCount; i++)
            {
                Spring spring = blueprint.Springs[i];

                Vector3 partcleAPosition = blueprint.Particles[spring.ParticleAIndex].Position;
                Vector3 partcleBPosition = blueprint.Particles[spring.ParticleBIndex].Position;

                spring.Draw(partcleAPosition, partcleBPosition);
            }
        }
    }

    private void OnDestroy()
    {
        Particles.Dispose();
        Springs.Dispose();
    }
}

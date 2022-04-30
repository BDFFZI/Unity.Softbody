using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Softbody : MonoBehaviour
{
    public NativeArray<Particle> Particles { get; private set; }
    public NativeArray<Spring> Springs { get; private set; }
    public Blueprint Blueprint { get => blueprint; }

    [SerializeField] Blueprint blueprint;
    private void OnDrawGizmos()
    {
        //绘制质点
        int particlesCount = blueprint.Particles.Count;
        for (int i = 0; i < particlesCount; i++)
        {
            Gizmos.DrawSphere(blueprint.Particles[i].Position, 0.01f);
        }
        //绘制弹簧
        int springsCount = blueprint.Springs.Count;
        for (int i = 0; i < springsCount; i++)
        {
            Spring spring = blueprint.Springs[i];

            Vector3 partcleAPosition = blueprint.Particles[spring.ParticleAIndex].Position;
            Vector3 partcleBPosition = blueprint.Particles[spring.ParticleBIndex].Position;

            Gizmos.DrawLine(partcleAPosition, partcleBPosition);
        }
    }
}

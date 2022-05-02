using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Codice.CM.Common.CmCallContext;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.ParticleSystem;

public class BlueprintEditor : MonoBehaviour
{
    [SerializeField] Blueprint blueprint;
    [SerializeField] Mesh mesh;
    [SerializeField] float elasticity;
    [SerializeField] float drag;

    [ContextMenu("CreateLine")]
    void CreateLine()
    {
        blueprint.Particles = new List<Particle>();
        for (int x = 0; x < 30; x++)
        {
            blueprint.Particles.Add(new Particle(new Vector3(x / 10f, 0, 0)) { Drag = 1 });
        }
        Particle particle = blueprint.Particles[0];
        particle.IsKinematic = true;
        blueprint.Particles[0] = particle;

        blueprint.Springs = new List<Spring>();
        for (int x = 0; x < 29; x++)
        {
            blueprint.Springs.Add(new Spring(x, x + 1, 0.1f, 6000, 1));
        }
    }

    [ContextMenu("CreateCloth")]
    void CreateCloth()
    {
        blueprint.Particles = new List<Particle>();
        for (int y = 0; y < 30; y++)
        {
            for (int x = 0; x < 30; x++)
            {
                blueprint.Particles.Add(new Particle(new Vector3(x / 10f, -y / 10f, 0)) { Drag = 100f });
            }
        }

        Particle particle0 = blueprint.Particles[0];
        particle0.IsKinematic = true;
        blueprint.Particles[0] = particle0;
        Particle particle29 = blueprint.Particles[29];
        particle29.IsKinematic = true;
        blueprint.Particles[29] = particle29;

        blueprint.Springs = new List<Spring>();

        //行结构弹簧
        for (int y = 0; y < 30; y++)
        {
            for (int x = 0; x < 29; x++)
            {
                blueprint.Springs.Add(new Spring(y * 30 + x, y * 30 + x + 1, blueprint.Particles, 3000, 1f));
            }
        }

        //列结构弹簧
        for (int x = 0; x < 30; x++)
        {
            for (int y = 0; y < 29; y++)
            {
                blueprint.Springs.Add(new Spring(y * 30 + x, (y + 1) * 30 + x, blueprint.Particles, 3000, 1f));
            }
        }

        //行剪切弹簧
        for (int y = 0; y < 29; y++)
        {
            for (int x = 0; x < 29; x++)
            {
                blueprint.Springs.Add(new Spring(y * 30 + x, (y + 1) * 30 + x + 1, blueprint.Particles, 3000, 1f));
            }
        }

        //列剪切弹簧
        for (int y = 0; y < 29; y++)
        {
            for (int x = 1; x < 30; x++)
            {
                blueprint.Springs.Add(new Spring(y * 30 + x, (y + 1) * 30 + x - 1, blueprint.Particles, 3000, 1f));
            }
        }

        //行弯曲弹簧
        for (int y = 0; y < 30; y++)
        {
            for (int x = 0; x < 28; x++)
            {
                blueprint.Springs.Add(new Spring(y * 30 + x, y * 30 + x + 2, blueprint.Particles, 3000, 1f));
            }
        }

        //列弯曲弹簧
        for (int x = 0; x < 30; x++)
        {
            for (int y = 0; y < 28; y++)
            {
                blueprint.Springs.Add(new Spring(y * 30 + x, (y + 2) * 30 + x, blueprint.Particles, 3000, 1f));
            }
        }
    }

    [ContextMenu("CreateCube")]
    void CreateCube()
    {
        int length = 5;

        blueprint.Particles = new List<Particle>();
        for (int x = 0; x < length; x++)
        {
            for (int y = 0; y < length; y++)
            {
                for (int z = 0; z < length; z++)
                {
                    blueprint.Particles.Add(new Particle(new Vector3(x, y, z) / length));
                }
            }
        }
        //Particle particle0 = blueprint.Particles[0];
        //particle0.IsKinematic = true;
        //blueprint.Particles[0] = particle0;
        //Particle particle9 = blueprint.Particles[9];
        //particle9.IsKinematic = true;
        //blueprint.Particles[9] = particle9;

        blueprint.Springs = new List<Spring>();
        float distance = Mathf.Sqrt(3f / (length * length));
        for (int current = 0; current < blueprint.Particles.Count; current++)
        {
            for (int target = current + 1; target < blueprint.Particles.Count; target++)
            {
                Particle currentParticle = blueprint.Particles[current];
                Particle targetParticle = blueprint.Particles[target];

                if (Vector3.Distance(currentParticle.Position, targetParticle.Position) < distance)
                {
                    blueprint.Springs.Add(new Spring(current, target, blueprint.Particles, 4000, 1));
                }
            }
        }
    }

    [ContextMenu("CreateMesh")]
    void CreateMesh()
    {
        float minDistance = 0.3f;
        //外层质点
        Vector3[] vertices = mesh.vertices;
        blueprint.Particles = vertices.Select(vertex => new Particle(vertex)).ToList();

        blueprint.Springs = new List<Spring>();
        int[] traingles = mesh.triangles;
        for (int i = 0; i < traingles.Length; i += 3)
        {
            int particle0Index = traingles[i + 0];
            int particle1Index = traingles[i + 1];
            int particle2Index = traingles[i + 2];

            blueprint.Springs.Add(new Spring(particle0Index, particle1Index, blueprint.Particles, 3000, 1));
            blueprint.Springs.Add(new Spring(particle1Index, particle2Index, blueprint.Particles, 3000, 1));
            blueprint.Springs.Add(new Spring(particle2Index, particle0Index, blueprint.Particles, 3000, 1));
        }

        List<Vector3> insidePositions = new List<Vector3>();

        //内部质点
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 currentVertex = vertices[i];
            currentVertex = Vector3.MoveTowards(currentVertex, Vector3.zero, minDistance);
            while (currentVertex != Vector3.zero)
            {
                insidePositions.Add(currentVertex);
                currentVertex = Vector3.MoveTowards(currentVertex, Vector3.zero, minDistance);
            }
        }

        //合并内部质点
        for (int current = 0; current < insidePositions.Count; current++)
        {
            for (int target = current + 1; target < insidePositions.Count; target++)
            {
                if (Vector3.Distance(insidePositions[current], insidePositions[target]) < minDistance)
                {
                    insidePositions.RemoveAt(target);
                    target--;
                }
            }
        }

        blueprint.Particles.AddRange(insidePositions.Select(position => new Particle(position) { }));


        //生成弹簧
        for (int current = 0; current < blueprint.Particles.Count; current++)
        {
            for (int target = current + 1; target < blueprint.Particles.Count; target++)
            {
                if (Vector3.Distance(blueprint.Particles[current].Position, blueprint.Particles[target].Position) < minDistance * 2)
                {
                    blueprint.Springs.Add(new Spring(current, target, blueprint.Particles, 3000, 1));
                }
            }
        }
    }
}

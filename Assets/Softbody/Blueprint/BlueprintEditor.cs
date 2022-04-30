using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlueprintEditor : MonoBehaviour
{
    [SerializeField] Blueprint blueprint;

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

        //ÐÐ½á¹¹µ¯»É
        for (int y = 0; y < 30; y++)
        {
            for (int x = 0; x < 29; x++)
            {
                blueprint.Springs.Add(new Spring(y * 30 + x, y * 30 + x + 1, blueprint.Particles, 3000, 1f));
            }
        }

        //ÁÐ½á¹¹µ¯»É
        for (int x = 0; x < 30; x++)
        {
            for (int y = 0; y < 29; y++)
            {
                blueprint.Springs.Add(new Spring(y * 30 + x, (y + 1) * 30 + x, blueprint.Particles, 3000, 1f));
            }
        }

        //ÐÐ¼ôÇÐµ¯»É
        for (int y = 0; y < 29; y++)
        {
            for (int x = 0; x < 29; x++)
            {
                blueprint.Springs.Add(new Spring(y * 30 + x, (y + 1) * 30 + x + 1, blueprint.Particles, 3000, 1f));
            }
        }

        //ÁÐ¼ôÇÐµ¯»É
        for (int y = 0; y < 29; y++)
        {
            for (int x = 1; x < 30; x++)
            {
                blueprint.Springs.Add(new Spring(y * 30 + x, (y + 1) * 30 + x - 1, blueprint.Particles, 3000, 1f));
            }
        }

        //ÐÐÍäÇúµ¯»É
        for (int y = 0; y < 30; y++)
        {
            for (int x = 0; x < 28; x++)
            {
                blueprint.Springs.Add(new Spring(y * 30 + x, y * 30 + x + 2, blueprint.Particles, 3000, 1f));
            }
        }

        //ÁÐÍäÇúµ¯»É
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
        blueprint.Particles = new List<Particle>();
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                for (int z = 0; z < 10; z++)
                {
                    blueprint.Particles.Add(new Particle(new Vector3(x, y, z) / 10));
                }
            }
        }
        Particle particle0 = blueprint.Particles[0];
        particle0.IsKinematic = true;
        blueprint.Particles[0] = particle0;
        //Particle particle9 = blueprint.Particles[9];
        //particle9.IsKinematic = true;
        //blueprint.Particles[9] = particle9;

        blueprint.Springs = new List<Spring>();
        float distance = Mathf.Sqrt(0.1f * 0.1f * 3);
        for (int current = 0; current < blueprint.Particles.Count; current++)
        {
            for (int target = current + 1; target < blueprint.Particles.Count; target++)
            {
                Particle currentParticle = blueprint.Particles[current];
                Particle targetParticle = blueprint.Particles[target];

                if (Vector3.Distance(currentParticle.Position, targetParticle.Position) < distance)
                {
                    blueprint.Springs.Add(new Spring(current, target, blueprint.Particles, 3000, 1));
                }
            }
        }
    }
}

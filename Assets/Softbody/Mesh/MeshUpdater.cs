using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ParticleSpring
{
    public class MeshUpdater : MonoBehaviour
    {
        public Vector3[] Vertices { get; set; }

        [SerializeField] MeshFilter meshFilter;
        [SerializeField] MeshCollider meshCollider;
        Mesh mesh;
        private void Awake()
        {
            mesh = meshFilter.mesh;
            Vertices = mesh.vertices;
        }

        private void LateUpdate()
        {
            mesh.vertices = Vertices;
            if (meshCollider != null)
                meshCollider.sharedMesh = mesh;

            mesh.RecalculateNormals();
        }
    }
}

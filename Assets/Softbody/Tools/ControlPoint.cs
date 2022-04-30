using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ControlPoint : MonoBehaviour
{
    [SerializeField] int index;
    [SerializeField] Softbody softbody;
    [SerializeField] Particle particle;
    // Start is called before the first frame update
    private void Awake()
    {
        particle = softbody.Particles[index];
    }

    // Update is called once per frame
    void Update()
    {
        softbody.SetParticle(index, particle);
    }
}

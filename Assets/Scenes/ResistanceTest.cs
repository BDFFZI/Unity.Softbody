using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResistanceTest : MonoBehaviour
{
    [SerializeField] ParticleSpring.BoxCollider boxCollider;
    [SerializeField] new Rigidbody rigidbody;

    private void FixedUpdate()
    {
        rigidbody.AddForceAtPosition(boxCollider.Resistance * 2, boxCollider.ResistancePosition);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] ParticleSpring.BoxCollider boxCollider;
    [SerializeField] new Rigidbody rigidbody;

    private void FixedUpdate()
    {
        rigidbody.AddForceAtPosition(boxCollider.Resistance * 0.05f, boxCollider.ResistancePosition);
    }
}

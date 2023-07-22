using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableRagdoll : MonoBehaviour
{
    [SerializeField] Rigidbody[] rigidbodies;
    [SerializeField] CapsuleCollider[] colliders;
    // Start is called before the first frame update
    void Start()
    {
        rigidbodies = this.GetComponentsInChildren<Rigidbody>();
        colliders = this.GetComponentsInChildren<CapsuleCollider>();
        DisableRagdoll();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DisableRagdoll()
    {
        foreach(Rigidbody r in rigidbodies)
        {
            r.isKinematic = true;
        }

        foreach(CapsuleCollider c in colliders)
        {
            c.enabled = false;
        }
    }
}

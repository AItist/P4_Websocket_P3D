using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindCollider : MonoBehaviour
{
    [SerializeField]
    List<Collider> colliders;

    private void Awake()
    {
        GetColliderAndSet();
    }
    void GetColliderAndSet()
    {
        // colliders.InsertRange()
        foreach (Collider col in transform.GetComponentsInChildren<Collider>())
        {
            colliders.Add(col);
            col.transform.GetComponent<Rigidbody>().useGravity = false;
        }
    }
}

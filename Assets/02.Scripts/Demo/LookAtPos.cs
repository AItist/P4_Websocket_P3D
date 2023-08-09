using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPos : MonoBehaviour
{
    public Transform lookAtTransform;

    // Start is called before the first frame update
    void Start()
    {
        transform.LookAt(lookAtTransform);
    }

    private void Update()
    {
        transform.LookAt(lookAtTransform);
    }
}

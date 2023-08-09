using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiddlePointAndDirection : MonoBehaviour
{
    public Transform lShoulder;
    public Transform rShoulder;

    // Update is called once per frame
    void Update()
    {
        transform.position = (lShoulder.position + rShoulder.position) / 2;
        transform.LookAt(lShoulder);
        transform.Rotate(new Vector3(0, 90, 0));

    }
}

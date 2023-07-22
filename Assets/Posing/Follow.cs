using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using JetBrains.Annotations;

public class Follow : MonoBehaviour
{
    public GameObject rig;
    public GameObject pelvis;
    public Vector3 childPos;
    public Quaternion childRot;

    void ChangeRot()
    {
        if(pelvis != null)
        {
            var changedPelvisRot = pelvis.transform.rotation;
            rig.transform.localRotation = changedPelvisRot;
            pelvis.transform.localRotation = Quaternion.identity;
            transform.localRotation = rig.transform.localRotation;
            rig.transform.localRotation = Quaternion.identity;
        }
        else
        {
            var changedChildRot = this.rig.transform.rotation;
            Debug.Log($"changeChildRot: {changedChildRot}");
            transform.rotation = changedChildRot;
            rig.transform.localRotation = Quaternion.identity;
        }
     
    }
    void ChangePos()
    {
        if(pelvis != null)
        { 
           var changedPelvisPos = pelvis.transform.position;
            rig.transform.localPosition = new Vector3(changedPelvisPos.x, 0, changedPelvisPos.z);  
            pelvis.transform.localPosition = new Vector3(0,changedPelvisPos.y, 0);
            transform.localPosition = rig.transform.localPosition;
            rig.transform.localPosition = Vector3.zero;
        }
        else
        {
            var childPos = this.rig.transform.position;
            transform.localPosition = childPos;
            rig.transform.localPosition = Vector3.zero;
        }
    
    }
    public void ChangePosRot()
    {
        ChangePos();
        ChangeRot();
    }


}


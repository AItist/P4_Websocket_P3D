using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum BoneProperty
{
    Pelvis = 0,

    L_Hand = 11,
    L_Elbow = 12,
    L_Arm = 13,
    R_Hand = 21,
    R_Elbow = 22,
    R_Arm = 23,

    L_UpLeg = 31,
    L_Leg = 32,
    L_Foot = 33,

    R_UpLeg = 41,
    R_Leg = 42,
    R_Foot = 43

}

public class Rotation_ : MonoBehaviour
{
    public bool isUpdate;
    public Transform target;
    public Transform center;
    public Transform origin;

    public Transform rotTarget;
    public BoneProperty boneProperty;

    [Button]
    public void Rotation_TwoLine(Transform objective)
    {
        Vector3 _a = target.position;
        Vector3 _b = center.position;
        Vector3 _c = origin.position;

        Vector3 AB = _a - _b;
        Vector3 BC = _c - _b;

        Quaternion rotation = Quaternion.FromToRotation(AB, BC);

        // 추가적으로 z축을 중심으로 90도 회전을 적용합니다.
        Quaternion additionalRotation = Quaternion.Euler(-90, 0, 0);
        Quaternion finalRotation = rotation * additionalRotation;

        objective.rotation = finalRotation;
    }

    public void Rotation_LookAt(Quaternion mul)
    {
        center.LookAt(target.position);
        center.rotation = center.rotation * mul;
        //center.Rotate(new Vector3(90, 0, 0));
        //center.LookAt(target.position, Vector3.back);
    }

    public void Rotation_Hand(Transform target, Quaternion mul)
    {
        center.LookAt(target.position);
        center.rotation = center.rotation * mul;
    }

    public void Rotation_LookAt_B(Quaternion mul)
    {
        center.LookAt(target.position);
        center.rotation = center.rotation * mul;
        center.Rotate(new Vector3(0, 180, 0));
        //center.Rotate(new Vector3(90, 0, 0));
        //center.LookAt(target.position, Vector3.back);
    }

    public void Rotation_LookAt_Center(Vector3 dir, Vector3 rot)
    {
        Vector3 direction = target.position - origin.position;

        rotTarget.rotation = Quaternion.FromToRotation(dir, direction);
        rotTarget.Rotate(rot);

        //center.LookAt(target.position);
        //center.rotation = center.rotation * mul;
    }

    public void Rotation_Origin_To(Transform objective, Quaternion mul)
    {
        rotTarget.rotation = objective.rotation;
        rotTarget.rotation *= mul;
    }

    public void Rotation_Duplicate(Transform objective)
    {
        rotTarget.rotation = objective.rotation;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isUpdate) { return; }
        if (target == null || center == null || origin == null || rotTarget == null) return;

        //if (boneProperty == BoneProperty.Pelvis)
        //{
        //    Rotation_LookAt_Center(Quaternion.Euler(90, 0, 0));
        //    //Rotation_Duplicate(origin);
        //}

        if (boneProperty == BoneProperty.L_Hand)
        {
            //Rotation_Duplicate(origin);
            Rotation_LookAt_Center(Vector3.left, new Vector3(180, 0, 0));
        }
        else if (boneProperty == BoneProperty.R_Hand)
        {
            //Rotation_Duplicate(origin);
            Rotation_LookAt_Center(Vector3.left, new Vector3(0, 0, 0));
        }
        else if (boneProperty == BoneProperty.L_Foot)
        {
            //Rotation_Duplicate(origin);
            Rotation_LookAt_Center(Vector3.forward, new Vector3(0, 90, 0));
        }
        else if (boneProperty == BoneProperty.R_Foot)
        {
            //Rotation_Duplicate(origin);
            Rotation_LookAt_Center(Vector3.forward, new Vector3(0, 90, 0));
        }
        //else if (boneProperty == BoneProperty.L_Elbow)
        //{
        //    Rotation_LookAt(Quaternion.Euler(90, 0, 0));
        //}
        //else if (boneProperty == BoneProperty.L_Arm)
        //{
        //    Rotation_LookAt(Quaternion.Euler(90, 0, 0));
        //}
        //else if (boneProperty == BoneProperty.R_Hand)
        //{
        //    Rotation_Duplicate(origin);
        //}
        //else if (boneProperty == BoneProperty.R_Elbow)
        //{
        //    //Rotation_TwoLine(rotTarget);
        //    Rotation_LookAt(Quaternion.Euler(90, 0, 0));
        //}
        //else if (boneProperty == BoneProperty.R_Arm)
        //{
        //    //Rotation_TwoLine(rotTarget);
        //    Rotation_LookAt(Quaternion.Euler(90, 0, 0));
        //}

        //else if (boneProperty == BoneProperty.L_UpLeg)
        //{
        //    //Rotation_TwoLine(rotTarget);
        //    Rotation_LookAt_B(Quaternion.Euler(90, 0, 0));
        //}
        //else if (boneProperty == BoneProperty.L_Leg)
        //{
        //    //Rotation_TwoLine(rotTarget);
        //    Rotation_LookAt(Quaternion.Euler(90, 0, 0));
        //}
        //else if (boneProperty == BoneProperty.L_Foot)
        //{
        //    Rotation_Origin_To(origin, Quaternion.Euler(75, 0, 0));
        //    //Rotation_Duplicate(origin);
        //}

        //else if (boneProperty == BoneProperty.R_UpLeg)
        //{
        //    //Rotation_TwoLine(rotTarget);
        //    Rotation_LookAt_B(Quaternion.Euler(90, 0, 0));
        //}
        //else if (boneProperty == BoneProperty.R_Leg)
        //{
        //    //Rotation_TwoLine(rotTarget);
        //    Rotation_LookAt(Quaternion.Euler(90, 0, 0));
        //}
        //else if (boneProperty == BoneProperty.R_Foot)
        //{
        //    Rotation_Origin_To(origin, Quaternion.Euler(75, 0, 0));
        //    //Rotation_Duplicate(origin);
        //}

    }
}

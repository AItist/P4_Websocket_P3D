using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

/// <summary>
/// 1. pelvis가 가장 첫번째로 적용된다.(중심 위치와 회전)
/// 2. Target의 33_pelvis 아래 transform에 바꾸고자 하는 값 적용. 해당값은 Target의 posing Group에 접근)
/// 3. 값 적용 후 Init_Rig 실행.
/// 
/// </summary>
public class PosingManager : MonoBehaviour
{
    //15: leftHand
    //16: rightHand
    //27: leftFoot
    //28: rightFoot
    //13:leftforearm
    //14:rightforearm
    //25:leftleg
    //26: rightleg
    //33.pelvis
    //23: righttupleg
    //24: leftupleg
    public PosingGroup target;
    public PosingGroup origin;
    public bool headSet;
    public bool includeLengthAdjust;

    private int counter = 0;

    private void Start()
    {
        OnOFFRigBuilder(false);
        SyncRoot();
    }
    private void Update()
    {
        //return;
        counter++;
        if (counter >= 4)
        {
            StartCoroutine(InitTargetToOriginRig());
            counter = 0;
        }
    }

    [Button]
    public void Init_Rig()
    {
        StartCoroutine(InitTargetToOriginRig());
    }

    public void OnOFFRigBuilder(bool rigbguilderActive)
    {
        if (rigbguilderActive)
            origin.rigbuilder.enabled = true;
        else
            origin.rigbuilder.enabled = false;

    }
    public void EnableRigBuilder(bool enable)
    {
        if (enable)
        {
            origin.rigbuilder.enabled = true;
        }
        else
        {
            origin.rigbuilder.enabled = false;
        }
    }
    void Sync4bodies()
    {

        origin.leftArm[0].transform.position = target.rigPoint[15].transform.position;
        origin.leftArm[0].transform.rotation = target.rigPoint[15].transform.rotation;

        origin.leftArm[1].transform.position = target.rigPoint[13].transform.position;
        origin.leftArm[1].transform.rotation = target.rigPoint[13].transform.rotation;

        origin.rightArm[0].transform.position = target.rigPoint[16].transform.position;
        origin.rightArm[0].transform.rotation = target.rigPoint[16].transform.rotation;

        origin.rightArm[1].transform.position = target.rigPoint[14].transform.position;
        origin.rightArm[1].transform.rotation = target.rigPoint[14].transform.rotation;

        origin.leftLeg[0].transform.position = target.rigPoint[27].transform.position;
        origin.leftLeg[0].transform.rotation = target.rigPoint[27].transform.rotation;

        origin.leftLeg[1].transform.position = target.rigPoint[25].transform.position;
        origin.leftLeg[1].transform.rotation = target.rigPoint[25].transform.rotation;

        origin.rightLeg[0].transform.position = target.rigPoint[28].transform.position;
        origin.rightLeg[0].transform.rotation = target.rigPoint[28].transform.rotation;

        origin.rightLeg[1].transform.position = target.rigPoint[26].transform.position;
        origin.rightLeg[1].transform.rotation = target.rigPoint[26].transform.rotation;

    }
    void SyncHead()
    {
        origin.head.transform.position = target.rigPoint[0].transform.position;
        origin.head.transform.rotation = target.rigPoint[0].transform.rotation;
    
    }
    void SyncRoot()
    {
        origin.transform.position = target.transform.position;
    }
    void SyncOriginToTarget()
    {
        foreach (KeyValuePair<int, Posing> dic_origin in origin.rigPoint)
        {
            foreach (KeyValuePair<int, Posing> dic_target in target.rigPoint)
            {
                if (dic_origin.Key == dic_target.Key)
                {
                
                    dic_target.Value.SetTransform();
                    dic_origin.Value.transform.position = dic_target.Value.transform.position;
                    dic_origin.Value.transform.rotation = dic_target.Value.transform.rotation;
                    dic_origin.Value.SetCurrentTransform();
                }
            }
        }
       

    }
    void SyncTargetToOrigin()
    {
        foreach (KeyValuePair<int, Posing> dic_origin in origin.rigPoint)
        {
            foreach (KeyValuePair<int, Posing> dic_target in target.rigPoint)
            {
                if (dic_origin.Key == dic_target.Key)
                {
                    //  dic_effector.Value.curTransform = dic_outEffector.Value.curTransform;
                    //    dic_effector.Value.SetCurrentTransform();
                    dic_origin.Value.SetCurrentTransform();
                    dic_target.Value.transform.position = dic_origin.Value.transform.position;
                    dic_target.Value.transform.rotation = dic_origin.Value.transform.rotation;
                    dic_target.Value.SetCurrentTransform();
                }
            }
        }
    }
    void SetTarget_RootTransform_0()
    {
       
        target.rigPoint[33].SetCurrentTransform();
        target.follow.ChangePosRot();

    }
    void SetTarget_RootTransformToOrigin_1()
    {
       // SyncOriginToTarget();
       origin.transform.position = target.transform.position;
      
        origin.transform.rotation = target.transform.rotation;
 
     //  origin.follow.SetParentFollowChild();
    }
    void EnableHeadSet_2()
    {
        headSet = true;
    }
    void SetTarget_HeadTransform_3()
    {
        target.rigPoint[0].SetCurrentTransform();
        SyncHead();
    }
    void EnableRigBuilder_4_7_10()
    {
        
        OnOFFRigBuilder(true);
    }
    void DisableHeadSetAndRigBuilder_5()
    {
        headSet = false;
        OnOFFRigBuilder(false);
    }
    void SetTarget_FootHandTransform_6()
    {
        for (int i = 0; i < target.rigPoint.Count; i++)
        {
            if(i == 15 || i == 16 || i == 27 || i == 28)
                target.rigPoint[i].SetCurrentTransform();
            if (i == 13 || i == 14 || i == 23 || i == 24)
                target.rigPoint[i].SetCurrentTransform();
        }
        Sync4bodies();
        
    }
    void DisableRigBuilder_8()
    {
        OnOFFRigBuilder(false);
    }
    void SetRigPointWithoutRigBuilder_9()
    {
        SyncOriginToTarget();
    }
    IEnumerator InitTargetToOriginRig()
    {
        yield return null;
        DisableRigBuilder_8();
        SetTarget_RootTransform_0();
        SetTarget_RootTransformToOrigin_1();
        EnableHeadSet_2();
        SetTarget_HeadTransform_3();
        EnableRigBuilder_4_7_10();
        DisableHeadSetAndRigBuilder_5();
        SetTarget_FootHandTransform_6();
        EnableRigBuilder_4_7_10();

        if (includeLengthAdjust)
        {
            DisableRigBuilder_8();
            SetRigPointWithoutRigBuilder_9();
        }


    }


    public void SetTransforms(int index, Transform posing)
    {
        target.rigPoint[index].curTransform = posing;
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix;
using UnityEngine.Animations.Rigging;
/// <summary>
/// ai rig index¿Í transform Dictionary.
/// </summary>
public class PosingGroup : MonoBehaviour
{
    [BoxGroup("RigPoint")]
    [ShowInInspector]
    public Dictionary<int, Posing> rigPoint;
    [BoxGroup("Posing")]
    public List<Posing> posings;
    public RigBuilder rigbuilder = null;

    public GameObject[] rightArm;
    public GameObject[] leftArm;
    public GameObject[] rightLeg;
    public GameObject[] leftLeg;
    public GameObject head;
    public Transform pelvis;
    public Follow follow;

    public Vector3 pelvisOriginLocalPos;
    public Quaternion pelvisOriginLocalRot;

    private void Awake()
    {
        SetPosings_1();
        SetRigPoints_2();
        pelvisOriginLocalPos = pelvis.localPosition;
        pelvisOriginLocalRot = pelvis.localRotation;
    }
    [Button]
    void SetPosings_1()
    {

        posings.AddRange(GetComponentsInChildren<Posing>());
        foreach (Posing p in posings)
        {
            p.SetTransform();
        }
       
    }
    /// <summary>
    /// µñ¼Å³Ê¸® Å×ÀÌºíÈ­.
    /// </summary>
    void SetRigPoints_2()
    {
        rigPoint = new Dictionary<int, Posing>();
        foreach (Posing p in posings)
        {
            rigPoint.Add(p.index, p);
        }
    }
  

}


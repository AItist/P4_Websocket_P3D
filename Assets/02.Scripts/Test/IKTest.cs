using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;

public class IKTest : MonoBehaviour
{
    public RootMotion.FinalIK.FullBodyBipedIK ik;

    public Vector3 scale = new Vector3(0.5f, 0.5f, 0.5f);

    public Transform L_Hand;
    public Transform L_Elbow;

    public Transform L_O_Shoulder;
    public Transform R_O_Shoulder;

    public Transform L_Shoulder;
    public Transform R_Shoulder;

    public Transform Rescale_Root;

    // Start is called before the first frame update
    void Start()
    {

    }

    [Button]
    public void ReScaling()
    {
        Vector3 dist_O = R_O_Shoulder.position - L_O_Shoulder.position;
        Vector3 dist = R_Shoulder.position - L_Shoulder.position;
        float dist_sqr = dist.sqrMagnitude;
        float dist_O_sqr = dist_O.sqrMagnitude;
        float diff = dist_sqr / dist_O_sqr;
        Debug.Log(dist.sqrMagnitude);
        Debug.Log(dist_O.sqrMagnitude);
        Debug.Log($"diff [dist_O / dist] {diff}");
        Debug.Log("Rescale root");

        Rescale_Root.localScale = Vector3.one * diff / 2;
    }

    [Button]
    public void DoLeftHand()
    {
        ik.solver.leftHandEffector.position = L_Hand.position;
    }

    [Button]
    public void DoLeftElbow()
    {
        ik.solver.leftArmChain.bendConstraint.bendGoal.position = L_Elbow.position;
    }

    // Update is called once per frame
    void Update()
    {
        //ik.solver.leftHandEffector.position = transform.position;
    }
}

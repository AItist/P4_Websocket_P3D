using Management;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Environment;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.GettingStarted;

public class PoseDirector : MonoBehaviour
{
    /// <summary>
    /// mediapipe �������� �Ҵ�� ���� ����Ʈ ����Ʈ
    /// </summary>
    public List<Transform> riggingPoints_mediapipe;
    public List<Transform> riggingPoints_finalIK;
    public Transform riggingPoint_centerSpine;
    public Transform rig_LeftShoulder;
    public Transform rig_RightShoulder;

    [Header("������ ������ ���� ��ü")]
    public Transform L_O_Shoulder;
    public Transform R_O_Shoulder;

    public Transform Rescale_Root;

    [Header("ī�޶� �̵� �⺻ ��ġ")]
    public Transform cameraRoot;
    public Vector3 camera1_pos;

    public Transform poseRoot;

    [Button]
    public void Rescaling()
    {
        float[] diff = GetPoseScale();

        Rescale_Root.localScale = Vector3.one * (diff[1] / 2 - 0.6f);
    }

    public float[] GetPoseScale()
    {
        List<Transform> riggingPoints_ = riggingPoints_finalIK;

        Vector3 dist = riggingPoints_[12].localPosition - riggingPoints_[11].localPosition;
        float dist_sqr = dist.sqrMagnitude; // ���� ��ġ�� ����� �Ÿ�
        float dist_O_sqr = 0.16158f; // �⺻ ���� ����� �Ÿ�
        //float dist_O_sqr = (rig_RightShoulder.position - rig_LeftShoulder.position).sqrMagnitude; // �⺻ ���� ����� �Ÿ�
        float diff = dist_O_sqr / dist_sqr; // (���� ��� / �⺻ ���) ����
        //float diff = dist_sqr / dist_O_sqr; // (���� ��� / �⺻ ���) ����

        //Debug.Log("--------------------");
        //Debug.Log(dist_sqr);
        //Debug.Log(dist_O_sqr);
        //Debug.Log($"diff [dist_O / dist] {diff}");
        //Debug.Log("Rescale root");
        //Debug.Log("--------------------");

        float[] result = new float[2];
        result[0] = dist_sqr;
        result[1] = diff;

        return result;
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    /// <param name="data"></param>
    public void ApplyPose(Data.ImageData data)
    {
        //List<Transform> riggingPoints_ = riggingPoints_mediapipe;
        List<Transform> riggingPoints_ = riggingPoints_finalIK;

        Unity.Mathematics.float3[] poses = data.PoseArray;

        riggingPoint_centerSpine.localScale = Vector3.one;

        //Debug.Log($"{poses.Length}, {GlobalSetting.POSE_RIGGINGPOINTS_COUNT}");
        for (int i = 0; i < GlobalSetting.POSE_RIGGINGPOINTS_COUNT; i++)
        {
            riggingPoints_[i].position = poses[i];
        }

        float[] diff = GetPoseScale();
        cameraRoot.position = riggingPoint_centerSpine.position;
        riggingPoint_centerSpine.localScale = Vector3.one * diff[1] * 2.5f;
        poseRoot.position = cameraRoot.position;

        //Rescaling();
        //ApplyCamera(data);
    }

    //private void ApplyCamera(Data.ImageData data)
    //{
    //    // 1�� ī�޶󿡼� ������ �Ӹ� ��ġ
    //    Unity.Mathematics.float3 head = data.PoseArray_0[0];
    //    //Debug.Log($"poseDirector: head {head}");

    //    Transform camera1 = MainManager.Instance.decalContainer[0].originCamera.transform;
    //    camera1.position = camera1_pos;

    //    //Vector3 trans = new Vector3(-0.1f, 0, 0);
    //    //Vector3 trans = new Vector3(head.x, head.y, 0);

    //    //camera1.Translate(trans);
    //}

    public static (Vector3, Vector3) ClosestPointsOnTwoLines(Camera line1_originCamera, Data.ImageData line1, Camera line2_originCamera, Data.ImageData line2, int destIndex)
    {
        Vector3 u = (Vector3)line1.PoseArray[destIndex] - line1_originCamera.transform.position;
        Vector3 v = (Vector3)line2.PoseArray[destIndex] - line2_originCamera.transform.position;
        Vector3 w = line1_originCamera.transform.position - line2_originCamera.transform.position;

        float a = Vector3.Dot(u, u);
        float b = Vector3.Dot(u, v);
        float c = Vector3.Dot(v, v);
        float d = Vector3.Dot(u, w);
        float e = Vector3.Dot(v, w);
        float D = a * c - b * b;

        float sc, tc;
        if (D < Mathf.Epsilon)
        {
            // lines are almost parallel
            sc = 0.0f;
            tc = (b > c ? d / b : e / c);
        }
        else
        {
            sc = (b * e - c * d) / D;
            tc = (a * e - b * d) / D;
        }

        // Closest point on line1 to line2
        Vector3 pointOnLine1 = line1_originCamera.transform.position + sc * u;
        // Closest point on line2 to line1
        Vector3 pointOnLine2 = line2_originCamera.transform.position + tc * v;

        return (pointOnLine1, pointOnLine2);
    }
}

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
    /// mediapipe 기준으로 할당된 리깅 포인트 리스트
    /// </summary>
    public List<Transform> riggingPoints_mediapipe;
    public List<Transform> riggingPoints_finalIK;
    public Transform riggingPoint_centerSpine;
    public Transform rig_LeftShoulder;
    public Transform rig_RightShoulder;

    [Header("스케일 조정을 위한 개체")]
    public Transform L_O_Shoulder;
    public Transform R_O_Shoulder;

    public Transform Rescale_Root;

    [Header("카메라 이동 기본 위치")]
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
        float dist_sqr = dist.sqrMagnitude; // 포즈 위치의 어깨간 거리
        float dist_O_sqr = 0.16158f; // 기본 모델의 어깨간 거리
        //float dist_O_sqr = (rig_RightShoulder.position - rig_LeftShoulder.position).sqrMagnitude; // 기본 모델의 어깨간 거리
        float diff = dist_O_sqr / dist_sqr; // (포즈 어깨 / 기본 어깨) 비율
        //float diff = dist_sqr / dist_O_sqr; // (포즈 어깨 / 기본 어깨) 비율

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
    /// 포즈 적용
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
    //    // 1번 카메라에서 수집된 머리 위치
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

using Management;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Environment;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.GettingStarted;

[System.Serializable]
public struct PoseContainer
{
    public Transform rig_centerSpine;
    public Transform cameraRoot;
    public Transform poseRoot;
    public List<Transform> riggs_pose;
}

public class PoseDirector : MonoBehaviour
{
    /// <summary>
    /// mediapipe 기준으로 할당된 리깅 포인트 리스트
    /// </summary>
    public List<Transform> riggingPoints_mediapipe;
    public List<Transform> riggingPoints_finalIK;

    public List<PoseContainer> poseContainer;

    public Vector3 camera1_pos;

    public float[] GetPoseScale(List<Transform> riggingPoints_)
    {
        //List<Transform> riggingPoints_ = riggingPoints_finalIK;

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
        _ApplyPose(data, 0);
        _ApplyPose(data, 1);
        _ApplyPose(data, 2);
        _ApplyPose(data, 3);
    }

    private void _ApplyPose(Data.ImageData data, int index)
    {
        List<Transform> riggingPoints_ = null;
        Unity.Mathematics.float3[] poses = null;
        Transform centerSpine = null;
        Transform camRoot = null;
        Transform poseRoot = null;
        if (index == 0)
        {
            riggingPoints_ = poseContainer[0].riggs_pose;
            poses = data.PoseArray_0;
            centerSpine = poseContainer[0].rig_centerSpine;
            camRoot = poseContainer[0].cameraRoot;
            poseRoot = poseContainer[0].poseRoot;
        }
        else if (index == 1)
        {
            riggingPoints_ = poseContainer[1].riggs_pose;
            poses = data.PoseArray_1;
            centerSpine = poseContainer[1].rig_centerSpine;
            camRoot = poseContainer[1].cameraRoot;
            poseRoot = poseContainer[1].poseRoot;
        }
        else if (index == 2)
        {
            riggingPoints_ = poseContainer[2].riggs_pose;
            poses = data.PoseArray_2;
            centerSpine = poseContainer[2].rig_centerSpine;
            camRoot = poseContainer[2].cameraRoot;
            poseRoot = poseContainer[2].poseRoot;
        }
        else if (index == 3)
        {
            riggingPoints_ = poseContainer[3].riggs_pose;
            poses = data.PoseArray_3;
            centerSpine = poseContainer[3].rig_centerSpine;
            camRoot = poseContainer[3].cameraRoot;
            poseRoot = poseContainer[3].poseRoot;
        }

        if (poses == null ||  poses.Length == 0)
        {
            //Debug.Log("poses 값이 할당되지 않은 상태입니다.");
            return;
        }

        centerSpine.localScale = Vector3.one;

        for (int i = 0; i < GlobalSetting.POSE_RIGGINGPOINTS_COUNT; i++)
        {
            riggingPoints_[i].position = poses[i];
        }

        float[] diff = GetPoseScale(riggingPoints_);
        camRoot.position = centerSpine.position;
        centerSpine.localScale = Vector3.one * diff[1] * 2.5f;
        poseRoot.position = camRoot.position;
    }

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

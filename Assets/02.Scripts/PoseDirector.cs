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
    public List<PoseContainer> poseContainer;

    public Vector3 camera1_pos;

    private float[] GetPoseScale(List<Transform> riggingPoints_)
    {
        Vector3 dist = riggingPoints_[12].localPosition - riggingPoints_[11].localPosition;
        float dist_sqr = dist.sqrMagnitude; // ���� ��ġ�� ����� �Ÿ�
        float dist_O_sqr = 0.16158f; // �⺻ ���� ����� �Ÿ�
        float diff = dist_O_sqr / dist_sqr; // (���� ��� / �⺻ ���) ����

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
        _ApplyPose(data, 0);
        _ApplyPose(data, 1);
        _ApplyPose(data, 2);
        _ApplyPose(data, 3);
    }

    /// <summary>
    /// ī�޶� ���� ����
    /// </summary>
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

        // ���� ����� �Ѿ���� ���� ���¶�� �ߴ�
        if (poses == null ||  poses.Length == 0)
        {
            //Debug.Log("poses ���� �Ҵ���� ���� �����Դϴ�.");
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
}

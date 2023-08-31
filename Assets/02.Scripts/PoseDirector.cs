using Management;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Environment;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.GettingStarted;
using PaintIn3D;
using Data;

[System.Serializable]
public struct PoseContainer
{
    public Transform rig_centerSpine;
    public Transform cameraRoot;
    public Transform poseRoot;
    public Transform rotSource;
    public Transform rotTarget;
    public Transform camera;
    public List<Transform> riggs_pose;
}

public class PoseDirector : MonoBehaviour
{
    public List<PoseContainer> poseContainer;

    public Vector3 camera1_pos;
    public bool debug;
    public float poseScale = 2.5f;
    public Vector3 testCamMPos = Vector3.zero;
    public List<Vector3> testDecalScale;

    private float[] GetPoseScale(List<Transform> riggingPoints_)
    {
        Vector3 dist = riggingPoints_[12].localPosition - riggingPoints_[11].localPosition;
        float dist_sqr = dist.sqrMagnitude; // 포즈 위치의 어깨간 거리
        float dist_O_sqr = 0.4f; // 기본 모델의 어깨간 거리
        float diff = dist_O_sqr / dist_sqr; // (포즈 어깨 / 기본 어깨) 비율
        float reverse_diff = dist_sqr / dist_O_sqr;

        //Debug.Log("--------------------");
        //Debug.Log(dist_sqr);
        //Debug.Log(dist_O_sqr);
        //Debug.Log($"diff [dist_O / dist] {diff}");
        //Debug.Log("Rescale root");
        //Debug.Log("--------------------");

        float[] result = new float[3];
        result[0] = dist_sqr;
        result[1] = diff;
        result[2] = reverse_diff;

        return result;
    }

    /// <summary>
    /// 포즈 적용
    /// </summary>
    /// <param name="data"></param>
    public void ApplyPose(Data.ImageData data, List<DecalContainer> decalContainer)
    {
        _ApplyPose(data, 0, decalContainer);
        _ApplyPose(data, 1, decalContainer);
        _ApplyPose(data, 2, decalContainer);
        _ApplyPose(data, 3, decalContainer);
    }

    /// <summary>
    /// 카메라별 포즈 적용
    /// </summary>
    private void _ApplyPose(Data.ImageData data, int index, List<DecalContainer> decalContainer)
    {
        
        List<Transform> riggingPoints_ = null;
        Unity.Mathematics.float3[] poses = null;
        Transform centerSpine = null;
        Transform camRoot = null;
        Transform poseRoot = null;
        Transform rotSource = null;
        Transform rotTarget = null;
        P3dPaintDecal decal = null;
        Vector3 tstDScale = Vector3.one;
        Transform camPos = null;
        if (index == 0)
        {
            riggingPoints_ = poseContainer[0].riggs_pose;
            poses = data.PoseArray_0;
            centerSpine = poseContainer[0].rig_centerSpine;
            camRoot = poseContainer[0].cameraRoot;
            poseRoot = poseContainer[0].poseRoot;
            rotSource = poseContainer[0].rotSource;
            rotTarget = poseContainer[0].rotTarget;
            decal = decalContainer[0].paintDecal;

            tstDScale = testDecalScale[0];

            camPos = poseContainer[0].camera;
        }
        else if (index == 1)
        {
            riggingPoints_ = poseContainer[1].riggs_pose;
            poses = data.PoseArray_1;
            centerSpine = poseContainer[1].rig_centerSpine;
            camRoot = poseContainer[1].cameraRoot;
            poseRoot = poseContainer[1].poseRoot;
            rotSource = poseContainer[1].rotSource;
            rotTarget = poseContainer[1].rotTarget;
            decal = decalContainer[1].paintDecal;

            tstDScale = testDecalScale[1];

            camPos = poseContainer[1].camera;
        }
        else if (index == 2)
        {
            riggingPoints_ = poseContainer[2].riggs_pose;
            poses = data.PoseArray_2;
            centerSpine = poseContainer[2].rig_centerSpine;
            camRoot = poseContainer[2].cameraRoot;
            poseRoot = poseContainer[2].poseRoot;
            rotSource = poseContainer[2].rotSource;
            rotTarget = poseContainer[2].rotTarget;
            decal = decalContainer[2].paintDecal;

            tstDScale = testDecalScale[2];

            camPos = poseContainer[2].camera;
        }
        else if (index == 3)
        {
            riggingPoints_ = poseContainer[3].riggs_pose;
            poses = data.PoseArray_3;
            centerSpine = poseContainer[3].rig_centerSpine;
            camRoot = poseContainer[3].cameraRoot;
            poseRoot = poseContainer[3].poseRoot;
            rotSource = poseContainer[3].rotSource;
            rotTarget = poseContainer[3].rotTarget;
            decal = decalContainer[3].paintDecal;

            tstDScale = testDecalScale[3];

            camPos = poseContainer[3].camera;
        }

        // 현재 포즈값이 넘어오지 않은 상태라면 중단
        if (poses == null ||  poses.Length == 0)
        {
            //Debug.Log("poses 값이 할당되지 않은 상태입니다.");
            return;
        }

        centerSpine.localScale = Vector3.one;

        // 받은 값을 riggingPoints에 적용한다.
        for (int i = 0; i < GlobalSetting.POSE_RIGGINGPOINTS_COUNT; i++)
        {
            riggingPoints_[i].position = poses[i];
        }

        if (debug)
        {
            // 전체 회전값을 가져오고 적용한다.
            Quaternion modelRotation = GetRotation(rotSource, riggingPoints_[11], riggingPoints_[12]);
            //rotTarget.rotation = modelRotation;
            Debug.Log(modelRotation.eulerAngles);
            float rotY = modelRotation.eulerAngles.y;
            rotTarget.rotation = Quaternion.Euler(new Vector3(0, rotY + 90, 0));
            poseRoot.rotation = Quaternion.identity;
            poseRoot.Rotate(new Vector3(0, rotY + 90, 0));
        }

        float[] diff = GetPoseScale(riggingPoints_);
        Debug.Log($"{diff[0]} /// {diff[1]} /// {diff[2]}");

        //float diff_texture = diff[1] * 3;

        if (diff[1] < 1)
        {
            diff[1] = diff[1] * 3;
        }

        decal.Scale = Vector3.one * diff[1];
        //camPos.localPosition = new Vector3(0, 0, -diff[1]);
        //decal.Scale = tstDScale;

        camRoot.position = centerSpine.position + testCamMPos;
        centerSpine.localScale = Vector3.one;
        poseRoot.position = camRoot.position;
    }

    public Vector3 tstVec;

    public Quaternion GetRotation(Transform target, Transform lShoulder, Transform rShoulder)
    {
        target.position = (lShoulder.position + rShoulder.position) / 2;
        target.LookAt(lShoulder);
        target.Rotate(new Vector3(0+tstVec.x, 0+tstVec.y, 0+ tstVec.z));

        return target.rotation;
    }
}

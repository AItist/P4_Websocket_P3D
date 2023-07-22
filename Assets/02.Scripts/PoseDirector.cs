using Management;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Environment;

public class PoseDirector : MonoBehaviour
{
    /// <summary>
    /// mediapipe 기준으로 할당된 리깅 포인트 리스트
    /// </summary>
    public List<Transform> riggingPoints_mediapipe;

    /// <summary>
    /// 포즈 적용
    /// </summary>
    /// <param name="data"></param>
    public void ApplyPose(Data.ImageData data)
    {
        Unity.Mathematics.float3[] poses = data.PoseArray;
        //Debug.Log($"{poses.Length}, {GlobalSetting.POSE_RIGGINGPOINTS_COUNT}");
        for (int i = 0; i < GlobalSetting.POSE_RIGGINGPOINTS_COUNT; i++)
        {
            riggingPoints_mediapipe[i].position = poses[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        return;
        var dic = MainManager.Instance.ImgDics;
        //Debug.Log(dic.Keys.Count);

        int activeCount = dic.Keys.Count;

        // 포인트 개수만큼 위치추정 진행
        for (int i = 0; i < GlobalSetting.POSE_RIGGINGPOINTS_COUNT; i++)
        {
            Vector3 sum = Vector3.zero;
            int count = 0;

            try
            {
                if (activeCount == 1)
                {
                    
                    foreach (var key in dic.Keys)
                    {
                        sum = dic[key].PoseArray[i];
                        count++;
                    }
                }
                else
                {
                    var keys = new List<int>(dic.Keys);
                    var dc = MainManager.Instance.decalContainer;

                    for (int j = 0; j < keys.Count; j++)
                    {
                        for (int k = 0; k < keys.Count; k++)
                        {
                            var _closestPoints = ClosestPointsOnTwoLines(dc[keys[j]].originCamera, dic[keys[j]], dc[keys[k]].originCamera, dic[keys[k]], i);
                            sum += _closestPoints.Item1;
                            sum += _closestPoints.Item2;
                            count += 2;
                        }
                    }
                }

                // 최종 중점을 구한다.
                Vector3 _midPoint = sum / count;

                Debug.Log(_midPoint);

                riggingPoints_mediapipe[i].position = _midPoint;
            }
            catch (System.Exception e)
            {
                // TODO: 켜지지 않은 웹캠 인덱스에 대해 접근을 시도하다가 오류가 발생하는 지점이 있음
                //Debug.Log(e);

            }
        }
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

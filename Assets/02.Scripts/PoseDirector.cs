using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PoseDirector : MonoBehaviour
{
    public List<AnimationCodeMain> pointers;
    public List<Vector3> points;
    public List<Transform> riggingPoints;

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // ���� Ȱ��ȭ�� ī�޶� ��ü ī��Ʈ
        int enabledCount = 0;
        foreach (var pointer in pointers)
        {
            if (pointer.enabled)
            {
                enabledCount++;
            }
        }

        // Lines�� ���� �������
        // points�� ���� Lines - 1���� Ȯ���Ѵ�
        if (points.Count != enabledCount - 1)
        {
            // lines - 1���� points�� ���� ������
            if (points.Count < enabledCount - 1)
            {
                // points�� �߰��Ѵ�.
                int diff = (enabledCount - 1) - points.Count;

                for (int i = 0; i < diff; i++)
                {
                    points.Add(new Vector3());
                }
            }

            // lines - 1���� points�� ���� Ŭ��
            else if (points.Count > enabledCount - 1)
            {
                // points�� ���̸�ŭ �����Ѵ�.
                int diff = points.Count - (enabledCount - 1);

                points.RemoveRange(0, diff);
            }
        }

        // ����Ʈ ������ŭ ��ġ���� ����
        for (int i = 0; i < 33; i++)
        {
            Vector3 sum = Vector3.zero;
            int count = 0;

            try
            {
                // �� ��(j, k)�� ��ȸ�ϸ鼭 �� ���� ����(__midpoint)�� points�� �Ҵ��Ѵ�.
                for (int j = 0; j < pointers.Count; j++)
                {
                    if (!pointers[j].enabled) { continue; }

                    for (int k = j + 1; k < pointers.Count; k++)
                    {
                        if (!pointers[k].enabled) { continue; }

                        var _closestPoints = ClosestPointsOnTwoLines(pointers[j], pointers[k], i);
                        sum += _closestPoints.Item1;
                        sum += _closestPoints.Item2;
                        count += 2;

                        Vector3 __midPoint = (_closestPoints.Item1 + _closestPoints.Item2) / 2;
                        points[k - 1] = __midPoint;
                    }
                }
            }
            catch (System.Exception)
            {
                // TODO: ������ ���� ��ķ �ε����� ���� ������ �õ��ϴٰ� ������ �߻��ϴ� ������ ����
            }
            

            // ���� ������ ���Ѵ�.
            Vector3 _midPoint = sum / count;
            riggingPoints[i].position = _midPoint;
        }
    }

    public static (Vector3, Vector3) ClosestPointsOnTwoLines(AnimationCodeMain line1, AnimationCodeMain line2, int destIndex)
    {
        Vector3 u = line1.dest_points[destIndex] - line1.originCamera.position;
        Vector3 v = line2.dest_points[destIndex] - line2.originCamera.position;
        Vector3 w = line1.originCamera.position - line2.originCamera.position;

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
        Vector3 pointOnLine1 = line1.originCamera.position + sc * u;
        // Closest point on line2 to line1
        Vector3 pointOnLine2 = line2.originCamera.position + tc * v;

        return (pointOnLine1, pointOnLine2);
    }
}

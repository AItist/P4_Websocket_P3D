using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ExampleTwoLines_NearPoint : MonoBehaviour
{
    [SerializeField]
    private bool debug_points;
    private bool debugCurrent_points;

    [SerializeField]
    private bool debug_midpoint;
    private bool debugCurrent_midpoint;

    private void ToggleRender_points(bool debug)
    {
        foreach (Transform t in points)
        {
            if (t.GetComponent<MeshRenderer>() != null)
            {
                t.GetComponent<MeshRenderer>().enabled = debug;
            }
        }
    }

    private void ToggleRender_midpoint(bool debug)
    {
        if (midPoint != null)
        {
            midPoint.GetComponent<MeshRenderer>().enabled = debug;
        }
    }

    public List<LineCode> lineCodes = new List<LineCode>();

    //public List<Line3D> Lines;
    public List<Transform> points;
    public Transform midPoint;
    
    public static (Vector3, Vector3) ClosestPointsOnTwoLines(LineCode line1, LineCode line2)
    {
        Vector3 u = line1.destination.position - line1.origin.position;
        Vector3 v = line2.destination.position - line2.origin.position;
        Vector3 w = line1.origin.position - line2.origin.position;

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
        Vector3 pointOnLine1 = line1.origin.position + sc * u;
        // Closest point on line2 to line1
        Vector3 pointOnLine2 = line2.origin.position + tc * v;

        return (pointOnLine1, pointOnLine2);
    }

    GameObject AddPoint(int index)
    {
        // Create a new sphere primitive
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = $"point {index} (blue)";
        sphere.transform.localScale = Vector3.one * 0.2f;
        sphere.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.blue);

        return sphere;
    }

    void Update()
    {
        if (debugCurrent_points != debug_points)
        {
            debugCurrent_points = debug_points;
            ToggleRender_points(debug_points);
        }

        if (debugCurrent_midpoint != debug_midpoint)
        {
            debugCurrent_midpoint = debug_midpoint;
            ToggleRender_midpoint(debug_midpoint);
        }

        // Lines의 수를 기반으로
        // points의 수가 Lines - 1인지 확인한다.
        if (points.Count != lineCodes.Count - 1)
        {
            // lines - 1보다 points의 수가 작을때
            if (points.Count < lineCodes.Count - 1)
            {
                // points를 추가한다.
                int diff = (lineCodes.Count - 1) - points.Count;

                for (int i = 0; i < diff; i++)
                {
                    GameObject obj = AddPoint(points.Count);

                    points.Add(obj.transform);
                }
            }

            // lines - 1보다 points의 수가 클때
            else if (points.Count > lineCodes.Count - 1)
            {
                // points를 차이만큼 제거한다.
                int diff = points.Count - (lineCodes.Count - 1);

                points.RemoveRange(0, diff);
            }
        }

        Vector3 sum = Vector3.zero;
        int count = 0;

        for (int i = 0; i < lineCodes.Count; i++)
        {
            for (int j = i + 1; j < lineCodes.Count; j++)
            {
                var _closestPoints = ClosestPointsOnTwoLines(lineCodes[i], lineCodes[j]);
                sum += _closestPoints.Item1;
                sum += _closestPoints.Item2;
                count += 2;

                Vector3 __midPoint = (_closestPoints.Item1 + _closestPoints.Item2) / 2;
                Debug.Log($"{j} {__midPoint}");
                points[j - 1].transform.position = __midPoint;
            }
        }

        Vector3 _midPoint = sum / count;
        midPoint.position = _midPoint;
        //Debug.Log("Midpoint of Closest Approach: " + _midPoint);
    }
}

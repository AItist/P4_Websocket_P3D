using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Line3D
{
    public Transform P1_transform; public Transform P2_transform;

    
    public Vector3 P1
    {
        get
        {
            return P1_transform.position;
        }
    }

    public Vector3 P2
    {
        get
        {
            return P2_transform.position;
        }
    }
}

public class ExampleTwoLines_NearPoint : MonoBehaviour
{
    public Line3D Line1;
    public Line3D Line2;
    public Transform midPoint;

    public static (Vector3, Vector3) ClosestPointsOnTwoLines(Line3D line1, Line3D line2)
    {
        Vector3 u = line1.P2 - line1.P1;
        Vector3 v = line2.P2 - line2.P1;
        Vector3 w = line1.P1 - line2.P1;

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
        Vector3 pointOnLine1 = line1.P1 + sc * u;
        // Closest point on line2 to line1
        Vector3 pointOnLine2 = line2.P1 + tc * v;

        return (pointOnLine1, pointOnLine2);
    }

    void Update()
    {
        var closestPoints = ClosestPointsOnTwoLines(Line1, Line2);
        Vector3 _midPoint = (closestPoints.Item1 + closestPoints.Item2) / 2;

        midPoint.position = _midPoint;
        //Debug.Log("Midpoint of Closest Approach: " + _midPoint);
    }
}

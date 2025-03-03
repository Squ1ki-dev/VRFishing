using System.Collections.Generic;
using UnityEngine;

public static class BezierCurve
{
    // The resolution of the curve (must add up to 1 to avoid gaps)
    private const float Resolution = 0.1f;

    /// <summary>
    /// Updates the positions of the rope sections using a Bezier curve.
    /// </summary>
    /// <param name="start">Start point (A).</param>
    /// <param name="control1">First control point (B).</param>
    /// <param name="control2">Second control point (C).</param>
    /// <param name="end">End point (D).</param>
    /// <param name="ropeSections">List to store the calculated rope sections.</param>
    public static void GetBezierCurve(Vector3 start, Vector3 control1, Vector3 control2, Vector3 end, List<Vector3> ropeSections)
    {
        if (ropeSections == null)
        {
            Debug.LogError("Rope sections list cannot be null.");
            return;
        }

        // Clear the list to ensure it's empty before adding new points
        ropeSections.Clear();

        // Generate points along the Bezier curve
        for (float t = 0; t <= 1f; t += Resolution)
        {
            Vector3 point = CalculateBezierPoint(start, control1, control2, end, t);
            ropeSections.Add(point);
        }

        // Ensure the end point is included
        ropeSections.Add(end);
    }

    /// <summary>
    /// Calculates a point on the Bezier curve using De Casteljau's algorithm.
    /// </summary>
    /// <param name="start">Start point (A).</param>
    /// <param name="control1">First control point (B).</param>
    /// <param name="control2">Second control point (C).</param>
    /// <param name="end">End point (D).</param>
    /// <param name="t">Interpolation factor (0 to 1).</param>
    /// <returns>The point on the Bezier curve at parameter t.</returns>
    private static Vector3 CalculateBezierPoint(Vector3 start, Vector3 control1, Vector3 control2, Vector3 end, float t)
    {
        float oneMinusT = 1f - t;

        // Layer 1: Interpolate between control points
        Vector3 Q = oneMinusT * start + t * control1;
        Vector3 R = oneMinusT * control1 + t * control2;
        Vector3 S = oneMinusT * control2 + t * end;

        // Layer 2: Interpolate between the results of Layer 1
        Vector3 P = oneMinusT * Q + t * R;
        Vector3 T = oneMinusT * R + t * S;

        // Final interpolated position
        return oneMinusT * P + t * T;
    }
}
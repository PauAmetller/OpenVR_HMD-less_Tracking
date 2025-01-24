using System;
using System.Linq;
using UnityEngine;

public static class CalibrationPointsUtils
{
    /// <summary>
    /// Tolerance for floating-point comparisons. Determines the allowable margin
    /// of error when checking geometric properties like perpendicularity or parallelism.
    /// </summary>
    private const float Tolerance = 0.03f;

    /// <summary>
    /// Checks the consistency between the points
    /// </summary>
    public static bool CheckConsistenceOfCalibrationPoints(Vector3[] points)
    {
        if(points.Length != 5)
            throw new ArgumentException("There must be 5 calibration points.");

        Vector3[] squarePoints = points.Take(4).ToArray();

        // Check the consistency of the rectangular base
        CheckConsistencyOfTheBase(squarePoints);

        // Compute the centroid of the points
        Vector3 centroid = ComputeCentroid(points);

        Vector3 squarePlaneNormal = GetNormalOfPlaneFormedBySquare(squarePoints);

        // Get the fifth point
        Vector3 fifthPoint = points[4];

        // Calculate the vector from the centroid to the fifth point
        Vector3 centroidToFifthPoint = fifthPoint - centroid;

        // Use AreVectorsParalel to check if the vector aligns with the plane normal
        bool isAligned = AreVectorsParalel(centroidToFifthPoint, squarePlaneNormal);

        return isAligned;
    }

    /// <summary>
    /// Checks if the angle between the vectors formed by the known points and the candidate point is approximately 90 degrees.
    /// </summary>
    private static bool CheckConsistencyOfTheBase(Vector3[] points)
    {
        // Check the angles formed by three points (for each pair of vectors)
        for (int i = 0; i < points.Length; i++)
        {
            Vector3 vec1 = points[i] - points[(i + 1) % 4];
            Vector3 vec2 = points[(i + 2) % 4] - points[(i + 1) % 4];

            // If the angle is close to 90 degrees, return true
            if (!AreVectorsPerpendicular(vec1, vec2))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Computes the UP vector (orthogonal to the ground plane) from a set of four points that form a square.
    /// </summary>
    public static Vector3 GetNormalOfPlaneFormedBySquare(Vector3[] squarePoints)
    {
        if (squarePoints.Length != 4)
            throw new ArgumentException("The squarePoints array must contain exactly 4 points and it has " + squarePoints.Length + " points.");

        // Calculate normals for each triangle
        Vector3[] normals = new Vector3[]
        {
        Vector3.Cross(squarePoints[3] - squarePoints[0], squarePoints[1] - squarePoints[0]).normalized,
        Vector3.Cross(squarePoints[0] - squarePoints[1], squarePoints[2] - squarePoints[1]).normalized,
        Vector3.Cross(squarePoints[1] - squarePoints[2], squarePoints[3] - squarePoints[2]).normalized,
        Vector3.Cross(squarePoints[2] - squarePoints[3], squarePoints[0] - squarePoints[3]).normalized
        };

        // Use the first normal as the reference
        Vector3 referenceNormal = normals[0];

        // Align all normals to the reference
        for (int i = 1; i < normals.Length; i++)
        {
            if (Vector3.Dot(referenceNormal, normals[i]) < 0) // Dot product < 0 means opposite direction
            {
                normals[i] = -normals[i]; // Flip the normal
            }
        }

        Vector3 averageNormal = normals.Aggregate(Vector3.zero, (sum, n) => sum + n).normalized;

        // Ensure the vector points upward (positive Y direction), the vector may not be pointing up always but due to the physical equipment
        // it should'nt be possible to get a negative y for the normal so it solves the orientation between up and down
        if (averageNormal.y < 0)
        {
            averageNormal = -averageNormal;
        }

        return averageNormal;
    }

    /// <summary>
    /// Calculate and check if the vectors are perpendicular (dot product close to zero)
    /// </summary>
    private static bool AreVectorsPerpendicular(Vector3 vec1, Vector3 vec2)
    {
        // Calculate the dot product between the two vectors
        float dotProduct = Vector3.Dot(vec1.normalized, vec2.normalized);

        // Check if the dot product is close to zero, indicating perpendicular vectors (right angle)
        return Mathf.Abs(dotProduct) <= Tolerance;
    }

    /// <summary>
    /// Calculate and check if the vectors are paralel (dot product close to one)
    /// </summary>
    private static bool AreVectorsParalel(Vector3 vec1, Vector3 vec2)
    {
        // Calculate the dot product between the two vectors
        float dotProduct = Vector3.Dot(vec1.normalized, vec2.normalized);

        // Check if the dot product is close to zero, indicating perpendicular vectors (right angle)
        return Mathf.Abs(dotProduct) >= 1 - Tolerance;
    }

    /// <summary>
    /// Checks the consistency between the points
    /// </summary>
    public static Vector3 ComputeCentroid(Vector3[] points)
    {
        var squarePoints = points.Where((val, idx) => idx != 4).ToArray();

        // Compute the centroid of the points
        Vector3 centroid = new Vector3(
            squarePoints.Average(p => p.x),
            squarePoints.Average(p => p.y),
            squarePoints.Average(p => p.z)
        );

        return centroid;
    }
}


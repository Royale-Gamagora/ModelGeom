using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Cylinder : MonoBehaviour
{
    [Header("Cylinder")]
    [SerializeField] private float _radius = 1f;
    [SerializeField] private float _height = 2f;
    [SerializeField, Range(3, 64)] private int _meridians = 16;

    [Header("Truncated")]
    [SerializeField] private bool _isTruncated = true;
    [SerializeField, Range(1f, 360f)] private float _angle = 300f;

    private void Start()
    {
        Mesh mesh = this.CreateCylinder();
        this.GetComponent<MeshFilter>().mesh = mesh;
    }

    private Mesh CreateCylinder()
    {
        Mesh mesh = new();
        List<Vector3> vertices = new();
        List<int> triangles = new();

        float halfHeight = this._height / 2f;
        float maxAngle = this._angle * Mathf.Deg2Rad;

        // Center points for the bottom and top faces
        int bottomCenter = 0;
        vertices.Add(new Vector3(0, -halfHeight, 0));

        int topCenter = 1;
        vertices.Add(new Vector3(0, halfHeight, 0));

        // Create the bottom and top rings
        int[] bottomPoints = new int[this._meridians + 1];
        int[] topPoints = new int[this._meridians + 1];

        for (int i = 0; i <= this._meridians; i++)
        {
            float theta = maxAngle * ((float)i / this._meridians);
            float x = Mathf.Cos(theta) * _radius;
            float z = Mathf.Sin(theta) * _radius;

            // Bottom point
            bottomPoints[i] = vertices.Count;
            vertices.Add(new Vector3(x, -halfHeight, z));

            // Top point
            topPoints[i] = vertices.Count;
            vertices.Add(new Vector3(x, halfHeight, z));
        }

        // Build the triangles between each meridian
        for (int i = 0; i < _meridians; i++)
        {
            int b0 = bottomPoints[i];
            int b1 = bottomPoints[i + 1];
            int t0 = topPoints[i];
            int t1 = topPoints[i + 1];

            // Side face
            triangles.Add(b0);
            triangles.Add(t0);
            triangles.Add(b1);

            triangles.Add(b1);
            triangles.Add(t0);
            triangles.Add(t1);

            // Bottom face
            triangles.Add(bottomCenter);
            triangles.Add(b0);
            triangles.Add(b1);

            // Top face
            triangles.Add(topCenter);
            triangles.Add(t1);
            triangles.Add(t0);
        }

        // Close the cylinder if it is truncated
        if (this._isTruncated)
        {
            // First cut face
            int bottomFirst = bottomPoints[0];
            int topFirst = topPoints[0];

            triangles.Add(bottomCenter);
            triangles.Add(topCenter);
            triangles.Add(bottomFirst);

            triangles.Add(topCenter);
            triangles.Add(topFirst);
            triangles.Add(bottomFirst);

            // Second cut face
            int bottomLast = bottomPoints[this._meridians];
            int topLast = topPoints[this._meridians];

            triangles.Add(bottomLast);
            triangles.Add(topLast);
            triangles.Add(bottomCenter);

            triangles.Add(topLast);
            triangles.Add(topCenter);
            triangles.Add(bottomCenter);
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}
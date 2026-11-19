using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Sphere : MonoBehaviour
{
    [Header("Sphere")]
    [SerializeField] private float _radius = 1f;
    [SerializeField, Range(2, 64)] private int _parallels = 16;
    [SerializeField, Range(3, 64)] private int _meridians = 16;

    [Header("Truncated")]
    [SerializeField] private bool _isTruncated = true;
    [SerializeField, Range(1f, 360f)] private float _truncatedAngle = 300f;

    private void Start()
    {
        Mesh mesh = this.CreateSphere();
        this.GetComponent<MeshFilter>().mesh = mesh;
    }

    private Mesh CreateSphere()
    {
        Mesh mesh = new();
        List<Vector3> vertices = new();
        List<int> triangles = new();

        float maxTheta = this._truncatedAngle * Mathf.Deg2Rad;
        int pointsPerParallel = this._meridians + 1;

        // Create all the vertices
        for (int i = 0; i <= this._parallels; i++)
        {
            float phi = Mathf.PI * ((float)i / this._parallels);
            float y = this._radius * Mathf.Cos(phi);
            float ringRadius = this._radius * Mathf.Sin(phi);

            for (int j = 0; j <= this._meridians; j++)
            {
                float theta = maxTheta * ((float)j / this._meridians);
                float x = ringRadius * Mathf.Cos(theta);
                float z = ringRadius * Mathf.Sin(theta);

                vertices.Add(new Vector3(x, y, z));
            }
        }

        // Create the faces between parallels and meridians
        for (int i = 0; i < this._parallels; i++)
        {
            for (int j = 0; j < this._meridians; j++)
            {
                int topLeft = (i * pointsPerParallel) + j;
                int topRight = topLeft + 1;
                int bottomLeft = ((i + 1) * pointsPerParallel) + j;
                int bottomRight = bottomLeft + 1;

                if (i == 0)
                {
                    // North pole faces
                    triangles.Add(topLeft);
                    triangles.Add(bottomRight);
                    triangles.Add(bottomLeft);
                }
                else if (i == this._parallels - 1)
                {
                    // South pole faces
                    triangles.Add(topLeft);
                    triangles.Add(topRight);
                    triangles.Add(bottomLeft);
                }
                else
                {
                    // Middle faces
                    triangles.Add(topLeft);
                    triangles.Add(topRight);
                    triangles.Add(bottomLeft);

                    triangles.Add(topRight);
                    triangles.Add(bottomRight);
                    triangles.Add(bottomLeft);
                }
            }
        }

        // Truncate the sphere
        if (this._isTruncated)
        {
            int center = vertices.Count;
            vertices.Add(Vector3.zero);

            for (int i = 0; i < this._parallels; i++)
            {
                // First cut
                int t0 = i * pointsPerParallel;
                int b0 = (i + 1) * pointsPerParallel;

                triangles.Add(center);
                triangles.Add(t0);
                triangles.Add(b0);

                // Second cut
                int t1 = (i * pointsPerParallel) + this._meridians;
                int b1 = ((i + 1) * pointsPerParallel) + this._meridians;

                triangles.Add(center);
                triangles.Add(b1);
                triangles.Add(t1);
            }
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}
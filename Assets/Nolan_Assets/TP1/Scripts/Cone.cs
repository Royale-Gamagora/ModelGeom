using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Cone : MonoBehaviour
{
    [Header("Cone")]
    [SerializeField] private float _radius = 1f;
    [SerializeField] private float _height = 2f;
    [SerializeField, Range(3, 64)] private int _meridians = 16;

    [Header("Truncated")]
    [SerializeField] private bool _isTruncated = true;
    [SerializeField] private float _truncatedHeight = 1.5f;

    private void Start()
    {
        Mesh mesh = this.CreateCone();
        this.GetComponent<MeshFilter>().mesh = mesh;
    }

    private Mesh CreateCone()
    {
        Mesh mesh = new();
        List<Vector3> vertices = new();
        List<int> triangles = new();

        // Compute the top radius using Thales theorem
        float topRadius = this._radius * (this._height - this._truncatedHeight) / this._height;

        // Bottom center point
        int bottomCenter = vertices.Count;
        vertices.Add(Vector3.zero);

        // Create the bottom ring (and the top ring if the cone is truncated)
        int[] bottomPoints = new int[this._meridians];
        int[] topPoints = this._isTruncated ? new int[this._meridians] : null;

        for (int i = 0; i < this._meridians; i++)
        {
            float theta = 2f * Mathf.PI * ((float)i / this._meridians);
            float cos = Mathf.Cos(theta);
            float sin = Mathf.Sin(theta);

            bottomPoints[i] = vertices.Count;
            vertices.Add(new Vector3(cos * this._radius, 0f, sin * this._radius));

            if (this._isTruncated)
            {
                topPoints[i] = vertices.Count;
                vertices.Add(new Vector3(cos * topRadius, this._truncatedHeight, sin * topRadius));
            }
        }

        // Bottom face
        for (int i = 0; i < this._meridians; i++)
        {
            int next = (i + 1) % this._meridians;

            triangles.Add(bottomCenter);
            triangles.Add(bottomPoints[i]);
            triangles.Add(bottomPoints[next]);
        }

        if (this._isTruncated)
        {
            // Top center point
            int topCenter = vertices.Count;
            vertices.Add(new Vector3(0f, this._truncatedHeight, 0f));

            // Side faces and top face
            for (int i = 0; i < this._meridians; i++)
            {
                int next = (i + 1) % this._meridians;
                int b0 = bottomPoints[i];
                int b1 = bottomPoints[next];
                int t0 = topPoints[i];
                int t1 = topPoints[next];

                // Side face
                triangles.Add(b0);
                triangles.Add(t0);
                triangles.Add(b1);

                triangles.Add(b1);
                triangles.Add(t0);
                triangles.Add(t1);

                // Top face
                triangles.Add(topCenter);
                triangles.Add(t1);
                triangles.Add(t0);
            }
        }
        else
        {
            // Single summit point for a full cone
            int top = vertices.Count;
            vertices.Add(new Vector3(0f, _height, 0f));

            // Side triangles connecting the summit to the base
            for (int i = 0; i < _meridians; i++)
            {
                int next = (i + 1) % _meridians;

                triangles.Add(top);
                triangles.Add(bottomPoints[next]);
                triangles.Add(bottomPoints[i]);
            }
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}
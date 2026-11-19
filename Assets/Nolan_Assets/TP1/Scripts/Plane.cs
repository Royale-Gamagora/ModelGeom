using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Plane : MonoBehaviour
{
    [SerializeField] private Vector3 _center = Vector3.zero;
    [SerializeField] private float _width = 5f;
    [SerializeField] private float _height = 5f;
    [SerializeField, Range(1, 64)] private int _rows = 4;
    [SerializeField, Range(1, 64)] private int _columns = 4;

    private void Start()
    {
        Mesh mesh = this.CreatePlane();
        this.GetComponent<MeshFilter>().mesh = mesh;
    }

    private Mesh CreatePlane()
    {
        Mesh mesh = new();
        List<Vector3> vertices = new();
        List<int> triangles = new();

        float stepX = this._width / this._columns;
        float stepZ = this._height / this._rows;

        // Bottom-left corner so the plane is in the good position
        float startX = this._center.x - (this._width / 2f);
        float startZ = this._center.z - (this._height / 2f);

        // Create all the vertices of the grid
        for (int z = 0; z <= _rows; z++)
        {
            for (int x = 0; x <= _columns; x++)
            {
                float posistionX = startX + (x * stepX);
                float positionZ = startZ + (z * stepZ);

                vertices.Add(new Vector3(posistionX, this._center.y, positionZ));
            }
        }

        // Create the faces
        int pointsPerRow = this._columns + 1;

        for (int z = 0; z < this._rows; z++)
        {
            for (int x = 0; x < this._columns; x++)
            {
                int bottomLeft = (z * pointsPerRow) + x;
                int bottomRight = bottomLeft + 1;
                int topLeft = ((z + 1) * pointsPerRow) + x;
                int topRight = topLeft + 1;

                // First triangle
                triangles.Add(bottomLeft);
                triangles.Add(topLeft);
                triangles.Add(bottomRight);

                // Second triangle
                triangles.Add(bottomRight);
                triangles.Add(topLeft);
                triangles.Add(topRight);
            }
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}
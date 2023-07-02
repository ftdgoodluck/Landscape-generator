using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FieldMesh : MonoBehaviour
{

    private Mesh _fieldMesh;
    private MeshCollider _meshCollider;

    [NonSerialized] private List<Vector3> _vertices;
    [NonSerialized] private List<int> _triangles;
    [NonSerialized] private List<Color> _colors;

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = _fieldMesh = new Mesh();
        _fieldMesh.name = "Field Mesh";
        _vertices = new List<Vector3>();
        _triangles = new List<int>();
    }

    public void Clear()
    {
        _fieldMesh.Clear();
        _vertices.Clear();
        _triangles.Clear();
    }

    public void Apply()
    {
        _fieldMesh.SetVertices(_vertices);
        _fieldMesh.SetTriangles(_triangles, 0);
        _fieldMesh.RecalculateNormals();
    }

    public void AddVertice(Vector3 point)
    {
        _vertices.Add(point);
    }

    public void AddTriangle(int point1, int point2, int point3)
    {
        _triangles.Add(point1);
        _triangles.Add(point2);
        _triangles.Add(point3);
    }


    public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = _vertices.Count;
        _vertices.Add(v1);
        _vertices.Add(v2);
        _vertices.Add(v3);
        _triangles.Add(vertexIndex);
        _triangles.Add(vertexIndex + 1);
        _triangles.Add(vertexIndex + 2);
    }
}

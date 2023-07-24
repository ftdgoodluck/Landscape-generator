using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FieldMesh : MonoBehaviour
{
    private Mesh _fieldMesh;
    private MeshCollider _meshCollider;
    [NonSerialized] public List<Color> texturetypes;
    [NonSerialized] public List<Vector3> texturetypes2;
    [NonSerialized] public List<Vector3> texturetypes3;
    [NonSerialized] private List<Vector3> _vertices;
    [NonSerialized] private List<int> _triangles;
    [NonSerialized] private List<Vector2> _uvs;
    [NonSerialized] private Color[] _colors;


    private void Awake()
    {

        GetComponent<MeshFilter>().mesh = _fieldMesh = new Mesh();
        _fieldMesh.name = "Field Mesh";
        _vertices = new List<Vector3>();
        _triangles = new List<int>();
        _uvs = new List<Vector2>();
        
        texturetypes = new List<Color>();
        texturetypes2 = new List<Vector3>();
        texturetypes3 = new List<Vector3>();
        

    }

    public void Clear()
    {
        _fieldMesh.Clear();
        _vertices.Clear();
        _triangles.Clear();
        _uvs.Clear();
    }

    public void Apply()
    {
        _fieldMesh.SetVertices(_vertices);
        _fieldMesh.SetColors(_colors);
        
        _fieldMesh.SetUVs(0, texturetypes2);

        _fieldMesh.SetUVs(1, texturetypes3);
        //_fieldMesh.SetUVs(0, _uvs);

        _fieldMesh.SetTriangles(_triangles, 0);
        
        //_fieldMesh.SetUVs(1, terrainTypes);


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

    //public void AddUVs(List<Vector2> uvs)
    //{
    //    _uvs = uvs;
    //}


    //public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    //{
    //    int vertexIndex = _vertices.Count;
    //    _vertices.Add(v1);
    //    _vertices.Add(v2);
    //    _vertices.Add(v3);
    //    _triangles.Add(vertexIndex);
    //    _triangles.Add(vertexIndex + 1);
    //    _triangles.Add(vertexIndex + 2);

    //}
    public void SetColors(Color[] colors)
    {
        _colors = colors;
    }   
}

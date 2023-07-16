using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FieldMesh : MonoBehaviour
{

    static Color color1 = new Color(1f, 1f, 1f);
    static Color color2 = new Color(0f, 1f, 0f);
    static Color color3 = new Color(0f, 0f, 1f);
    private Mesh _fieldMesh;
    private MeshCollider _meshCollider;
    [NonSerialized] private List<Color> colors = new List<Color>();
    [NonSerialized] private List<Vector3> _vertices, terrainTypes;
    [NonSerialized] private List<int> _triangles;
    [NonSerialized] private List<Color> _colors;
    [NonSerialized] private List<Vector2> _uvs;
   
    private void Awake()
    {
       
        GetComponent<MeshFilter>().mesh = _fieldMesh = new Mesh();
        _fieldMesh.name = "Field Mesh";
        _vertices = new List<Vector3>();
        _triangles = new List<int>();
         _uvs = new List<Vector2>();

        terrainTypes = new List<Vector3>();
        colors = new List<Color>();

      
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
        _fieldMesh.SetColors(colors);
        _fieldMesh.SetUVs(0, terrainTypes);
        _fieldMesh.SetTriangles(_triangles, 0);
        //_fieldMesh.SetUVs(0, _uvs);
       
        
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

    public void AddUVs(List<Vector2> uvs)
    {
        _uvs = uvs;
    }


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
    public void Addtexture(Vector3 typ)
    {
        
            terrainTypes.Add(typ);
       
    }
    public void Changetexture(int index,Vector3 texturetype)
    {
        terrainTypes[index] = texturetype;
    }
    public void Addcolor(Color col)
    {
        colors.Add(col);

    }
    public void Changecolor(int index, Color col)
    {
        colors[index] = col;
    }
}

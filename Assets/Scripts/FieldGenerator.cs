using mattatz.MeshSmoothingSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Unity.VisualScripting;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.UIElements;
using static FieldGenerator;

public class FieldGenerator : MonoBehaviour
{

    public int VerticalPointsCount = 100;
    public int HorizontalPointsCount = 100;

    private int hexSize = Metrics.hexSize;

    public float triangleSize = 0.2f;
    public float hillHeight = 0.4f;
    public float rockHeight = 1.0f;
    public float swampHeight = -0.2f;
    public float step = 0.1f;
    public int typequantity = 5;
    public List<float> biomsweights = new List<float>();
    public int smoothlevel = 2;

    public int secondsmoothlevel = 2;
    public int needsametostay = 2;
    public FieldMesh fieldMesh;
    private List<HexMapElement> hexMap = new List<HexMapElement>();
    private List<Vertice> vertices = new List<Vertice>();
    private List<Vector2> uvs = new List<Vector2>();

    private Color[] colors;
    private List<Vector3> texturscodes = new List<Vector3>();
    private Dictionary<Vertice, Hex> verticehex = new Dictionary<Vertice, Hex>();
    private List<Hex> hexlist = new List<Hex>();
    [NonSerialized] private List<int> _triangles;
    private Dictionary<int, VertexConnection> connections;
    private void Start()
    {


        texturscodes.Add(new Vector3(1, 0, 0));
        texturscodes.Add(new Vector3(0, 1, 0));
        texturscodes.Add(new Vector3(0, 0, 1));
        _triangles = new List<int>();
        colors = new Color[HorizontalPointsCount * (VerticalPointsCount - 1)];
        fieldMesh.Clear();
        //GenerateHexMap();
        GenerateRandomHexMap();
        GenerateInitialPoints();
        GenerateTriangles();
        //AddUVs();
        connections = CreateConnections();
        UpdateHeights();
        SetGrid();
        SmoothMesh();
        Setheigh();
        NormalizeEdgesHeights();
        NormalizeHeights();
        Paintmesh();
        PopulateMeshVertices();
        PopulateTriangles();
        SetColors();


        fieldMesh.Apply();
    }

    private void GenerateInitialPoints()
    {
        int i = 0;
        for (int z = 0; z < VerticalPointsCount - 1; z++)
        {
            for (int x = 0; x < HorizontalPointsCount; x++)
            {

                Vector3 position;
                position.x = z % 2 == 0 ? x * triangleSize : x * triangleSize + triangleSize / 2;
                position.z = z * triangleSize * 0.86602540378f;
                position.y = 0f;
                vertices.Add(new Vertice(position, x, z));


                colors[z * HorizontalPointsCount + x] = Color.white;
                fieldMesh.texturetypes2.Add(new Vector3(0, 0, 0));
                fieldMesh.texturetypes3.Add(new Vector3(0, 0, 0));



                //uvs.Add(new Vector2((float)x / HorizontalPointsCount, (float)z / VerticalPointsCount));
                //fieldMesh.AddVertice(position);
                i++;

            }
        }
    }
    private void SetGrid()
    {
        foreach (Hex hex in hexlist)
        {
            foreach (Vertice vertice in vertices)
            {
                if (vertice.DiscreteDistance(hex.center) == 6)
                    colors[vertice.z * HorizontalPointsCount + vertice.x] = Color.black;
            }
        }
    }

    private void PopulateMeshVertices()
    {
        foreach (Vertice v in vertices)
        {
            fieldMesh.AddVertice(v.position);
        }
    }
    private void SetColors()
    {
        fieldMesh.SetColors(colors);
    }



    private void GenerateRandomHexMap()
    {
        int vc = VerticalPointsCount / hexSize;
        int hc = HorizontalPointsCount / hexSize;
        for (int z = 0; z < vc; z++)
        {
            for (int x = 0; x < hc; x++)
            {
               
                
                float randomnumber = UnityEngine.Random.Range(0, biomsweights.Sum());
                for (int i= 0; i < biomsweights.Count;i++)
                {
                    if (randomnumber < biomsweights.Take(i + 1).Sum())
                    {
                        var hextype = (HexType)(i);
                        hexMap.Add(new HexMapElement(x, z, hextype, i));
                    }
                }
                

                

            }
        }
    }

    //private void GenerateHexMap()
    //{
    //    hexMap.Add(new HexMapElement(0, 0, HexType.hill));
    //    hexMap.Add(new HexMapElement(0, 1, HexType.hill));
    //    //hexMap.Add(new HexMapElement(0, 2, HexType.plain));
    //    //hexMap.Add(new HexMapElement(0, 3, HexType.plain));
    //    hexMap.Add(new HexMapElement(0, 4, HexType.hill));
    //    //hexMap.Add(new HexMapElement(0, 5, HexType.plain));
    //    hexMap.Add(new HexMapElement(1, 0, HexType.hill));
    //    hexMap.Add(new HexMapElement(1, 1, HexType.hill));
    //    //hexMap.Add(new HexMapElement(1, 2, HexType.rock));
    //    hexMap.Add(new HexMapElement(1, 3, HexType.rock));
    //    hexMap.Add(new HexMapElement(1, 4, HexType.rock));
    //    //hexMap.Add(new HexMapElement(1, 5, HexType.rock));
    //    //hexMap.Add(new HexMapElement(2, 0, HexType.plain));
    //    //hexMap.Add(new HexMapElement(2, 1, HexType.plain));
    //    //hexMap.Add(new HexMapElement(2, 2, HexType.plain));
    //    //hexMap.Add(new HexMapElement(2, 3, HexType.rock));
    //    hexMap.Add(new HexMapElement(2, 4, HexType.swamp));
    //    //hexMap.Add(new HexMapElement(2, 5, HexType.plain));
    //    //hexMap.Add(new HexMapElement(3, 0, HexType.plain));
    //    //hexMap.Add(new HexMapElement(3, 1, HexType.plain));
    //    //hexMap.Add(new HexMapElement(3, 2, HexType.plain));
    //    hexMap.Add(new HexMapElement(3, 3, HexType.swamp));
    //    //hexMap.Add(new HexMapElement(3, 4, HexType.plain));
    //    //hexMap.Add(new HexMapElement(3, 5, HexType.plain));
    //    //hexMap.Add(new HexMapElement(4, 0, HexType.plain));
    //    hexMap.Add(new HexMapElement(4, 1, HexType.hill));
    //    //hexMap.Add(new HexMapElement(4, 2, HexType.plain));
    //    //hexMap.Add(new HexMapElement(4, 3, HexType.plain));
    //    //hexMap.Add(new HexMapElement(4, 4, HexType.plain));
    //    //hexMap.Add(new HexMapElement(4, 5, HexType.plain));
    //    //hexMap.Add(new HexMapElement(5, 0, HexType.plain));
    //    //hexMap.Add(new HexMapElement(5, 1, HexType.rock));
    //    //hexMap.Add(new HexMapElement(5, 2, HexType.hill));
    //    //hexMap.Add(new HexMapElement(5, 3, HexType.plain));
    //    //hexMap.Add(new HexMapElement(5, 4, HexType.plain));
    //    //hexMap.Add(new HexMapElement(5, 5, HexType.plain));
    //}


    private void UpdateHeights()
    {

        var hexCenters = vertices.Where(v => v.IsHexCenter());
        foreach (var hexCenter in hexCenters)
        {



            var hexCoords = hexCenter.ToHexCoordinates();
            //Debug.Log("tX: "+ hexCenter.tX.ToString() + " tZ: " + hexCenter.tZ.ToString() + "/ hexX: " + hexCoords.Item1.ToString() + " hexZ: " + hexCoords.Item2.ToString());
            var hex = hexMap.FirstOrDefault(e => e.X == hexCoords.Item1 && e.Z == hexCoords.Item2);

            Hex hexhex = new Hex(hexCenter, hex.hexType);
            verticehex.Add(hexCenter, hexhex);
            //var height = hex.hexType switch

            //{
            //    HexType.plain => 0f,
            //    HexType.hill => hillHeight,
            //    HexType.rock => rockHeight,
            //    HexType.swamp => swampHeight,
            //    _ => throw new ArgumentException("wrong hex type")
            //};
            //vertices.First(v => v.tX == hexCenter.tX && v.tZ == hexCenter.tZ).position.y = height;



            //Paintmesh(hexCenter, hex.textureType,0);


            //Debug.Log(hexCenter.ToString());

            for (int d = 1; d < Metrics.hexSize; d++)
            {

                var neighbours = vertices.Where(v => v.DiscreteDistance(hexCenter) == d);
                foreach (var n in neighbours)
                {


                    //float newHeight;
                    //if (hexCenter.position.y == 0f)
                    //    newHeight = 0f;
                    //else
                    //{
                    //    newHeight = hex.hexType switch
                    //    {
                    //        HexType.plain => 0f,
                    //        HexType.hill => HillHeight(hexCenter, n),
                    //        HexType.rock => RockHeight(hexCenter, n),
                    //        HexType.swamp => SwampHeight(hexCenter, n),
                    //        _ => throw new ArgumentException("wrong hex type")
                    //    };


                    //}
                    //n.position.y = newHeight;


                    verticehex.Add(n, hexhex);
                    hexhex.vertcies.Add(n);


                    //Paintmesh(n, hex.textureType);




                }


            }
            //var neighbours2 = vertices.Where(v => v.DiscreteDistance(hexCenter) == 6);
            //foreach (var n in neighbours2)
            //{
            //    if (!verticehex.ContainsKey(n)){
            //        verticehex.Add(n, hexhex);
            //        hexhex.vertcies.Add(n);
            //    }


            //}
            //for edge vertice



            //var neighbours3 = vertices.Where(v => v.DiscreteDistance(hexCenter) == 7);
            //foreach (var n in neighbours3)
            //{

            //    Paintmesh(n, hex.textureType);
            //}

            hexlist.Add(hexhex);


        }


    }


    private void Paintmesh()
    {

        foreach (Hex hex in hexlist)
        {

            Paintmesh(hex.center, hex.typeindex);
            foreach (Vertice vertice in hex.vertcies)
            {
                if (vertice != hex.center)
                    Paintmesh(vertice, hex.typeindex);

            }
        }




    }

    private void Paintmesh(Vertice n, int typeNumber)
    {
        int index = n.x + n.z * HorizontalPointsCount;
        float mainweight = 0.3f;
        float neighbourweight = (1 - mainweight) / connections[index].Connection.Count;
        if (typeNumber <= 2)
        {
            fieldMesh.texturetypes2[index] += texturscodes[typeNumber] * mainweight;
        }
        else
        {
            fieldMesh.texturetypes3[index] += texturscodes[typeNumber - 3] * mainweight;
        }
        foreach (int neighbourindex in connections[index].Connection)
        {
            Hex neighbourhex = verticehex[vertices[neighbourindex]];
            if (neighbourhex.typeindex <= 2)
            {
                fieldMesh.texturetypes2[index] += texturscodes[neighbourhex.typeindex] * neighbourweight;
            }
            else
            {
                fieldMesh.texturetypes3[index] += texturscodes[neighbourhex.typeindex - 3] * neighbourweight;
            }
        }
        //blend with neighbours

        //if (fieldMesh.texturetypes2[index] == new Vector3(0,0,0) && fieldMesh.texturetypes3[index] == new Vector3(0, 0, 0))
        //{
        //    if (typeNumber <= 2)
        //    {
        //        fieldMesh.texturetypes2[index] = texturscodes[typeNumber];
        //    }
        //    else
        //    {
        //        fieldMesh.texturetypes3[index] = texturscodes[typeNumber - 3];
        //    }
        //}
        //else
        //{

        //    fieldMesh.texturetypes2[index] = fieldMesh.texturetypes2[index] / 2;
        //    fieldMesh.texturetypes3[index] = fieldMesh.texturetypes3[index] / 2;
        //    if (typeNumber <= 2)
        //    {
        //        fieldMesh.texturetypes2[index] += texturscodes[typeNumber]/2;
        //    }
        //    else
        //    {
        //        fieldMesh.texturetypes3[index] += texturscodes[typeNumber - 3]/2;
        //    }
        //}
        ////blending in else


    }
    private Dictionary<int, VertexConnection> CreateConnections()
    {
        return VertexConnection.BuildNetwork(_triangles);
    }
    private void SmoothMesh()
    {

        for (int loop = 0; loop < smoothlevel; loop++)
        {
            for (int i = 0; i < vertices.Count; i++)
            {  
                
                SmoothMesh(i,true);
               

            }

        }
        while (verticehex.Keys.Count != vertices.Count)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                if (!verticehex.ContainsKey(vertices[i]))
                {
                    SmoothMesh(i,false);
                }
            }
        }
        AfterSmooth();




    }
    private void SmoothMesh(int i, bool random)
    {
        
        int a = 0;
        Startoffor:
        int connectTo = UnityEngine.Random.Range(0, connections[i].Connection.Count);

        int distance = verticehex[vertices[connectTo]].center.DiscreteDistance(vertices[i]);
        
       // if (distance > 8 && random)
         //   return;
        if (distance <= 6 || UnityEngine.Random.Range(0, distance-4 ) <3 || !random )
        {
            if (verticehex.ContainsKey(vertices[connections[i].Connection[connectTo]]))
            {

                if (verticehex.ContainsKey(vertices[i]))
                {
                    verticehex[vertices[i]].vertcies.Remove(vertices[i]);


                }

                verticehex[vertices[connections[i].Connection[connectTo]]].vertcies.Add(vertices[i]);
                verticehex[vertices[i]] = verticehex[vertices[connections[i].Connection[connectTo]]];
            }
            else if (a < 5)
            {
                a++;
                goto Startoffor;
            }
        }
    }
    private void AfterSmooth()
    {
        for (int loop = 0; loop < secondsmoothlevel; loop++)
        {
            for (int i = 0; i < vertices.Count; i++)
            {

                List<HexType> hexlist = new List<HexType>();
                Dictionary<HexType, int> coincidence = new Dictionary<HexType, int>();
                for (int a = 0; a < connections[i].Connection.Count; a++)
                {
                    HexType hextype = verticehex[vertices[connections[i].Connection[a]]].type;
                    if (!hexlist.Contains(hextype))
                    {
                        hexlist.Add(hextype);
                        coincidence.Add(hextype, 1);
                    }
                    else coincidence[hextype]++;
                }
                foreach (HexType hex in coincidence.Keys)
                {
                    if (!coincidence.ContainsKey(verticehex[vertices[i]].type) || coincidence[verticehex[vertices[i]].type] <= needsametostay)
                    {
                        HexType supremacytype = HexType.plain;
                        int supremacyquantity = 0;
                        foreach (HexType typ in coincidence.Keys)
                        {
                            if (coincidence[typ] > supremacyquantity)
                            {
                                supremacytype = typ;
                                supremacyquantity = coincidence[typ];
                            }

                        }
                        for (int a = 0; a < connections[i].Connection.Count; a++)
                        {
                            if (verticehex[vertices[connections[i].Connection[a]]].type == supremacytype)
                            {

                                verticehex[vertices[i]].vertcies.Remove(vertices[i]);
                                verticehex[vertices[i]] = verticehex[vertices[connections[i].Connection[a]]];
                                verticehex[vertices[connections[i].Connection[a]]].vertcies.Add(vertices[i]);
                                break;
                            }
                        }

                    }
                }
            }
        }
        }
    private void Setheigh()
    {
       
        foreach (Hex hex in hexlist)
        {
            var height = hex.type switch

            {
                HexType.plain => 0f,
                HexType.hill => hillHeight,
                HexType.rock => rockHeight,
                HexType.swamp => swampHeight,
                HexType.forest => 0f,
                _ => throw new ArgumentException("wrong hex type")
            };
            vertices.First(v => v.tX == hex.center.tX && v.tZ == hex.center.tZ).position.y = height;
            foreach (Vertice n in hex.vertcies)
            {
                float newHeight;
                if (hex.center.position.y == 0f)
                    newHeight = 0f;
                else
                {
                    newHeight = hex.type switch
                    {
                        HexType.plain => 0f,
                        HexType.hill => HillHeight(hex.center, n),
                        HexType.rock => RockHeight(hex.center, n),
                        HexType.swamp => SwampHeight(hex.center, n),
                        HexType.forest =>0f,
                        _ => throw new ArgumentException("wrong hex type")
                    };


                }
                vertices[n.x + n.z * HorizontalPointsCount].position.y = newHeight;

            }
        }

    }
    private float RockHeight(Vertice hexCenter, Vertice target)
    {
        var h = hexCenter.position.y;
        Vector2 start, end;
        start.x = hexCenter.position.x;
        start.y = hexCenter.position.z;
        end.x = target.position.x;
        end.y = target.position.z;
        var relativeDistance = Vector2.Distance(start, end) / (hexSize * triangleSize);
        //Debug.Log(relativeDistance.ToString());
        if (relativeDistance < 0.2f)
            return Mathf.Lerp(h, 0.9f * h, relativeDistance * 5);
        else if (relativeDistance < 0.4f)
            return Mathf.Lerp(0.9f * h, 0.7f * h, relativeDistance * 2 / 5);
        else if (relativeDistance < 0.6f)
            return Mathf.Lerp(0.7f * h, 0.4f * h, relativeDistance * 3 / 5);
        else if (relativeDistance < 0.8f)
            return Mathf.Lerp(0.4f * h, 0.1f * h, relativeDistance * 4 / 5);
        else
            return Mathf.Lerp(0.2f * h, 0, relativeDistance);
    }

    private float HillHeight(Vertice hexCenter, Vertice target)
    {
        Vector2 start, end;
        start.x = hexCenter.position.x;
        start.y = hexCenter.position.z;
        end.x = target.position.x;
        end.y = target.position.z;
        //var relativeDistance = Vector3.Distance(hexCenter.position, target.position) / (hexSize * triangleSize);
        var relativeDistance = Vector2.Distance(start, end) / (hexSize * triangleSize);
        return Mathf.Lerp(hexCenter.position.y, 0, Mathf.Pow(relativeDistance, 2f));
    }

    private float SwampHeight(Vertice hexCenter, Vertice target)
    {
        var h = hexCenter.position.y;
        Vector2 start, end;
        start.x = hexCenter.position.x;
        start.y = hexCenter.position.z;
        end.x = target.position.x;
        end.y = target.position.z;
        var relativeDistance = Vector2.Distance(start, end) / (hexSize * triangleSize);
        if (relativeDistance < 0.8f)
            return h;
        else
            return Mathf.Lerp(h, 0, relativeDistance);
    }

    private void NormalizeHeights()
    {
        foreach (Vertice v in vertices)
        {
            var chance = UnityEngine.Random.Range(0f, 1f);
            if (chance > 0.7f)
                continue;
            else
            {
                var averageNeighboursHeight = vertices.Where(e => e.DiscreteDistance(v) == 1).Select(e => e.position.y).Average();
                v.position.y = averageNeighboursHeight;
            }
        }
    }

    private void NormalizeEdgesHeights()
    {
        var hexCenters = vertices.Where(v => v.IsHexCenter());
        foreach (Vertice hexCenter in hexCenters)
        {
            var edgeVertices = vertices.Where(v => v.DiscreteDistance(hexCenter) == hexSize);
            foreach (Vertice v in edgeVertices)
            {
                var averageNeighboursHeight = vertices.Where(e => e.DiscreteDistance(v) == 1).Select(e => e.position.y).Average();
                v.position.y = averageNeighboursHeight;
            }


        }


    }
    private void AddTriangle(int point1, int point2, int point3)
    {
        _triangles.Add(point1);
        _triangles.Add(point2);
        _triangles.Add(point3);
    }

    private void GenerateTriangles()
    {
        for (int z = 0; z < (VerticalPointsCount -1) / 2; z++)
        {
            for (int x = 0; x < HorizontalPointsCount -1; x++)
            {


                AddTriangle(
                    z * 2 * HorizontalPointsCount + x,
                    z * 2 * HorizontalPointsCount + x + HorizontalPointsCount,
                    z * 2 * HorizontalPointsCount + x + 1);
               AddTriangle(
                    z * 2 * HorizontalPointsCount + x + HorizontalPointsCount,
                    z * 2 * HorizontalPointsCount + x + HorizontalPointsCount + 1,
                    z * 2 * HorizontalPointsCount + x + 1);
                AddTriangle(
                    z * 2 * HorizontalPointsCount + x + HorizontalPointsCount,
                    z * 2 * HorizontalPointsCount + x + HorizontalPointsCount * 2,
                    z * 2 * HorizontalPointsCount + x + HorizontalPointsCount * 2 + 1);
              AddTriangle(
                    z * 2 * HorizontalPointsCount + x + HorizontalPointsCount,
                    z * 2 * HorizontalPointsCount + x + HorizontalPointsCount * 2 + 1,
                    z * 2 * HorizontalPointsCount + x + HorizontalPointsCount + 1);
            }
        }
    }
    private void PopulateTriangles()
    {
        for (int i = 0; i < _triangles.Count; i += 3)
        {
            fieldMesh.AddTriangle(_triangles[i], _triangles[i + 1], _triangles[i + 2]);

        }
    }
}
public class Hex
{
    public Vertice center;
    public HexType type;
    public List<Vertice> vertcies = new List<Vertice>();
    public int typeindex;
    public Hex(Vertice hexcenter, HexType hextype)
    {
        center = hexcenter;
        type = hextype;

        typeindex = Convert.ToInt16(type);
        
    }
   
}
public struct HexMapElement
{
    public int X;
    public int Z;
    public HexType hexType;
    public int textureType;

    public HexMapElement(int x, int z, HexType type, int typeIndex)
    {
        X = x;
        Z = z;
        hexType = type;
        textureType = typeIndex;

    }

    public override string ToString()
    {
        return "X " + X.ToString() + "   Z " + Z.ToString();
    }
}

public static class Metrics
{
    public const int hexSize = 6;
}

public class Vertice
{
    //private const int initialOffset = 

    public Vector3 position;
    public int x;
    public int z;

    public int tX => x - z / 2;
    public int tZ => z;
    public int tY => -tX - tZ;


    public Vertice(Vector3 pos, int X, int Z)
    {
        position = pos;
        x = X;
        z = Z;
    }

    public (int, int) ToHexCoordinates()
    {
        var hexZ = (tZ / Metrics.hexSize) / 2;
        var hexX = (((tX + tZ / 2) / Metrics.hexSize) * 2 + 2) / 3;
        return (hexX, hexZ);
    }

    public bool IsHexCenter()
    {
        if (tX % Metrics.hexSize == 0)
        {
            if ((tZ - tX) % (Metrics.hexSize * 3) == 0)
                return true;
            else
                return false;
        }
        else
            return false;
    }

    public int DiscreteDistance(Vertice other)
    {
        return (Mathf.Abs(tX - other.tX) + Mathf.Abs(tZ - other.tZ) + Mathf.Abs(tY - other.tY)) / 2;
    }

    public override string ToString()
    {
        string res = "Vertice: x: " + x.ToString() + " z: " + z.ToString();
        return res;
    }


    
    
}


public enum HexType
{
    plain, hill, rock, swamp, forest
}

//public interface IHexHeightsProvider {
//    float Height(Vertice hexCenter, int distanceFromHexCenter);
//}

//public class 

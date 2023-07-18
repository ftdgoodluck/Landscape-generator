using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    

    public FieldMesh fieldMesh;
    private List<HexMapElement> hexMap = new List<HexMapElement>();
    private List<Vertice> vertices = new List<Vertice>();
    private List<Vector2> uvs = new List<Vector2>();
    private List<Vector3> texturelist = new List<Vector3>();
    private Vector3 texturevector = new Vector3();
    private List<Color> colors = new List<Color>();
    

    private void Start()
    {
        colors.Add(Color.red);
        colors.Add(Color.green);
        colors.Add(Color.blue);
        colors.Add(Color.black);
        fieldMesh.Clear();
        //GenerateHexMap();
        GenerateRandomHexMap();
        GenerateInitialPoints();
        AddUVs();
        UpdateHeights();

        NormalizeEdgesHeights();
        NormalizeHeights();
        PopulateMeshVertices();
        Addtextures();
        GenerateTriangles();

        fieldMesh.Apply();
    }

    private void GenerateInitialPoints()
    {
        int i = 0;
        for (int z = 0; z < VerticalPointsCount; z++)
        {
            for (int x = 0; x < HorizontalPointsCount; x++)
            {
                Vector3 position;
                position.x = z % 2 == 0 ? x * triangleSize : x * triangleSize + triangleSize / 2;
                position.z = z * triangleSize * 0.86602540378f;
                position.y = 0f;
                vertices.Add(new Vertice(position, x, z));
                //texturelist.Add(new Vector3(4, 4, 4));

                fieldMesh.Addcolor(colors[0]);
               




                uvs.Add(new Vector2((float)x / HorizontalPointsCount, (float)z / VerticalPointsCount));
                //fieldMesh.AddVertice(position);
                i++;

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
    private void Addtextures()
    {
        foreach (Vector3 text in texturelist)
            fieldMesh.Addtexture(text);
    }

    private void AddUVs()
    {
        fieldMesh.AddUVs(uvs);
    }

    private void GenerateRandomHexMap()
    {
        int vc = VerticalPointsCount / hexSize;
        int hc = HorizontalPointsCount / hexSize;
        for (int z = 0; z < vc; z++)
        {
            for (int x = 0; x < hc; x++)
            {
                int randomnumber = UnityEngine.Random.Range(0, 4);
                var hextype = (HexType)(randomnumber);
                hexMap.Add(new HexMapElement(x, z, hextype, randomnumber));
                

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

            var height = hex.hexType switch

            {
                HexType.plain => 0f,
                HexType.hill => hillHeight,
                HexType.rock => rockHeight,
                HexType.swamp => swampHeight,
                _ => throw new ArgumentException("wrong hex type")
            };
            vertices.First(v => v.tX == hexCenter.tX && v.tZ == hexCenter.tZ).position.y = height;

            int typeNumber = hex.textureType;
            int index = vertices.IndexOf(hexCenter);
            texturevector.x = texturevector.z = texturevector.y = typeNumber;
            texturelist[index] = texturevector;
            fieldMesh.Changecolor(index, colors[typeNumber]);
         

            //Debug.Log(hexCenter.ToString());

            for (int d = 1; d < Metrics.hexSize; d++)
            {

                var neighbours = vertices.Where(v => v.DiscreteDistance(hexCenter) == d);
                foreach (var n in neighbours)
                {

                    float newHeight;
                    if (hexCenter.position.y == 0f)
                        newHeight = 0f;
                    else
                    {
                        newHeight = hex.hexType switch
                        {
                            HexType.plain => 0f,
                            HexType.hill => HillHeight(hexCenter, n),
                            HexType.rock => RockHeight(hexCenter, n),
                            HexType.swamp => SwampHeight(hexCenter, n),
                            _ => throw new ArgumentException("wrong hex type")
                        };


                    }
                    n.position.y = newHeight;


                    typeNumber = hex.textureType;
                    index = vertices.IndexOf(n);
                    texturevector.x = texturevector.z = texturevector.y = typeNumber;
                    texturelist[index] = texturevector;
                    fieldMesh.Changecolor(index, colors[typeNumber]);




                }
                

            }
            var neighbours2 = vertices.Where(v => v.DiscreteDistance(hexCenter) == 6);
            foreach (var n in neighbours2)
            {

                typeNumber = hex.textureType;
                index = vertices.IndexOf(n);  
                texturevector.x = texturevector.z = texturevector.y = typeNumber;   
                texturelist[index] = texturevector;
                fieldMesh.Changecolor(index, colors[typeNumber]);


            }
            //for edge vertice


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

    private void GenerateTriangles()
    {
        for (int z = 0; z < (VerticalPointsCount - 1) / 2; z++)
        {
            for (int x = 0; x < HorizontalPointsCount - 1; x++)
            {


                fieldMesh.AddTriangle(
                    z * 2 * HorizontalPointsCount + x,
                    z * 2 * HorizontalPointsCount + x + HorizontalPointsCount,
                    z * 2 * HorizontalPointsCount + x + 1);
                fieldMesh.AddTriangle(
                    z * 2 * HorizontalPointsCount + x + HorizontalPointsCount,
                    z * 2 * HorizontalPointsCount + x + HorizontalPointsCount + 1,
                    z * 2 * HorizontalPointsCount + x + 1);
                fieldMesh.AddTriangle(
                    z * 2 * HorizontalPointsCount + x + HorizontalPointsCount,
                    z * 2 * HorizontalPointsCount + x + HorizontalPointsCount * 2,
                    z * 2 * HorizontalPointsCount + x + HorizontalPointsCount * 2 + 1);
                fieldMesh.AddTriangle(
                    z * 2 * HorizontalPointsCount + x + HorizontalPointsCount,
                    z * 2 * HorizontalPointsCount + x + HorizontalPointsCount * 2 + 1,
                    z * 2 * HorizontalPointsCount + x + HorizontalPointsCount + 1);
            }
        }
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
    plain, hill, rock, swamp
}

//public interface IHexHeightsProvider {
//    float Height(Vertice hexCenter, int distanceFromHexCenter);
//}

//public class 
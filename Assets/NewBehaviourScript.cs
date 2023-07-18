using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public FieldMesh fieldMesh;
    // Start is called before the first frame update
    void Start()
    {
        fieldMesh.AddVertice(new Vector3(10, 10, 10));
        fieldMesh.AddVertice(new Vector3(10, 10, 11));
        fieldMesh.AddVertice(new Vector3(11, 10, 10));
        fieldMesh.AddTriangle(0,1,2);
        fieldMesh.Addtexture(new Vector3(0, 0, 0));
        fieldMesh.Addtexture(new Vector3(0, 0, 0));
        fieldMesh.Addtexture(new Vector3(2, 2, 2));
        fieldMesh.Addcolor(new Color(1, 0, 0));
        fieldMesh.Addcolor(new Color(0, 1, 0));
        fieldMesh.Addcolor(new Color(0, 1, 0));
        fieldMesh.Apply();
    }

    // Update is called once per frame
   
}

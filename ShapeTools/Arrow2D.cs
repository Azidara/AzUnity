using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Arrow2D : MonoBehaviour
{
    [SerializeField] private float _stemLength = 2;
    [SerializeField] private float _stemWidth = 0.1f;
    [SerializeField] private float _pointLength = 0.5f;
    [SerializeField] private float _pointWidth = 0.5f;
    [SerializeField] private Color _color = Color.white;

    public float stemLength {get{return _stemLength;} set{_stemLength = value; this.MakeArrowMesh();}}
    public float stemWidth {get{return _stemWidth;} set{_stemWidth = value; this.MakeArrowMesh();}}
    public float pointLength {get{return _pointLength;} set{_pointLength = value; this.MakeArrowMesh();}}
    public float pointWidth {get{return _pointWidth;} set{_pointWidth = value; this.MakeArrowMesh();}}
    public Color color {get{return _color;} set{_color = value; this.UpdateColor();}}

    [System.NonSerialized]
    public List<Vector3> vertices;
    [System.NonSerialized]
    public List<int> triangles;

    Mesh mesh;

    void Awake(){
        mesh = new Mesh();
        this.GetComponent<MeshFilter>().mesh = mesh;

        MakeArrowMesh();
        UpdateColor();
    }

    void UpdateColor(){
        Material material = GetComponent<MeshRenderer>().sharedMaterial;
        if (material == null){
            material = new Material(Shader.Find("Standard"));
        }
        material.color = this._color;
        GetComponent<MeshRenderer>().material = material;
    }

    void MakeArrowMesh(){
        vertices = new List<Vector3>();
        triangles = new List<int>();

        Vector3 stemOrigin = Vector3.zero;
        float steamHalfWidth = _stemWidth/2f;

        vertices.Add(stemOrigin+(steamHalfWidth*Vector3.down));
        vertices.Add(stemOrigin+(steamHalfWidth*Vector3.up));
        vertices.Add(vertices[0]+(_stemLength*Vector3.right));
        vertices.Add(vertices[1]+(_stemLength*Vector3.right));

        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(3);

        triangles.Add(3);
        triangles.Add(1);
        triangles.Add(0);

        triangles.Add(0);
        triangles.Add(3);
        triangles.Add(2);

        triangles.Add(2);
        triangles.Add(3);
        triangles.Add(0);

        Vector3 tipOrigin = _stemLength*Vector3.right;
        float tipHalfWidth = _pointWidth/2;

        vertices.Add(tipOrigin+(tipHalfWidth*Vector3.up));
        vertices.Add(tipOrigin+(tipHalfWidth*Vector3.down));
        vertices.Add(tipOrigin+(_pointLength*Vector3.right));

        triangles.Add(4);
        triangles.Add(6);
        triangles.Add(5);

        triangles.Add(5);
        triangles.Add(6);
        triangles.Add(4);

        if (mesh == null){
            mesh = new Mesh(); 
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
    }

    void OnValidate(){
        this.MakeArrowMesh();
        this.UpdateColor();
    }
}

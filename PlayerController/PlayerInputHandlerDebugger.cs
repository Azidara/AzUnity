using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerInputHandlerDebugger : MonoBehaviour
{
    PlayerInputHandler debugtarget;
    GameObject InputCircleGO;
    Arrow2D InputArrow;
    Arrow2D VelocityArrow;
    //Arrow2D Velocity3DHand;

    void Awake(){
        this.debugtarget = this.GetComponent<PlayerInputHandler>(); 
    }

    void Start(){
        InitalizeComponents();
    }

    private void InitalizeComponents(){
        InputCircleGO = new GameObject("Movement Limit");
        SetupMovementCircle(InputCircleGO);
        SetupDebugGO(InputCircleGO);
        InputCircleGO.transform.position += new Vector3(0, 0.002f, 0);
        
        GameObject InputArrowGO = new GameObject("Input");
        InputArrow = InputArrowGO.AddComponent<Arrow2D>();
        InputArrow.color = Color.blue;
        SetupDebugGO(InputArrowGO);
        InputArrowGO.transform.position += new Vector3(0, 0.001f, 0);

        GameObject VelocityArrowGO = new GameObject("Velocity");
        VelocityArrow = VelocityArrowGO.AddComponent<Arrow2D>();
        VelocityArrow.color = Color.green;
        SetupDebugGO(VelocityArrowGO);

        //GameObject Velocity3DGO = new GameObject("Velocity 3D");
    }

    void SetupMovementCircle(GameObject GO){
        MeshRenderer mr = GO.AddComponent<MeshRenderer>();
        mr.sharedMaterial = Resources.Load("CircleShaderMat", typeof(Material)) as Material;

        MeshFilter mf = GO.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        vertices.Add(new Vector3(1,0,1));
        vertices.Add(new Vector3(1,0,-1));
        vertices.Add(new Vector3(-1,0,-1));
        vertices.Add(new Vector3(-1,0,1));

        triangles.Add(0);
        triangles.Add(2);
        triangles.Add(1);

        triangles.Add(1);
        triangles.Add(2);
        triangles.Add(0);

        triangles.Add(0);
        triangles.Add(2);
        triangles.Add(3);

        triangles.Add(3);
        triangles.Add(2);
        triangles.Add(0);

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        Vector2[] uv = new Vector2[4]
        {
            new Vector2(1, 1),
            new Vector2(1, 0),
            new Vector2(0, 0),
            new Vector2(0, 1)
        };
        mesh.uv = uv;

        mf.mesh = mesh;
    }
    void SetupDebugGO(GameObject GO){
        GO.transform.SetParent(this.transform);
        GO.transform.localPosition = new Vector3(0, -(debugtarget.controller.height/2), 0);
        ConfigureMeshRenderer(GO);
    }
    private void ConfigureMeshRenderer(GameObject GO){
        MeshRenderer mr = GO.GetComponent<MeshRenderer>();
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mr.receiveShadows = false;
        mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        mr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
    }

    void Update(){
        float speedMod = 1.0f;
        speedMod *= debugtarget.isSprinting ? debugtarget.sprintModifer : 1.0f;
        speedMod *= debugtarget.isCrouching ? debugtarget.crouchModifer : 1.0f;
        
        
        float inputLimit = debugtarget.MaxHorizontalVelocity * speedMod;
        InputCircleGO.transform.localScale = new Vector3(inputLimit,1,inputLimit) * 2.5f / debugtarget.MaxHorizontalVelocity;


        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        Vector3 movement = debugtarget.movementInput.y * forward + debugtarget.movementInput.x * Vector3.Normalize(Camera.main.transform.right);
        movement *= speedMod;
        InputArrow.target = movement * 2.5f;


        VelocityArrow.target = new Vector3(debugtarget.velocity.x,0,debugtarget.velocity.z) * 2.5f / debugtarget.MaxHorizontalVelocity;
    }
}

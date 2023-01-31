using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRig : MonoBehaviour
{
    // Spherical Coord position
    [SerializeField] private Transform target;
    [SerializeField] private float r;
    [SerializeField] private float φ;
    [SerializeField] private float θ;
    // Camera Boundries
    [SerializeField] public bool surfaceCollision = false;
    [SerializeField] public float minZoom = 1.001f;
    [SerializeField] public float maxZoom = 1000f;
    [SerializeField] float minPolarAngle = 0.001f;
    [SerializeField] float maxPolarAngle = 180-0.001f;

    // User Input Fields
    [SerializeField] private bool useBuiltInInput;
    private bool inputEnabled = true;
    [SerializeField] float verticalSpeed = 0.002f;
    [SerializeField] float horizontalSpeed = 0.005f;
    [SerializeField] float zoomSpeed = 0.01f;
    [SerializeField] bool smoothZoom = false;
    float minSmoothZoomStep = 0.001f;
    [SerializeField] bool logarithmicZoom = false;
    [SerializeField] bool invertScroll = true;

    [HideInInspector]
    public int inspectorTab;

    #region - Built In Input Handling -

    private CameraInputHandler inputHandler;
    [SerializeField] private float smoothZoomTarget = 0;

    private void Awake(){
        if (this.useBuiltInInput){
            inputHandler = new CameraInputHandler();

            inputHandler.Camera.RotateCamera.performed += OnCameraRotate;
            inputHandler.Camera.ZoomCamera.performed += OnCameraZoom;
            inputHandler.Camera.UnlockCursor.performed += OnUnlockCursor;
        }
    }

    private void OnEnable(){
        if (this.useBuiltInInput){
            inputHandler.Enable();
        }
    }
    private void OnDisable(){
        if (this.useBuiltInInput){
            inputHandler.Disable();
        }
    }

    private void OnCameraZoom(InputAction.CallbackContext cc){
        if (inputEnabled){
            float Δ = cc.ReadValue<float>() * zoomSpeed;
            if (invertScroll){
                Δ = Δ * -1;
            }
            if (logarithmicZoom){
                Δ = Δ * 0.15f * r;
            }
            if (smoothZoom){
                smoothZoomTarget += Δ;                            // Update the target distance
                Δ = 0.01f * smoothZoomTarget;                      // Take a step towards target point
                if (Mathf.Abs(Δ) < minSmoothZoomStep){              // Check if step is smaller then the minimum step size
                    if (Δ < 0){Δ = -minSmoothZoomStep;}             // Keep correct directionality (sign)
                    else{Δ = minSmoothZoomStep;}
                }
                if (Mathf.Abs(Δ) > Mathf.Abs(smoothZoomTarget)){  // Check if step is larger then the remaining distance
                    Δ = smoothZoomTarget;
                    smoothZoomTarget = 0;
                }
                else {
                    smoothZoomTarget -= Δ;                            // Remove the amount we are moving from the target
                }
                smoothZoomTarget -= Δ;                            // Remove the amount we are moving from the target
            }
            
            this.zoom(Δ);
        }
    }
    private void OnCameraRotate(InputAction.CallbackContext cc){
        if (inputEnabled){
            Vector2 Δ = cc.ReadValue<Vector2>();
            float Δphi = Δ.x * horizontalSpeed * Mathf.PI * -1;
            float Δtheta = Δ.y * verticalSpeed * Mathf.PI;
            this.rotate(Δphi, Δtheta);
        }
    }

    private void OnUnlockCursor(InputAction.CallbackContext cc){
        this.inputEnabled = !inputEnabled;
    }

    void Update()
    {
        // This is for the built in user input
        if (smoothZoom && smoothZoomTarget != 0){
            float Δ = 0.01f * smoothZoomTarget;                // Take a step towards target point
            if (Mathf.Abs(Δ) < minSmoothZoomStep){              // Check if step is smaller then the minimum step size
                if (Δ < 0){Δ = -minSmoothZoomStep;}             // Keep correct directionality (sign)
                else{Δ = minSmoothZoomStep;}
            }
            if (Mathf.Abs(Δ) > Mathf.Abs(smoothZoomTarget)){  // Check if step is larger then the remaining distance
                Δ = smoothZoomTarget;
                smoothZoomTarget = 0;
            }
            else {
                smoothZoomTarget -= Δ;                            // Remove the amount we are moving from the target
            }
            this.zoom(Δ);
        }
    }

    #endregion

    void Start()
    {
        this.transform.SetParent(target);
        Vector3 initpos = cartesianToSpherical(this.transform.position);
        r = initpos.x;
        φ = initpos.y;
        θ = initpos.z;
        this.transform.LookAt(target);
    }

    public void zoom(float Δ){
        r = Mathf.Clamp(r + Δ, minZoom, maxZoom);
        this.updatePosition();
    }

    public void rotate(Vector2 Δ){
        rotate(Δ.x, Δ.y);
    }
    public void rotate(float φ, float θ){
        this.φ += φ;
        this.θ = Mathf.Clamp(this.θ + θ, minPolarAngle * Mathf.PI/180, maxPolarAngle * Mathf.PI/180);
        this.updatePosition();
    }

    void updatePosition(){
        Vector3 spherepos = sphericalToCartesian(r, φ, θ);
        this.transform.position = target.position + spherepos;
        this.transform.LookAt(target);
    }

    Vector3 cartesianToSpherical(Vector3 cartesian){
        return cartesianToSpherical(cartesian.x, cartesian.y, cartesian.z);
    }
    Vector3 cartesianToSpherical(float x, float y, float z){
        float r = Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2) + Mathf.Pow(z, 2));
        float φ = Mathf.Atan(z/x);
        float θ = Mathf.Acos(y/r);

        return new Vector3 (r, φ, θ);
    }
    Vector3 sphericalToCartesian(Vector3 spherical){
        return sphericalToCartesian(spherical.x, spherical.y, spherical.z);
    }
    Vector3 sphericalToCartesian(float r, float φ, float θ){
        float x = r * Mathf.Sin(θ) * Mathf.Cos(φ);
        float z = r * Mathf.Sin(θ) * Mathf.Sin(φ);
        float y = r * Mathf.Cos(θ);
        return (new Vector3 (x,y,z));
    }
}

[CustomEditor(typeof(CameraRig))]
public class CameraRigEditor : Editor {
    private CameraRig itarget;
    private SerializedObject soTarget;

    #region - CameraRig Properties -
    // Camera Settings
    private SerializedProperty cameratarget;
    private SerializedProperty r;
    private SerializedProperty φ;
    private SerializedProperty θ;
    private SerializedProperty surfaceCollision;
    private SerializedProperty minZoom;
    private SerializedProperty maxZoom;
    private SerializedProperty minPolarAngle;
    private SerializedProperty maxPolarAngle;
    // Built In Input Settings
    private SerializedProperty useBuiltInInput;
    private SerializedProperty verticalSpeed;
    private SerializedProperty horizontalSpeed;
    private SerializedProperty zoomSpeed;
    private SerializedProperty smoothZoom;
    private SerializedProperty logarithmicZoom;
    private SerializedProperty invertScroll;

    // Debug
    private SerializedProperty smoothZoomTarget;

    #endregion

    private void OnEnable() {
        itarget = (CameraRig)target;
        soTarget = new SerializedObject(target);

        cameratarget = soTarget.FindProperty("target");
        r = soTarget.FindProperty("r");
        φ = soTarget.FindProperty("φ");
        θ = soTarget.FindProperty("θ");
        surfaceCollision = soTarget.FindProperty("surfaceCollision");
        minZoom = soTarget.FindProperty("minZoom");
        maxZoom = soTarget.FindProperty("maxZoom");
        minPolarAngle = soTarget.FindProperty("minPolarAngle");
        maxPolarAngle = soTarget.FindProperty("maxPolarAngle");

        useBuiltInInput = soTarget.FindProperty("useBuiltInInput");
        verticalSpeed = soTarget.FindProperty("verticalSpeed");
        horizontalSpeed = soTarget.FindProperty("horizontalSpeed");
        zoomSpeed = soTarget.FindProperty("zoomSpeed");
        smoothZoom = soTarget.FindProperty("smoothZoom");
        logarithmicZoom = soTarget.FindProperty("logarithmicZoom");
        invertScroll = soTarget.FindProperty("invertScroll");

        smoothZoomTarget = soTarget.FindProperty("smoothZoomTarget");
    }
    public override void OnInspectorGUI()
    {
        soTarget.Update();
        EditorGUI.BeginChangeCheck();

        itarget.inspectorTab = GUILayout.Toolbar(itarget.inspectorTab, new string[] {"Camera Settings", "Built In Input Settings"} );
        switch(itarget.inspectorTab){
            case 0:
                EditorGUILayout.PropertyField(cameratarget);
                EditorGUILayout.PropertyField(r);
                EditorGUILayout.PropertyField(φ);
                EditorGUILayout.PropertyField(θ);
                EditorGUILayout.PropertyField(surfaceCollision);
                EditorGUILayout.PropertyField(minZoom);
                EditorGUILayout.PropertyField(maxZoom);
                EditorGUILayout.PropertyField(minPolarAngle);
                EditorGUILayout.PropertyField(maxPolarAngle);
                break;
            case 1:
                EditorGUILayout.PropertyField(useBuiltInInput);
                EditorGUILayout.PropertyField(verticalSpeed);
                EditorGUILayout.PropertyField(horizontalSpeed);
                EditorGUILayout.PropertyField(zoomSpeed);
                EditorGUILayout.PropertyField(smoothZoom);
                EditorGUILayout.PropertyField(logarithmicZoom);
                EditorGUILayout.PropertyField(invertScroll);
                //EditorGUILayout.PropertyField(smoothZoomTarget);
            break;;
        }

        if (EditorGUI.EndChangeCheck()) {
            soTarget.ApplyModifiedProperties();
            //GUI.FocusControl(null);
        }
    }
}

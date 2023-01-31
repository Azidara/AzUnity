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
    float zoomStepSize = 0.01f;
    [SerializeField] bool smoothRotate = false;
    [SerializeField] float rotationStepSize = 0.01f;
    float minSmoothRotateStep = (1f/128f) * (Mathf.PI/180f); // 1/4 degree
    [SerializeField] bool logarithmicZoom = false;
    [SerializeField] bool invertScroll = true;

    [HideInInspector]
    public int inspectorTab;

    #region - Built In Input Handling -

    private CameraInputHandler inputHandler;
    [SerializeField] private Vector3 smoothMovementDelta;

    private void Awake(){
        if (this.useBuiltInInput){
            inputHandler = new CameraInputHandler();

            inputHandler.Camera.RotateCamera.performed += OnCameraRotate;
            inputHandler.Camera.ZoomCamera.performed += OnCameraZoom;
            inputHandler.Camera.UnlockCursor.performed += OnUnlockCursor;
        }
    }
    public void EnableInput(){
        this.OnEnable();
    }

    public void DisableInput(){
        this.OnDisable();
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

    private float FindStepSize(float distance, float baseStepSize, float minStepSize){
        if (distance == 0) {return 0;}                  // If there is no distance to go return
        float mag = Mathf.Abs(distance * baseStepSize); // Take a step towards target point
        if (mag < Mathf.Abs(minStepSize)) { // Check if the step is smaller then the minimum step
            mag = Mathf.Abs(minStepSize);
            if (mag > Mathf.Abs(distance)){ // Check that step is not greater then remaining distance
                Debug.Log("Using Remaining distance");
                mag = Mathf.Abs(distance);
            } 
        }   
        if ( distance < 0 ){ return mag * -1; } // Keep correct direction (sign)          
        else{return mag;}
    }

    private void UpdateZoom(){
        // This is for the built in user input
        if (smoothZoom && smoothMovementDelta.x != 0){
            float Δ = FindStepSize(smoothMovementDelta.x, zoomStepSize, minSmoothZoomStep);               
            smoothMovementDelta.x -= Δ;  
            this.zoom(Δ);
        }
    }

    private void UpdateRotation() {
        if (smoothRotate && (smoothMovementDelta.y != 0 || smoothMovementDelta.z != 0)){
            float Δphi = FindStepSize(smoothMovementDelta.y, rotationStepSize, minSmoothRotateStep);
            float Δtheta = FindStepSize(smoothMovementDelta.z, rotationStepSize, minSmoothRotateStep);
            smoothMovementDelta.y -= Δphi;
            smoothMovementDelta.z -= Δtheta;
            // Debug 
            Debug.Log($"{smoothMovementDelta.y},{smoothMovementDelta.z}\n{Δphi},{Δtheta}");
            this.rotate(Δphi, Δtheta);
        }
    }

    private void OnUnlockCursor(InputAction.CallbackContext cc){
        this.inputEnabled = !inputEnabled;
    }

    private void OnCameraZoom(InputAction.CallbackContext cc){
        if (inputEnabled){
            float Δ = cc.ReadValue<float>() * zoomSpeed;
            if (invertScroll){ Δ = Δ * -1; }
            if (logarithmicZoom){ Δ = Δ * 0.15f * r; }
            if (smoothZoom){
                smoothMovementDelta.x += Δ;
            }
            else {
                this.zoom(Δ);
            }
        }
    }
    private void OnCameraRotate(InputAction.CallbackContext cc){
        if (inputEnabled){
            Vector2 Δ = cc.ReadValue<Vector2>();
            float Δphi = Δ.x * horizontalSpeed * Mathf.PI * -1;
            float Δtheta = Δ.y * verticalSpeed * Mathf.PI;
            if (smoothRotate){
                smoothMovementDelta.y += Δphi;
                smoothMovementDelta.z += Δtheta;
            }
            else {
                this.rotate(Δphi, Δtheta);
            }
        }
    }

    void Update()
    {
        UpdateZoom();
        UpdateRotation();
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
    private SerializedProperty logarithmicZoom;
    private SerializedProperty smoothZoom;
    private SerializedProperty smoothRotate;
    private SerializedProperty invertScroll;

    // Debug
    private SerializedProperty smoothMovementDelta;
    private SerializedProperty rotationStepSize;

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
        logarithmicZoom = soTarget.FindProperty("logarithmicZoom");
        smoothZoom = soTarget.FindProperty("smoothZoom");
        smoothRotate = soTarget.FindProperty("smoothRotate");
        invertScroll = soTarget.FindProperty("invertScroll");

        smoothMovementDelta = soTarget.FindProperty("smoothMovementDelta");
        rotationStepSize = soTarget.FindProperty("rotationStepSize");
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
                EditorGUILayout.PropertyField(logarithmicZoom);
                EditorGUILayout.PropertyField(smoothZoom);
                EditorGUILayout.PropertyField(smoothRotate);
                EditorGUILayout.PropertyField(invertScroll);
                EditorGUILayout.PropertyField(smoothMovementDelta);
                EditorGUILayout.PropertyField(rotationStepSize);
            break;;
        }

        if (EditorGUI.EndChangeCheck()) {
            soTarget.ApplyModifiedProperties();
            //GUI.FocusControl(null);
        }
    }
}

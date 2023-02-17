using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class SimplePlayerController : MonoBehaviour
{
    private PlayerInputActions _inputHandler;
    private CharacterController _CharCtrl;

    #region -- Configurable Settings --
    public float modelHeight = 1f;
    public float crouchSpeed = 2.5f;
    public float defaultSpeed = 5f;
    public float sprintSpeed = 10f;
    public float jumpSpeed = 5f;
    public bool gravityEnabled = true;
    public float gravity = 9.81f;
    public Vector3 respawnPoint;

    #endregion

    #region -- Controller state --
    private Vector2 _moveInput;
    private float _verticalVelocity;
    [SerializeField] private bool _isGrounded = false;
    [SerializeField] private bool _isSprinting = false;
    [SerializeField] private bool _isCrouching = false;
    
    #endregion

    #region -- Exposed Properties --
    public bool isGrounded {get{return _isGrounded;}}
    public bool isSprinting {get{return _isSprinting;}}
    public bool isCrouching {get{return _isCrouching;}}

    #endregion

    private void Awake() {
        this._CharCtrl = this.GetComponent<CharacterController>();
        this._inputHandler = new PlayerInputActions();
        _inputHandler.Player.Move.performed += OnMovement;
        _inputHandler.Player.Jump.performed += OnJump;
        _inputHandler.Player.Sprint.performed += OnSprint;
        _inputHandler.Player.Crouch.performed += OnCrouch;
        _inputHandler.Player.Respawn.performed += OnRespawn;
        _inputHandler.Player.SetRespawn.performed += OnSetRespawn;

        if (respawnPoint == Vector3.zero || respawnPoint == null){
            respawnPoint = this.transform.position;
        }
        testGrounded();
    }

    #region  -- Input Events --
    private void OnEnable(){
        _inputHandler.Enable();
    }

    private void OnDisable(){
        _inputHandler.Disable();
    }

    private void OnMovement(InputAction.CallbackContext cc){
        _moveInput = cc.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext cc){
        this.jump();
    }

    private void OnSprint(InputAction.CallbackContext cc){
        this.sprint();
    }

    private void OnCrouch(InputAction.CallbackContext cc){
        this.crouch();
    }

    private void OnRespawn(InputAction.CallbackContext cc){
        this.jumpto(respawnPoint);
    }

    private void OnSetRespawn(InputAction.CallbackContext cc){
        this.respawnPoint = this.transform.position;
    }

    #endregion 

    #region -- Actions --
    public void jump(){
        if(_isGrounded) {
            _verticalVelocity += jumpSpeed;
            _isGrounded = false;
        }
    }

    public void sprint(){
        _isCrouching = false;
        _isSprinting = !_isSprinting;
    }

    public void crouch(){
        _isSprinting = false;
        _isCrouching = !_isCrouching;
    }

    public void jumpto(Vector3 position){
        _CharCtrl.enabled = false;
        this.transform.position = position;
        _CharCtrl.enabled = true;
        testGrounded();
    }
    
    #endregion

    private bool testGrounded(){
        _isGrounded = Physics.Raycast(this.transform.position, Vector3.down, modelHeight + 0.05f);
        return isGrounded;
    }

    private Vector2 CalculateFrameInput(){
        if (_moveInput == Vector2.zero){
            return _moveInput;
        }
        Vector2 camForward = new Vector2(Camera.main.transform.forward.x, Camera.main.transform.forward.z).normalized;
        Vector2 camRight = new Vector2(Camera.main.transform.right.x, Camera.main.transform.right.z).normalized;

        // Calculate input relative to the camera view;
        Vector2 frameInput = _moveInput.y * camForward + _moveInput.x * camRight;
        Vector2.ClampMagnitude(frameInput, 1f);

        // Apply magnitude modifers;
        float curSpeed = _isSprinting ? sprintSpeed : defaultSpeed;
        curSpeed = _isCrouching ? crouchSpeed : curSpeed;
        frameInput *= curSpeed;

        return frameInput;
    }

    private void Update() {
        // Apply deceleration movements first.
        if (gravityEnabled){
            _verticalVelocity -= gravity * Time.deltaTime;
        }

        if (_isGrounded){
            // Stop downward motion
            _verticalVelocity = Mathf.Clamp(_verticalVelocity, 0f, Mathf.Infinity);
        }

        Vector2 frameInput = CalculateFrameInput();
        Vector3 movement = new Vector3(frameInput.x, _verticalVelocity, frameInput.y) * Time.deltaTime;
        
        _CharCtrl.Move(movement);
        testGrounded();
    }
}

using UnityEngine;
using UnityEngine.InputSystem;
using AzMath;

[RequireComponent(typeof(CharacterController))]
public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInputActions inputHandler;
    public CharacterController controller;

    [SerializeField] private Vector2 _movementInput;
    public Vector2 movementInput {get{return _movementInput;}}
    [SerializeField] public PolarVec3 _velocity;
    public Vector3 velocity {get{return PolarVec3.PolartoCartesian(_velocity);}}

    public float movementSpeed = 1.0f;
    public float sprintModifer = 2.0f;
    public float crouchModifer = 0.5f;
    public float jumpHeight = 5.0f;

    public float velocityDecayRateGrounded = 1.0f;
    public float MaxHorizontalVelocity = 5.0f;

    public Vector3 respawnPoint;

    public bool isSprinting = false;
    public bool isCrouching = false;

    public bool gravityEnabled = true;
    public float gravityValue = -9.81f;
    
    private void Awake() {
        controller = this.GetComponent<CharacterController>();
        inputHandler = new PlayerInputActions();

        _velocity = PolarVec3.zero;
        if (respawnPoint == Vector3.zero){
            respawnPoint = this.transform.position;
        }

        inputHandler.BasicPlayer.Move.performed += OnMovement;
        inputHandler.BasicPlayer.Jump.performed += OnJump;
        inputHandler.BasicPlayer.Sprint.performed += OnSprint;
        inputHandler.BasicPlayer.Crouch.performed += OnCrouch;
        inputHandler.BasicPlayer.Respawn.performed += OnRespawn;
        inputHandler.BasicPlayer.SetRespawn.performed += OnSetRespawn;
    }

    #region - Inputs -

    private void OnEnable(){
        inputHandler.Enable();
    }

    private void OnDisable(){
        inputHandler.Disable();
    }

    private void OnMovement(InputAction.CallbackContext cc){
        _movementInput = cc.ReadValue<Vector2>();
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

    #region - Actions -
    public void jumpto(Vector3 position){
        this.transform.position = position;
    }

    public void jump(){
        if(controller.isGrounded) {
            this._velocity.h += jumpHeight;
        }
    }

    public void sprint(){
        isCrouching = false;
        isSprinting = !isSprinting;
    }

    public void crouch(){
        isSprinting = false;
        isCrouching = !isCrouching;
    }
    #endregion

    private PolarVec3 horizontalMovement(){
        // Update Horizontal movement
        if (_movementInput != Vector2.zero){
            Vector3 forward = Camera.main.transform.forward;
            forward.y = 0;
            forward = Vector3.Normalize(forward);

            Vector3 movement = _movementInput.y * forward + _movementInput.x * Vector3.Normalize(Camera.main.transform.right);

            return PolarVec3.CartesianToPolar(movement);
        }

        return PolarVec3.zero;
    }

    void Update() {
        // Find Vertical movement
        if (gravityEnabled){
            _velocity.h += gravityValue * Time.deltaTime;
        }
        PolarVec3 userinput;
        if (controller.isGrounded){
            // Velocity decay on ground
            if (_velocity.r < 1 && _velocity.r > -1){
                _velocity.r = 0;
            }
            else {
                _velocity.r -= velocityDecayRateGrounded * Time.deltaTime;
            }
            

            userinput = horizontalMovement();
            userinput *= movementSpeed;
            // Add user input modifiers
            float speedMod = 1.0f;
            speedMod *= isSprinting ? sprintModifer : 1.0f;
            speedMod *= isCrouching ? crouchModifer : 1.0f;
            userinput.r = userinput.r * speedMod;

            float rmax = Mathf.Max(_velocity.r, MaxHorizontalVelocity * speedMod);
            // Add user input to current _velocity
            _velocity += userinput;

            // Make sure the userinput doesn't increase the speed past the max allowed
            _velocity.r = Mathf.Clamp(_velocity.r, Mathf.NegativeInfinity, rmax);
            
            // Stop any downward motion as we are grounded.
            _velocity.h = Mathf.Clamp(_velocity.h, 0f, Mathf.Infinity);
        }
        else {
            userinput = horizontalMovement();
            userinput *= movementSpeed;
            _velocity += userinput * Time.deltaTime;
        }

        controller.Move(PolarVec3.PolartoCartesian(_velocity) * Time.deltaTime);
    }
}

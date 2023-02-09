using UnityEngine;
using UnityEngine.InputSystem;
using AzMath;

public class BasicPlayerController : MonoBehaviour
{
    Vector2 movementInput;

    [SerializeField] PolarVec3 velocity;

    public float jumpHeight = 5.0f;
    public float movementSpeed = 1.0f;
    public float sprintModifer = 2.0f;
    public float crouchModifer = 0.5f;

    public float velocityDecayRateGrounded = 1.0f;
    public float MaxHorizontalVelocity = 5.0f;
    public float MaxVerticalVelocity = 15.0f;

    public float modelCentroidHeight = 0.5f;

    public bool isSprinting = false;
    public bool isCrouching = false;
    public bool isGrounded = false;
    public bool gravityEnabled = true;
    public float gravityValue = -9.81f;

    #region - Input Handling -
    private BasicPlayerInputHandler inputHandler;
    public Vector3 respawnPoint;

    int Debugcounter = 0;

    private void Awake() {
        velocity = PolarVec3.zero;

        if (respawnPoint == Vector3.zero){
            respawnPoint = this.transform.position;
        }
        inputHandler = new BasicPlayerInputHandler();

        inputHandler.BasicPlayer.Move.performed += OnMovement;
        inputHandler.BasicPlayer.Jump.performed += OnJump;
        inputHandler.BasicPlayer.Sprint.performed += OnSprint;
        inputHandler.BasicPlayer.Crouch.performed += OnCrouch;
        inputHandler.BasicPlayer.Respawn.performed += OnRespawn;
        inputHandler.BasicPlayer.SetRespawn.performed += OnSetRespawn;
    }

    private void OnEnable(){
        inputHandler.Enable();
    }

    private void OnDisable(){
        inputHandler.Disable();
    }

    private void OnMovement(InputAction.CallbackContext cc){
        movementInput = cc.ReadValue<Vector2>();
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

    public void jump(){
        if(isGrounded) {
            velocity.h += jumpHeight;
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

    public void move(Vector3 delta){
        // Add check for collisoins, step height and slope
        this.transform.position += delta;
        // Grounded state is updated after movement
        testGrounded();
    }

    public void jumpto(Vector3 position){
        this.transform.position = position;
        // Grounded state is updated after movement
        testGrounded();
    }

    private bool testGrounded(){
        isGrounded = Physics.Raycast(this.transform.position, Vector3.down, modelCentroidHeight + 0.05f);
        return isGrounded;
    }

    private PolarVec3 horizontalMovement(){
        // Update Horizontal movement
        if (movementInput != Vector2.zero){
            Vector3 forward = Camera.main.transform.forward;
            forward.y = 0;
            forward = Vector3.Normalize(forward);

            Vector3 movement = movementInput.y * forward + movementInput.x * Vector3.Normalize(Camera.main.transform.right);

            return PolarVec3.CartesianToPolar(movement);
        }

        return PolarVec3.zero;
    }

    void Update() {
        // Find Vertical movement
        if (gravityEnabled){
            velocity.h += gravityValue * Time.deltaTime;
        }
        PolarVec3 userinput;
        if (isGrounded){
            // Velocity decay on ground
            if (velocity.r < 1 && velocity.r > -1){
                velocity.r = 0;
            }
            else {
                velocity.r -= velocityDecayRateGrounded * Time.deltaTime;
            }
            

            userinput = horizontalMovement();
            userinput *= movementSpeed;
            // Add user input modifiers
            float speedMod = 1.0f;
            speedMod *= isSprinting ? sprintModifer : 1.0f;
            speedMod *= isCrouching ? crouchModifer : 1.0f;
            userinput.r = userinput.r * speedMod;

            float rmax = Mathf.Max(velocity.r, MaxHorizontalVelocity * speedMod);
            // Add user input to current velocity
            velocity += userinput;

            // Make sure the userinput doesn't increase the speed past the max allowed
            velocity.r = Mathf.Clamp(velocity.r, Mathf.NegativeInfinity, rmax);
            
            // Stop any downward motion as we are grounded.
            velocity.h = Mathf.Clamp(velocity.h, 0f, Mathf.Infinity);
        }
        else {
            userinput = horizontalMovement();
            userinput *= movementSpeed;
            velocity += userinput * Time.deltaTime;
        }

        this.move(PolarVec3.PolartoCartesian(velocity) * Time.deltaTime);

        if (Debugcounter == 0){
            Debug.Log($"Frame Debug\n" +
                      $"Time : {Time.deltaTime}\n" +
                      $"User Input : {PolarVec3.PolartoCartesian(userinput)}\n" +
                      $"Velocity : {PolarVec3.PolartoCartesian(velocity)}");
        }
        Debugcounter = (Debugcounter + 1) % 60;
    }


    // Model & Gameobject fields
    //[SerializeField] GameObject body;
    //[SerializeField] GameObject head;
    //[SerializeField] Transform center;

    // State Monitors
    //[SerializeField] Vector3 _look;

    // fields to play around with
    //public int airjumpsallowed = 1;

    // Enable fields
    //public bool enable_controller;
    //public bool enable_movement;
    //public bool enable_jump;
    //public bool enable_sprint;
    //public bool etc;

    // Events
    //public Event movementstarted;
    //public Event movementended;
    //public Event leftground;
    //public Event touchedground;

    // Abilities
    
    //void jump(float x){}

    /*
    void climb_edge(){}
    void grapple(){}
    void airjump(){}
    void toggle_fly(){}
    void toggle_noclip(){}

    // Should these be included?
    void reset_movement(){}

    void OnTriggerEnter(Collider other){}
    */
}

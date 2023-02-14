using UnityEngine;
using AzMath;

public class BasicPlayerController : MonoBehaviour
{
    public float modelCentroidHeight = 0.5f;
    public bool isGrounded = false;
    
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

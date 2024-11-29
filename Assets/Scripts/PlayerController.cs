using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public Vector2 speed;
    public float acceleration = 0.1f;
    public float maxSpeed = 5f;
    public Vector2 playerInput;
    public FacingDirection direction = FacingDirection.right;
    public groundDetector groundHitBox;
    public float apexHeight;
    public float apexTime;
    float gravity;
    public float terminalVeloicty;
    public float coyoteTime;
    float timeSinceGrounded;

    public enum FacingDirection
    {
        left, right
    }

    // Start is called before the first frame update
    void Start()
    {
        speed = new Vector2(0,0);
    }

    // Update is called once per frame
    void Update()
    {
        playerInput.x = 0;
        //The input from the player needs to be determined and then passed in the to the MovementUpdate which should
        //manage the actual movement of the character.
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
            playerInput.x = -1;
        }
        else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
            playerInput.x  = 1;
        }
        if(playerInput.y == 0 && (timeSinceGrounded < coyoteTime) && (Input.GetKeyDown(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))){
            playerInput.y = 1;
        }

        if(IsGrounded()){
            timeSinceGrounded = 0;
        }
        else{
            timeSinceGrounded += Time.deltaTime;
        }
    }

    void FixedUpdate(){
        MovementUpdate(playerInput);
    }

    private void MovementUpdate(Vector2 playerInput)
    {

        //horizontal movement
        if(playerInput.x > 0){
            direction = FacingDirection.right;
            if(speed.x < 0) speed.x = 0;
            speed.x = Mathf.Min(speed.x + acceleration, maxSpeed);
        }
        else if(playerInput.x < 0){
            direction = FacingDirection.left;
            if(speed.x > 0) speed.x = 0;
            speed.x = Mathf.Max(speed.x - acceleration, -maxSpeed);
        }
        else{
            speed.x = 0;
        }

        //apply gravity
        if(!IsGrounded()){
            gravity = -(2f * apexHeight / apexTime) / (apexTime * 2f * 24f);
            speed.y = Mathf.Max(speed.y + gravity, terminalVeloicty);
        }
        else if(speed.y < 0){
            speed.y = 0;
        }

        //vertical movement
        if(playerInput.y == 1){
            this.playerInput.y = 0;
            speed.y = 2f * apexHeight / apexTime;
        }

        //update velocity
        Rigidbody2D rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = new Vector2(speed.x, speed.y);
    }

    public bool IsWalking()
    {
        return speed.x != 0;
    }
    public bool IsGrounded()
    {
        return groundHitBox.grounded;
    }

    public FacingDirection GetFacingDirection()
    {
        return direction;
    }
}

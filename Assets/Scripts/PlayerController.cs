using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public Vector2 speed;                                       //player speed
    public float acceleration = 0.1f;                           //horizontal speed increase per FixedUpdate() call
    public float decceleration = 0f;
    public float maxSpeed = 5f;                                 //maximum horizonal speed
    public Vector2 playerInput;                                 //input vector
    public FacingDirection direction = FacingDirection.right;   //horizontal direction of movement
    public groundDetector groundHitBox;                         //for detecting ground collision
    public groundDetector headHitBox;                           //for detecting head collision
    public groundDetector rightHitBox;                          //for detecting right wall collision
    public groundDetector leftHitBox;                           //for detecting left wall collision
    public float apexHeight;                                    //how high the jump is
    public float apexTime;                                      //how long the jump takes
    float gravity;                                              //how strong gravity is
    public float terminalVeloicty;                              //maximum fall speed
    public float coyoteTime;                                    //time after leaving ground that player can still jump
    float timeSinceGrounded;                                    //time since leaving ground
    public float wallSlideStrength;                             //how much clinging to a wall slows fall speed
    public float dashTime;                                      //how long a dash lasts
    public float dashSpeed;                                     //how fast a dash moves the player
    float dashClock;                                            //timer for the dash
    bool dashCooldown = true;                                   //cooldown flag for dash, resets when grounded
    bool wasGroundedLastFrame = true;                           //flag for if the player was grounded last frame
    bool bounce = false;                                        //flag for if the player should bounce
    public float bounceThreshold;                               //minimum speed to allow a bounce
    public float elasticity;                                    //how much energy is preserved in a bounce

    public enum FacingDirection
    {
        left, right
    }

    void Start()
    {
        speed = new Vector2(0,0);
    }

    // Update is called once per frame
    void Update()
    {
        //set x input to 0
        playerInput.x = 0;

        //left input
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
            playerInput.x = -1;
        }
        //right input
        else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
            playerInput.x  = 1;
        }
        //jump input, only happens if player hasn't been off the ground longer than coyote time allows
        if(playerInput.y == 0 && (timeSinceGrounded < coyoteTime) && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space))){
            playerInput.y = 1;
        }

        //reset dash cooldown when grounded
        if(IsGrounded()){
            dashCooldown = true;
        }

        //dash input
        if(Input.GetKeyDown(KeyCode.LeftShift) && dashCooldown){
            dashClock = dashTime;
            dashCooldown = false;
        }
    }

    void FixedUpdate(){
        //reset time since grounded if grounded
        if(IsGrounded()){
            timeSinceGrounded = 0;
            //if the player was in the air last frame and they are moving faster than the bounce threshold, set bounce flag
            if(!wasGroundedLastFrame && speed.y <= bounceThreshold){
                bounce = true;
            }
        }
        //add time to time since grounded if the player is in the air
        else{
            timeSinceGrounded += Time.deltaTime;
        }

        //decrement the dash timer
        dashClock -= Time.deltaTime;
        
        //update movement
        MovementUpdate(playerInput);

        //update was grounded last frame
        if(IsGrounded()){
            wasGroundedLastFrame = true;
        }
        else{
            wasGroundedLastFrame = false;
        }
    }

    private void MovementUpdate(Vector2 playerInput)
    {

        //horizontal movement
        //if dashing
        if(dashClock > 0){
            //dash right
            if(direction == FacingDirection.right){
                speed.x = dashSpeed;
            }
            //dash left
            else{
                speed.x = -dashSpeed;
            }
        }
        //if not dashing
        else{
            //move right
            if(playerInput.x > 0){
                direction = FacingDirection.right;
                //if moving left and grounded, reset speed to 0, move right instantly
                if(speed.x < 0 && IsGrounded()) speed.x = 0;
                //add right speed
                speed.x = Mathf.Min(speed.x + acceleration, maxSpeed);
            }
            //move left
            else if(playerInput.x < 0){
                direction = FacingDirection.left;
                //if moving right and grounded, reset speed to 0, move leftt instantly
                if(speed.x > 0 && IsGrounded()) speed.x = 0;
                //add left speed
                speed.x = Mathf.Max(speed.x - acceleration, -maxSpeed);
            }
            //if no input, stop the player
            else{
                if(speed.x > 0) speed.x = Mathf.Max(speed.x - decceleration, 0);
                if(speed.x < 0) speed.x = Mathf.Min(speed.x + decceleration, 0);
            }
        }

        //apply gravity
        if(!IsGrounded()){
            gravity = -(2f * apexHeight / apexTime) / (apexTime * 2f * 24f);
            speed.y = Mathf.Max(speed.y + gravity, terminalVeloicty);
            //if the player is clinging to a wall, reduce fall speed
            if((rightHitBox.activated && playerInput.x > 0)|| (leftHitBox.activated && playerInput.x < 0) && speed.y < 0){
                speed.y /= wallSlideStrength;
                //reset time since grounded to allow wall jumps
                timeSinceGrounded = 0;
            }
        }
        //if the player is on the ground, set vertical speed to 0
        else if(speed.y < 0){
            speed.y = 0;
        }

        //if the player hits their head, or if they are dashing, set vertical speed to 0
        if((speed.y > 0 && headHitBox.activated) || dashClock > 0){
            speed.y = 0;
        }

        //jump 
        if(playerInput.y == 1){
            //reset jump input
            this.playerInput.y = 0;
            //set initial jump speed
            speed.y = 2f * apexHeight / apexTime;
            //if wall jumping, set horizontal speed away from the wall
            if((rightHitBox.activated && playerInput.x > 0)){
                speed.x = -maxSpeed;
            }
            else if((leftHitBox.activated && playerInput.x < 0)){
                speed.x = maxSpeed;
            }
        }

        //if bounce flag is set
        if(bounce){
            //set y speed to bounce speed
            speed.y = -bounceThreshold * elasticity;
            //reset bounce flag
            bounce = false;
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
        return groundHitBox.activated;
    }

    public FacingDirection GetFacingDirection()
    {
        return direction;
    }
}

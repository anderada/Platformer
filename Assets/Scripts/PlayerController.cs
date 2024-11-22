using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float speed = 0f;
    public float acceleration = 0.1f;
    public float maxSpeed = 5f;
    public Vector2 playerInput;

    public enum FacingDirection
    {
        left, right
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //The input from the player needs to be determined and then passed in the to the MovementUpdate which should
        //manage the actual movement of the character.
        float xInput = 0;
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
            xInput = -1;
        }
        else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
            xInput = 1;
        }
        playerInput = new Vector2(xInput, Input.GetAxis("Vertical"));
    }

    void FixedUpdate(){
        MovementUpdate(playerInput);
    }

    private void MovementUpdate(Vector2 playerInput)
    {
        if(playerInput.x > 0){
            if(speed < 0) speed = 0;
            speed = Mathf.Min(speed + acceleration, maxSpeed);
        }
        else if(playerInput.x < 0){
            if(speed > 0) speed = 0;
            speed = Mathf.Max(speed - acceleration, -maxSpeed);
        }
        else{
            speed = 0;
        }

        Rigidbody2D rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = new Vector2(speed, rigidBody.velocity.y);
    }

    public bool IsWalking()
    {
        return false;
    }
    public bool IsGrounded()
    {
        return true;
    }

    public FacingDirection GetFacingDirection()
    {
        return FacingDirection.left;
    }
}

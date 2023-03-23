using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerContoller : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 10f; //Walk speed
    [SerializeField] private float runSpeed = 20f; //Run speed
    private float currentSpeed; //Current speed to use for movement

    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float gravity = -9.8f; //initial downward speed
    private float velocity = 0f;

    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private bool mouseVerticalInverted = false;

    private CharacterController controller; //ref for assigning the character controller attached to the player
    private GameManager manager;
    private Transform camTransform; //ref for the main camera's transform

    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Vector3 groundBoxSize = new Vector3(0, 0.1f, 0);
    [SerializeField] private float groundCheckDistance = 1f;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>(); //assign the character controller to the controller var
        controller.detectCollisions = false;

        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        camTransform = GetComponentInChildren<Camera>().transform; //assign the transform of the player's camera
        //camTransform = Camera.main.transform;

        Cursor.visible = false; //Make the cursor not visable
        Cursor.lockState = CursorLockMode.Locked; //Keeps the cursor from moving out of the screen

        currentSpeed = walkSpeed; //set the current move speed to the default move speed;
    }

    private void OnDestroy()
    {
        Cursor.visible = true; //Make the cursor visible
        Cursor.lockState = CursorLockMode.None; //unlocks the cursor
    }

    // Update is called once per frame
    void Update()
    {
        if (!manager.Paused) //no player input if game is paused
        {
            PlayerMovement(); //calls the player movement function

            if (Input.GetKeyDown(KeyCode.Space) && GroundCheck())
            {
                velocity = jumpForce;
            }

            Gravity();
            PlayerRotation(); //calls the player camera aim function
            PlayerRun(); //calls the if player is running check
            //if (GroundCheck())
            //{
            //    velocity = 0;
            //}
        }
    }

    private bool GroundCheck()
    {
        bool onGround = Physics.BoxCast(controller.bounds.center, groundBoxSize, Vector3.down,
            controller.transform.rotation, groundCheckDistance, groundMask) ;  //Raycast from the "Down" position of the box collider))v

        return onGround;  //Raycast from the "Down" position of the box collider))
    }

    private void Gravity()
    {

        velocity -= gravity * Time.deltaTime; //gravity
        Vector3 CharVerticalVect = transform.TransformDirection(new Vector3(0, velocity, 0)); //vertical movement
        controller.Move(CharVerticalVect * Time.deltaTime); //Move the character with the character controller by multiplying the movement vector by time
        
    }

    //"Run" Needs to be added to input mananger
    private void PlayerRun() //checks if the player inputs the run button, and assigns the run speed
    {
        if (Input.GetAxisRaw("Run") > 0 && GroundCheck()) //Check if the Run button is inputed while on the ground
        {
            currentSpeed = runSpeed; //If inputed, set the player's speed to the run speed
        }
        else// if(GroundCheck()) //maintain run speed until grounded
        {
            currentSpeed = walkSpeed; //if not, keep the player's speed to the default speed
        }
    }

    private void PlayerMovement() //Controls the player movement 
    {
        //Creates a vector3 for holding the current horizontal inputs, gets their direction in world space
        Vector3 CharHorizontalVect = transform.TransformDirection(new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized); 
        controller.Move(CharHorizontalVect * currentSpeed * Time.deltaTime); //Move the character with the character controller by multiplying the movement vector by time
        
    }
    private void PlayerRotation() //Controls the player camera aiming
    {
        Vector3 characterRotation = new Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0f); //Creates a vector 3 that holds our mouse x and y movements

        transform.Rotate(new Vector3(0f, characterRotation.y * mouseSensitivity, 0f)); //Rotates the player object around the y axis when the mouse moves in the x axis
        //rotate the player camera around the x axis when the mouse moves in the y axis
        camTransform.Rotate(new Vector3(
            mouseVerticalInverted ? characterRotation.x * mouseSensitivity: -characterRotation.x * mouseSensitivity, 0f, 0f)); //check if mouseVerticalInverted is true, else inverse x rotation
            
    }
}

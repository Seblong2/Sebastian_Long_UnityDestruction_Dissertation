using UnityEditor;
using UnityEngine;
using static CharacterDefaulkts;

public class SCR_Controller : MonoBehaviour
{
    private CharacterController characterController;
    private Player_Input inputActions;
    public Vector2 input_movement;
    public Vector2 input_look;
    private WorldVoxel world;
    private Vector3 velocity;

    public float playerWidth = 0.15f;
    public bool isGrounded;
    


    private Vector3 newCamRot;
    private Vector3 newPlayerRot;

    [Header("Refs")]
    public Transform cam;

    [Header("Settings")]
    public InputSettings playerSettings;
    public float lookClampYMin = -70;
    public float lookClampYMax = 80;

    [Header("Physics stuff")]
    public float gravity = -9.81f;
    public float fallMultiplier = 50f;

    private void Start()
    {
        world = GameObject.Find("WorldVoxel").GetComponent<WorldVoxel>();
    }


    private void Awake()
    {
        inputActions = new Player_Input();

        inputActions.Player_Actions.Movement.performed += e => input_movement = e.ReadValue<Vector2>();
        inputActions.Player_Actions.Look.performed += e => input_look = e.ReadValue<Vector2>();

        inputActions.Enable();

        newCamRot = cam.localRotation.eulerAngles;
        newPlayerRot = transform.localRotation.eulerAngles;

        characterController = GetComponent<CharacterController>();
    }

   

    private void Update()
    {

        calculateLook();


        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        else
        {
            velocity.y += gravity * fallMultiplier * Time.deltaTime;
        }

        Cursor.lockState = CursorLockMode.Locked;

        CalculateVelocity();

        calculateMovement();
    }

    private void CalculateVelocity()
    {
        if ((velocity.z > 0 && playerFront) || (velocity.z < 0 && playerBack)) //Updating checking for collisions
            velocity.z = 0;

        if ((velocity.x > 0 && playerRight) || (velocity.x < 0 && playerLeft)) //Updating checking for collisions
            velocity.x = 0;

        if ((velocity.z > 0 && playerFront) || (velocity.z < 0 && playerBack)) //Updating checking for collisions
            velocity.z = 0;

        if (velocity.y < 0) //Updating checking for collisions
            velocity.y = checkDown(velocity.y);
        else if (velocity.y > 0)
            velocity.y = checkUp(velocity.y);
    }

    private void calculateLook()
    {
        newPlayerRot.y += playerSettings.lookXsens * input_look.x * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(newPlayerRot);


        newCamRot.x -= playerSettings.lookYsens * input_look.y * Time.deltaTime;

        newCamRot.x = Mathf.Clamp(newCamRot.x, lookClampYMin, lookClampYMax);

        cam.localRotation = Quaternion.Euler(newCamRot);
    }

    private void calculateMovement()
    {
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        // NO CLIP SETTINGS IF NEEDED FOR PLAYER //

        /*
        Vector3 camForward = camHolder.forward;
        Vector3 camRight = camHolder.right;


        Vector3 moveDirection = camForward * input_movement.y + camRight * input_movement.x;
        moveDirection.Normalize();

        Vector3 finalVelocity = moveDirection * playerSettings.Forwardspeed * Time.deltaTime;

        characterController.Move(finalVelocity);
        */

        // NO CLIP SETTINGS IF NEEDED FOR PLAYER //

        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * input_movement.y + camRight * input_movement.x;
        moveDir.Normalize();

        Vector3 desiredMove = moveDir * playerSettings.Forwardspeed;

       

        //Handle Z Axis
        if (desiredMove.z > 0 && !playerFront)
            velocity.z = desiredMove.z;
        else if (desiredMove.z < 0 && !playerBack)
            velocity.z = desiredMove.z;
        else
            velocity.z = 0;

        //Handle X Axis
        if (desiredMove.x > 0 && !playerRight)
            velocity.x = desiredMove.x;
        else if (desiredMove.x < 0 && !playerLeft)
            velocity.x = desiredMove.x;
        else
            velocity.x = 0;

        




        characterController.Move(velocity * Time.deltaTime);

        
    }

    private float checkDown(float downspeed)//Collision on the y axis down for the voxel chunks
    {
        Vector3[] checkPoint = new Vector3[]
        {
            new Vector3(transform.position.x - playerWidth, transform.position.y + downspeed, transform.position.z - playerWidth),
            new Vector3(transform.position.x + playerWidth, transform.position.y + downspeed, transform.position.z - playerWidth),
            new Vector3(transform.position.x + playerWidth, transform.position.y + downspeed, transform.position.z + playerWidth),
            new Vector3(transform.position.x - playerWidth, transform.position.y + downspeed, transform.position.z + playerWidth)
        };

        foreach (var point in checkPoint)
        {
            float checkY = point.y + downspeed * Time.deltaTime;
            if (world.playerVoxelCheck(point.x, point.y, point.z))
            {
                isGrounded = true;

                float voxelTopY = Mathf.Floor(checkY) + 1f;

                transform.position = new Vector3(transform.position.x, voxelTopY, transform.position.z);

                return 0;
            }
        }

        isGrounded = false;
        return downspeed;
    }

    private float checkUp(float upspeed) //Collision on the y axis up for the voxel chunks
    {
        Vector3[] checkPoint = new Vector3[]
          {
            new Vector3(transform.position.x - playerWidth, transform.position.y + 2f + upspeed, transform.position.z - playerWidth),
            new Vector3(transform.position.x + playerWidth, transform.position.y + 2f + upspeed, transform.position.z - playerWidth),
            new Vector3(transform.position.x + playerWidth, transform.position.y + 2f + upspeed, transform.position.z + playerWidth),
            new Vector3(transform.position.x - playerWidth, transform.position.y + 2f + upspeed, transform.position.z + playerWidth)
          };
        
        foreach (var point in checkPoint)
        {
            float checkY = point.y + upspeed * Time.deltaTime;
            if (world.playerVoxelCheck(point.x, point.y, point.z))
            {
                isGrounded = true;

                float voxelTopY = Mathf.Floor(checkY) + 1f;

                transform.position = new Vector3(transform.position.x, voxelTopY, transform.position.z);

                velocity.y = 0f;

                return 0;
            }
        }

        isGrounded = false;
        return upspeed;
    }

    public bool playerFront //Collision on the z axis in front of the player for the voxel chunks
    {
        get 
        {
         if (
                world.playerVoxelCheck(transform.position.x, transform.position.y, transform.position.z + playerWidth) ||
                world.playerVoxelCheck(transform.position.x, transform.position.y + 1f, transform.position.z + playerWidth)
                )
                return true;
         else
                return false;
        }
    }
    public bool playerBack //Collision on the z axis behind the player for the voxel chunks
    {
        get
        {
            if (
                   world.playerVoxelCheck(transform.position.x, transform.position.y, transform.position.z - playerWidth) ||
                   world.playerVoxelCheck(transform.position.x, transform.position.y + 1f, transform.position.z - playerWidth)
                   )
                return true;
            else
                return false;
        }
    }
    public bool playerLeft //Collision on the x axis to the left of the player for the voxel chunks
    {
        get
        {
            if (
                   world.playerVoxelCheck(transform.position.x - playerWidth, transform.position.y, transform.position.z ) ||
                   world.playerVoxelCheck(transform.position.x - playerWidth, transform.position.y + 1f, transform.position.z )
                   )
                return true;
            else
                return false;
        }
    }
    public bool playerRight //Collision on the x axis to the Right of the player for the voxel chunks
    {
        get
        {
            if (
                   world.playerVoxelCheck(transform.position.x + playerWidth, transform.position.y, transform.position.z) ||
                   world.playerVoxelCheck(transform.position.x + playerWidth, transform.position.y + 1f, transform.position.z )
                   )
                return true;
            else
                return false;
        }
    }
}

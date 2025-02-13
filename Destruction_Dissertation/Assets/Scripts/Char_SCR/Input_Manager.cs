using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.LowLevel;

public class Input_Manager : MonoBehaviour
{
    private Player_Input playerInput;
    private Player_Input.Player_ActionsActions movement;


    private Player_Movement playerMovement;
    private Player_Look look;

    private void Awake()
    {
        playerInput = new Player_Input();
        movement = playerInput.Player_Actions;
        playerMovement = GetComponent<Player_Movement>();
        look = GetComponent<Player_Look>();
    }
     void FixedUpdate()
    {
        //Tell movement script to move using values from input action map
        playerMovement.ProcessMovement(movement.Movement.ReadValue<Vector2>());
    }

    private void LateUpdate()
    {
        look.ProcessLook(movement.Look.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        movement.Enable();
    }

    private void OnDisable()
    {
        movement.Disable();
    }
}

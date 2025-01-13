using UnityEngine;
using static SCR_Models;
using System;

public class SCR_Controller : MonoBehaviour
{
    private CharacterController characterController;
    private Standard_Input standardInput;
    public Vector2 input_movement;
    public Vector2 input_view;

    private Vector3 CameraRot;
    private Vector3 PlayerRot;

    [Header("Refs")]
    public Transform cameraHold;

    [Header("Settings")]
    public PlayerSettingsModel playerSettings;
    public float ClampYmin = -70;
    public float ClampYmax = 80;

    private void Awake()
    {
        standardInput = new Standard_Input();

        standardInput.Character.Movement.performed += e => input_movement = e.ReadValue<Vector2>();
        standardInput.Character.View.performed += e => input_view = e.ReadValue<Vector2>();

        standardInput.Enable();

        CameraRot = cameraHold.localRotation.eulerAngles;
        PlayerRot = transform.localRotation.eulerAngles;

        characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Confined;

    }
    private void Update()
    {
        ViewCalculation();
        MovementCalculation();
    }

    private void ViewCalculation()
    {
        PlayerRot.y += playerSettings.ViewXsens * input_view.x * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(PlayerRot);



        CameraRot.x -= playerSettings.ViewYsens * input_view.y * Time.deltaTime;
        CameraRot.x = Mathf.Clamp(CameraRot.x, ClampYmin, ClampYmax);

        cameraHold.localRotation = Quaternion.Euler(CameraRot);
    }

    private void MovementCalculation()
    {
        var verticalSpeed = playerSettings.forwardspeed * input_movement.y * Time.deltaTime;
        var horizontalSpeed = playerSettings.strafespeed * input_movement.x * Time.deltaTime;

        Vector3 moveSpeed = new Vector3(horizontalSpeed, 0, verticalSpeed);

        moveSpeed = transform.TransformDirection(moveSpeed);

        characterController.Move(moveSpeed);
    }
}

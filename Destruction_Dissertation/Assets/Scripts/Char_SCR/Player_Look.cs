using UnityEngine;

public class Player_Look : MonoBehaviour
{
    public Camera Camera;
    private float xRotation = 0f;

    public float xSensitivty = 30f;
    public float ySensitivity = 30f;

    public void ProcessLook(Vector2 input)
    {
        float mouseX = input.x;
        float mouseY = input.y;
        //Camera rotation caluclation for up and down
        xRotation -= (mouseY * Time.deltaTime) * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        //Apply to Camera Transforms
        Camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        //Rotate player left and right
        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * xSensitivty);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Camera cam;
    private float xRotation = 0f;
    private float yRotation = 0f;

    public float xSensitivity = 250f;
    public float ySensitivity = 250f;
    
    float topClamp = -80f;
    float bottomClamp = 80f;

    void Start()
    {
        // Locking the cursor to the middle of the screen and making it invisible
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void ProcessLook(Vector2 input)
    {
        float mouseX = input.x;
        float mouseY = input.y;

        // Rotation around the x axis (Look up and down)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);

        // Rotation around the y axis (Look left and right
        yRotation += mouseX;
        
        // Apply the rotations to our transform
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

    }
}

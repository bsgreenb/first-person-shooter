using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;
    bool isMoving;
    private Vector3 lastPosition = new Vector3(0f,0f,0f);
    private bool lerpCrouch = false;
    private bool crouching = false;
    
    public float crouchTimer;

    private bool sprinting;
    public float speed = 5f;
    public float gravity = -9.8f;
    public float jumpHeight = 3f;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded;
        if (lerpCrouch) {
            crouchTimer += Time.deltaTime;
            float p = crouchTimer / 1;
            p *= p;
            if (crouching) {
                controller.height = Mathf.Lerp(controller.height, 1, p);
            } else {
                controller.height = Mathf.Lerp(controller.height, 2, p);
            }

            if (p > 1) {
                lerpCrouch = false;
                crouchTimer = 0f;
            }
        }
    }

    // Receives the inputs from our InputManager.cs and apply them to our character controller.
    public void ProcessMove(Vector2 input) 
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
        playerVelocity.y += gravity * Time.deltaTime;
        if (isGrounded && playerVelocity.y < 0) {
            playerVelocity.y = -2f;
        }
        controller.Move(playerVelocity * Time.deltaTime);
        if (lastPosition != gameObject.transform.position && isGrounded == true) {
            isMoving = true;
        } else {
            isMoving = false;
        }
        lastPosition = gameObject.transform.position;
    }

    public void Jump() 
    {
        if (isGrounded) {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
    }

    public void Crouch()
    {
        crouching = !crouching;
        crouchTimer = 0;
        lerpCrouch = true;
    }

    public void StartSprinting() {
        sprinting = true;
        speed = 10; // Sprint speed
    }

    public void StopSprinting() {
        sprinting = false;
        speed = 5; // Normal speed
    }
}
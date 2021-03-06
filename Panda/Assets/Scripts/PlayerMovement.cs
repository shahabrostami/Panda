﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float walkSpeed = 2;
    public float runSpeed = 6;
    public float turnSpeedTime = 0.2f;
    public float gravity = -12;
    public float jumpHeight = 1;
    [Range(0,1)]
    public float airControlPercent;

    float turnSmoothVelocity;
    public float speedSmoothTime = 0.1f;
    float speedSmoothVelocity;
    float currentSpeed;
    float velocityY;

    Animator animator;
    Transform cameraT;
    CharacterController controller;

    private Vector3 hitNormal;
    private bool isGrounded;
    private float slopeLimit;
    private float collisionAngle;

    void Start () {
        animator = GetComponent<Animator>();
        cameraT = Camera.main.transform;
        controller = GetComponent<CharacterController>();
        isGrounded = true;
        slopeLimit = controller.slopeLimit;
    }
	
	void FixedUpdate ()
    {
        // input section
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector2 input = new Vector2(moveHorizontal, moveVertical);
        Vector2 inputDir = input.normalized;

        bool running = Input.GetKey(KeyCode.LeftShift);
        DoMove(inputDir, running);

        if(Input.GetKeyDown(KeyCode.Space))
            DoJump();

        // animator
        float animationSpeedPercent = ((running) ? currentSpeed / runSpeed : currentSpeed / walkSpeed * 0.5f);
        animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
    }

    void DoJump()
    {
        if(controller.isGrounded && collisionAngle < 45)
        {
            float jumpVelociy = Mathf.Sqrt(-2 * gravity * jumpHeight);
            velocityY = jumpVelociy;
        }
    }

    float GetModifiedSmoothTime(float smoothTime)
    {
        if(controller.isGrounded)
        {
            return smoothTime;
        }

        if (airControlPercent == 0)
            return float.MaxValue;

        return smoothTime / airControlPercent;
    }

    void DoMove(Vector2 inputDir, bool running)
    {
        if (inputDir != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSpeedTime));
        }
        
        float targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));

        // transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);

        velocityY += Time.deltaTime * gravity;
        Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;
        
        if (!isGrounded && inputDir == Vector2.zero)
        {
            velocity.x = (1f - hitNormal.y) * hitNormal.x * (1f - 0.1f);
            velocity.z = (1f - hitNormal.y) * hitNormal.z * (1f - 0.1f);
        }

        controller.Move(velocity * Time.deltaTime);

        currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;

        if (controller.isGrounded)
            velocityY = 0;

        collisionAngle = Vector3.Angle(Vector3.up, hitNormal);
        isGrounded = (collisionAngle) <= 45f;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        hitNormal = hit.normal;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("BarrelPickUp"))
        {
            other.gameObject.SetActive(false);
        }
    }
}

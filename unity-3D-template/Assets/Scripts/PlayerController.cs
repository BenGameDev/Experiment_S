using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 12f;
    public float dashSpeed = 60f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public float groundDistance = 2f;
    public LayerMask groundMask;

    public Vector3 velocity;
    public Vector3 forwardDirection;

    public Transform groundCheck;
    public float x;

    public bool usedDash;
    public bool freeze;
    public bool activeGrapple;
    public bool enableMovementOnNextTouch = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        x = Input.GetAxis("Horizontal");
        Movement();

        if(freeze)
        {
            speed = 0;
        }
        else if(!freeze) 
        {
            speed = 10;
        }
    }

    public void Movement()
    {
        if (activeGrapple) { return; }
        Vector3 input = transform.right * x;

        controller.Move(input * speed * Time.deltaTime);

        Jump();
        Dash();
    }

    public void Jump()
    {
        if (IsGrounded() && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetAxis("Jump") > 0 && IsGrounded())
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
        
        if(!IsGrounded() && Input.GetAxis("Down") >0)
        {
            velocity.y = -Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        
    }

    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;
        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);

        Invoke(nameof(ResetRestrictions), 3f);
    }

    private Vector3 velocityToSet;

    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        controller.Move(velocityToSet);
    }

    public void ResetRestrictions()
    {
        activeGrapple = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(enableMovementOnNextTouch) 
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();

            GetComponent<Grapple>().StopGrapple();
        }
    }
    public void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && usedDash == false)
        {
            if (x < 0)
            {
                forwardDirection = -transform.right;
                usedDash = true;
                if (usedDash == true)
                {
                    velocity.x = -1 * dashSpeed;
                    StartCoroutine(DashTime());
                }
            }
            if(x >= 0)
            {
                forwardDirection = transform.right;
                usedDash = true;
                if (usedDash == true)
                {
                    velocity.x = 1 * dashSpeed;
                    StartCoroutine(DashTime());
                }
            }
            controller.Move(velocity * Time.deltaTime);
        }

    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

    public bool IsGrounded()
    {
        if (Physics.CheckSphere(groundCheck.position, groundDistance, groundMask))
        {
            return true;
        }
        return false;
    }

    IEnumerator DashTime()
    {
        yield return new WaitForSeconds(0.1f);
        velocity = forwardDirection * (dashSpeed / 2);
        yield return new WaitForSeconds(0.15f);
        usedDash = false;
        velocity.x = 0;
        velocity.z = 0;
    }
}

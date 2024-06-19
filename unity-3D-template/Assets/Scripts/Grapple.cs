using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    public PlayerController pc;

    public Vector3 slimePosition;
    public Vector3 grappleDirection;
    public Vector3 grapplePoint;
    public float grappleSpeed;
    public float previousVelocity;

    public GameObject grappleHook;
    public void Update()
    {
        slimePosition = transform.position;
        grapplePoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 13.87f));
        grappleDirection = grapplePoint - slimePosition;
        grappleDirection.z = 0f;
        GrappleHook();
    }
    public void GrappleHook()
    {
        if(Input.GetMouseButton(0)) 
        {
            Ray ray = new Ray(slimePosition, grappleDirection);

            if(Physics.Raycast(ray, out RaycastHit hitData)) 
            { 
                if(hitData.collider.gameObject.tag == "Roof")
                {
                    StartGrapple();
                }
            }
            Debug.DrawRay(slimePosition, grappleDirection);
        }
    }

    public void StartGrapple()
    {
        pc.controller.Move(grappleDirection * grappleSpeed * Time.deltaTime);
        previousVelocity = pc.velocity.y;
        pc.velocity.y = 0f;
        if(transform.position == grapplePoint)
        {
            StopGrapple();
        }
    }

    public void StopGrapple()
    {
        pc.velocity.y = previousVelocity;
    }

}

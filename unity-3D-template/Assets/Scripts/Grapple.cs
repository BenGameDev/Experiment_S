using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    [Header("References")]
    private PlayerController pc;
    public Transform gunTip;
    public LayerMask grappleable;
    public LineRenderer lR;

    [Header("Grapple")]
    public float maxGrappleDist;
    public float grappleDelayTime;
    public float overshootYAxis;

    public Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grappleCD;
    public float grappleCDTimer;

    [Header("Input")]
    private KeyCode grappleKey = KeyCode.Mouse1;

    private bool grappleEnabled;

    private void Start()
    {
        pc = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(grappleKey)) StartGrapple(); Debug.Log("GrappleButtonClicked");
        
        if(grappleCDTimer > 0)
        {
            grappleCDTimer -= Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        if(grappleEnabled) 
        {
            lR.SetPosition(0, gunTip.position);
        }
    }
    private void StartGrapple()
    {
        if (grappleCDTimer > 0) return;
        pc.freeze = true;

        grappleEnabled = true;

        RaycastHit hit;
        if(Physics.Raycast(transform.position, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 13.87f)) - transform.position, out hit, maxGrappleDist, grappleable))
        {
            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grapplePoint = transform.position + Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 13.87f)) - transform.position * maxGrappleDist;
            Invoke(nameof(StopGrapple),grappleDelayTime);
        }
        lR.enabled = true;
        lR.SetPosition(1, grapplePoint);
    }

    private void ExecuteGrapple()
    {
        pc.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        pc.JumpToPosition(grapplePoint, highestPointOnArc);
        Invoke(nameof(StopGrapple), 1f);
    }

    public void StopGrapple()
    {
        pc.freeze = false;
        grappleEnabled = false;
        grappleCDTimer = grappleCD;
        lR.enabled = false;
    }
}


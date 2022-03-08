using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemPickUper : MonoBehaviour
{
    public Transform headPivotPoint;
    public Transform holdingPoint;
    public float maxPickupDistance = 3;
    public LayerMask pickupMask;
    public float rotateSens = 75;
    public float throwForce = 10;
    public float minHoldingDistance = 1;
    public float maxHoldingDistance = 3;
    public float minMaxMouseThrowInput = 5;

    [HideInInspector]
    public string currentItemName;
    [HideInInspector]
    public bool moveable;

    bool HandEmpty;
    float distance;
    float distanceBetweenGrabPointAndCenter;

    RaycastHit hitInfo;

    PlayerLook playerLook;
    void Start()
    {
        playerLook = GetComponent<PlayerLook>();
        HandEmpty = true;
    }

    void Update()
    {
        float mouse0Pos = Input.GetAxisRaw("Fire1");
        float mouse1Pos = Input.GetAxisRaw("Fire2");
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        //if the players hand is empty cast a ray to check if something that can be picked up is in front of it 
        if (HandEmpty)
        {
            print("hand empty");
            if (Physics.Raycast(headPivotPoint.position, headPivotPoint.forward, out hitInfo, maxPickupDistance, pickupMask, QueryTriggerInteraction.Ignore))
            {
                print("hit something");
                currentItemName = hitInfo.collider.name;
                moveable = true;
                //if the player hold mouse 0 (left mouse) the object can be moved by the player
                if (mouse0Pos == 1)
                {
                    print("grabbed something");
                    HandEmpty = false;

                    distance = Vector3.Distance(headPivotPoint.position, hitInfo.transform.position);
                    distanceBetweenGrabPointAndCenter = Vector3.Distance(hitInfo.transform.position, hitInfo.point);

                    holdingPoint.SetPositionAndRotation(hitInfo.transform.position, hitInfo.transform.rotation);

                    hitInfo.collider.attachedRigidbody.velocity = Vector3.zero;
                    hitInfo.collider.attachedRigidbody.angularVelocity = Vector3.zero;

                    EnablePickedupObject(false);
                }
            }
            else
            {
                currentItemName = "";
                moveable = false;
            }

        }// if the player lets go of the mouse button the object falls?
        else if (mouse0Pos == 0) //something in hand and left mouse is up
        {
            print("hand let go");

            HandEmpty = true;
            holdingPoint.position = headPivotPoint.position;
            EnablePickedupObject(true);

            hitInfo.collider.attachedRigidbody.AddForce(GetThrowForce(mouseX, mouseY), ForceMode.Impulse);

        }// move the selected object to the offset point relative to the player
        else if (mouse0Pos == 1) //somehting in hand and left mouse is down
        {
            print("hand holding something");

            float moveGrabedItem = Input.GetAxisRaw("Mouse ScrollWheel");

            //if theres something in the hand and there was scrolling adjust the distance accordingly
            Vector3 dirToCurrentObjectCenter = (holdingPoint.position - headPivotPoint.position).normalized;
            if (moveGrabedItem != 0)
            {
                float updatedDistance = distance + moveGrabedItem;
                //adding and subtracting the difference between the ray hit point and the objects center so the scrolling wont get stuck if the object center is too far or close
                if (updatedDistance <= maxHoldingDistance + distanceBetweenGrabPointAndCenter && updatedDistance >= minHoldingDistance - distanceBetweenGrabPointAndCenter)
                {
                    distance = updatedDistance;
                    holdingPoint.position = headPivotPoint.position + dirToCurrentObjectCenter * distance;
                }
            }

            //if the right mouse button is held down rotate the object holder by the mouse
            if (mouse1Pos == 1)
            {
                playerLook.lockMouseMovement = true;
                //add to the rotation
                holdingPoint.rotation = Quaternion.Euler(holdingPoint.rotation.eulerAngles + mouseX * rotateSens * Time.deltaTime * Vector3.up);
                holdingPoint.rotation = Quaternion.Euler(holdingPoint.rotation.eulerAngles + mouseY * rotateSens * Time.deltaTime * Vector3.right);
            }


            hitInfo.transform.SetPositionAndRotation(holdingPoint.position, holdingPoint.rotation);

            Debug.DrawRay(headPivotPoint.position, dirToCurrentObjectCenter * distance);
        }

        if (playerLook.lockMouseMovement && mouse1Pos == 0)
        {
            playerLook.lockMouseMovement = false;
        }

        //debug ray for the throwforce
        Debug.DrawRay(headPivotPoint.position + headPivotPoint.forward * maxPickupDistance, GetThrowForce(mouseX, mouseY));
    }

    Vector3 GetThrowForce(float mouseX, float mouseY)
    {
        //rotating the force so its always perpendicular to the players x axis
        //the force is rotated by multipling it with the main bodys y rotation
        //rotating on the x axis so when looking up or down the force is always perpendicular to the players y axis
        //the force is rotated by the headPivotpoints x axis and then its rotated by the bodys y axis
        Vector3 rotatedXDirection = Quaternion.AngleAxis(headPivotPoint.parent.parent.rotation.eulerAngles.y, Vector3.up)
                                   * Vector3.right * Mathf.Clamp(GetPlayerSensMouse(mouseX), -minMaxMouseThrowInput, minMaxMouseThrowInput);
        Vector3 rotatedYDirection = Quaternion.AngleAxis(headPivotPoint.parent.parent.rotation.eulerAngles.y, Vector3.up) * Quaternion.AngleAxis(headPivotPoint.eulerAngles.x, Vector3.right)
                                   * Vector3.up * Mathf.Clamp(GetPlayerSensMouse(mouseY), -minMaxMouseThrowInput, minMaxMouseThrowInput);

        return (rotatedXDirection + rotatedYDirection) * throwForce;
    }

    float GetPlayerSensMouse(float mouse)
    {
        return mouse * Time.deltaTime * playerLook.mouseSens;
    }

    void EnablePickedupObject(bool enable)
    {
        if (enable)
        {
            hitInfo.collider.enabled = enable;
            hitInfo.collider.attachedRigidbody.useGravity = enable;
        }
        else
        {
            hitInfo.collider.attachedRigidbody.useGravity = enable;
            hitInfo.collider.enabled = enable;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(headPivotPoint.position, headPivotPoint.forward * maxPickupDistance);

        Vector3 dirToCurrentObjectCenter = (holdingPoint.position - headPivotPoint.position).normalized;

        Gizmos.DrawSphere(headPivotPoint.position + dirToCurrentObjectCenter * distance, .2f);
    }
}

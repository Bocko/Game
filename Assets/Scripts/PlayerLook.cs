using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public float mouseSens = 100;
    public bool mouseInverted = false;
    public float upperLookAngleLimit = -90;
    public float lowerLookAngleLimit = 90;
    public Transform headPivotPoint;
    public bool lockMouseMovement;
    public float camHeightInPlayer { get; private set; }

    Transform playerCam;
    float xRotation = 0f;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        lockMouseMovement = false;
        playerCam = transform.GetComponentInChildren<Camera>().transform;
        camHeightInPlayer = playerCam.localPosition.y;
        headPivotPoint.position = playerCam.position;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;

        if (!lockMouseMovement)
        {
            //negative x rotation to be not inverted
            xRotation += mouseY * (mouseInverted ? 1 : -1);
            xRotation = Mathf.Clamp(xRotation, upperLookAngleLimit, lowerLookAngleLimit);

            //rotate cam then set the same rotation to head pivot point
            playerCam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            headPivotPoint.localRotation = playerCam.localRotation;
            //rotate body to look horizontaly
            transform.Rotate(Vector3.up * mouseX);
        }
    }

    public void SetCamAndHeadPivotLocalYPos(float amount)
    {
        playerCam.localPosition = Vector3.up * amount;
        headPivotPoint.position = playerCam.position;
    }
}

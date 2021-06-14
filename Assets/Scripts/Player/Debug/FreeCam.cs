using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCam : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    public float movementSpeed = 10f;
    public float fastMovementSpeed = 100f;
    public CharacterController controller;
    public Transform debugObject;
    public bool doLockCursor;
    Vector2 mouse;
    float xRotation = 0f;

    private void Start() {
        if (doLockCursor) {
            Cursor.lockState = CursorLockMode.Locked;
        } else {
            Cursor.lockState = CursorLockMode.None;
        }
    }
    private void Update() {
        MouseLook();
        CameraMove();
    }

    void MouseLook() {
        mouse.x = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouse.y = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouse.y;

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        debugObject.Rotate(Vector3.up * mouse.x);
    }

    void CameraMove() {
        var fastMode = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        var moveSpeed = fastMode ? this.fastMovementSpeed : this.movementSpeed;

        Vector3 _move = transform.right * x + transform.forward * z;

        if (Input.GetKey(KeyCode.Q)) {
            controller.Move(Vector3.up * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.E)){
            controller.Move(-Vector3.up * moveSpeed * Time.deltaTime);
        }
        controller.Move(_move * moveSpeed * Time.deltaTime);
    }
}

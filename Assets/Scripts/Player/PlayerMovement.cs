using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Camera cam;
    public WorldGenerator world;

    public float moveSpeed;
    public float fastMoveSpeed = 100f;
    private bool cursorEnabled = true;
    public bool debugMode;
    void Update() {
        var fastMode = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        var movementSpeed = fastMode ? this.fastMoveSpeed : this.moveSpeed;

        transform.position = Vector3.MoveTowards(transform.position, transform.position + (cam.transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal")), Time.deltaTime * movementSpeed);
        transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y"), 0, 0));
        cam.transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0));


        if (cursorEnabled)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;

        if (Input.GetKeyDown(KeyCode.Escape)) {
            cursorEnabled = !cursorEnabled;
        }



        if (Input.GetKey(KeyCode.E)) {
            transform.position = transform.position + (transform.up * movementSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.Q)) {
            transform.position = transform.position + (-transform.up * movementSpeed * Time.deltaTime);
        }

        if (Input.GetMouseButton(0)) {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1f));
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit)) {
                if (hit.transform.tag == "Terrain")
                    world.GetChunkFromVector3(hit.transform.position).PlaceTerrain(hit.point);

            } else {
                Debug.Log("Nothing Clicked");
            }
        }

        if (Input.GetMouseButton(1)) {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1f));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                

                
                if (hit.transform.tag == "Terrain")
                    world.GetChunkFromVector3(hit.transform.position).RemoveTerrain(hit.point);

            } else {
                Debug.Log("Nothing Clicked");
            }
        }
    }
}

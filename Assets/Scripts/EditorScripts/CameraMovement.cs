using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    float speed = 10f;
    float rotationSpeed = 500f;
    float zoomSpeed = 500f;
    bool movement;

    // Start is called before the first frame update
    void Start()
    {
        movement = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!movement) {
            return;
        }
        Movement();
        Zoom();
        Rotation();
    }

    void Movement() {
        float xAxis = Input.GetAxis("Horizontal");
        float zAxis = Input.GetAxis("Vertical");
        
        // lock movement to XZ axis
        Vector3 forward = transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = transform.right;
        right.y = 0;
        right.Normalize();
        
        Vector3 move = (right * xAxis + forward * zAxis) * speed * Time.deltaTime;
        transform.Translate(move, Space.World);
    }

    void Zoom() {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 move = transform.forward * scroll * zoomSpeed * 10f * Time.deltaTime;
        transform.Translate(move, Space.World);
    }

    void Rotation() {
        if (Input.GetMouseButton(2)) {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            Vector3 rotation = new Vector3(-mouseY, mouseX, 0) * rotationSpeed * Time.deltaTime;
            transform.eulerAngles += rotation;
        }
    }

    public void StopMovement() {
        movement = false;
    }

    public void StartMovement() {
        movement = true;
    }
}

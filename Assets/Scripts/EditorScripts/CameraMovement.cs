using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CameraMovement : MonoBehaviour
{

    // public float speed = 10f;
    // public float rotationSpeed = 500f;
    float zoomSpeed = 500f;
    bool movement;
    private float minBoundX = -10f;
    private float maxBoundX = 10f;
    private float minBoundZ = -10f;
    private float maxBoundZ = 10f;
    public TMP_InputField cameraSpeedInput;
    public TMP_InputField cameraRotationSpeedInput;
    public Slider cameraSpeedSlider;
    public Slider cameraRotationSpeedSlider;

    // Start is called before the first frame update
    void Start()
    {
        movement = false;
        cameraSpeedInput.text = CameraSpeed.speed.ToString();
        cameraSpeedSlider.value = CameraSpeed.speed;
        cameraRotationSpeedInput.text = CameraSpeed.rotationSpeed.ToString();
        cameraRotationSpeedSlider.value = CameraSpeed.rotationSpeed;
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
        
        Vector3 move = (right * xAxis + forward * zAxis) * CameraSpeed.speed * Time.deltaTime;
        transform.Translate(move, Space.World);

        if (transform.position.x < minBoundX) {
            transform.position = new Vector3(minBoundX, transform.position.y, transform.position.z);
        }

        if (transform.position.x > maxBoundX) {
            transform.position = new Vector3(maxBoundX, transform.position.y, transform.position.z);
        }

        if (transform.position.z < minBoundZ) {
            transform.position = new Vector3(transform.position.x, transform.position.y, minBoundZ);
        }

        if (transform.position.z > maxBoundZ) {
            transform.position = new Vector3(transform.position.x, transform.position.y, maxBoundZ);
        }
    }

    void Zoom() {

        if (IsPointerOverUIObject()) {
            return;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 move = transform.forward * scroll * zoomSpeed * 10f * Time.deltaTime;

        Vector3 newPosition = transform.position + move;

        if (newPosition.y < 3f) {
            transform.position = new Vector3(transform.position.x, 3f, transform.position.z);
           return;
        } else if (newPosition.y > 20f) {
            transform.position = new Vector3(transform.position.x, 20f, transform.position.z);
            return;
        }
        
        transform.Translate(move, Space.World);

    }

    void Rotation() {
        if (Input.GetMouseButton(2)) {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            Vector3 rotation = new Vector3(-mouseY, mouseX, 0) * CameraSpeed.rotationSpeed * Time.deltaTime;
            transform.eulerAngles += rotation;
        }
    }

    public void StopMovement() {
        movement = false;
    }

    public void StartMovement() {
        movement = true;
    }

    public void updateBounds(bool expand, bool max, float xAdjustment, float zAdjustment) {
        if (expand) {
            if (max) {
                maxBoundX += xAdjustment;
                maxBoundZ += zAdjustment;
            } else {
                minBoundX -= xAdjustment;
                minBoundZ -= zAdjustment;
            }
        } else {
            if (max) {
                maxBoundX -= xAdjustment;
                maxBoundZ -= zAdjustment;
            } else {
                minBoundX += xAdjustment;
                minBoundZ += zAdjustment;
            }
        }
    }

    public void setCameraSpeed() {
        float newSpeed = CameraSpeed.speed;
        if (cameraSpeedInput.text != CameraSpeed.speed.ToString()) {
            newSpeed = float.Parse(cameraSpeedInput.text);
        } else if (cameraSpeedSlider.value != CameraSpeed.speed) {
            newSpeed = cameraSpeedSlider.value;
        }
        if (newSpeed < cameraSpeedSlider.minValue) {
            newSpeed = cameraSpeedSlider.minValue;
        }
        CameraSpeed.speed = newSpeed;
        cameraSpeedInput.text = newSpeed.ToString();
        cameraSpeedSlider.value = newSpeed;
    }

    public void setCameraRotationSpeed() {
        float newRotationSpeed = CameraSpeed.rotationSpeed;
        if (cameraRotationSpeedInput.text != CameraSpeed.rotationSpeed.ToString()) {
            newRotationSpeed = float.Parse(cameraRotationSpeedInput.text);
        } else if (cameraRotationSpeedSlider.value != CameraSpeed.rotationSpeed) {
            newRotationSpeed = cameraRotationSpeedSlider.value;
        }
        if (newRotationSpeed < cameraRotationSpeedSlider.minValue) {
            newRotationSpeed = cameraRotationSpeedSlider.minValue;
            
        }
        CameraSpeed.rotationSpeed = newRotationSpeed;
        cameraRotationSpeedInput.text = newRotationSpeed.ToString();
        cameraRotationSpeedSlider.value = newRotationSpeed;
    }

    private bool IsPointerOverUIObject() {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}

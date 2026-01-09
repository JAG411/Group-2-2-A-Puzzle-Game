using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    
    public float speed = 5f;
    public bool movement = false;
    private Rigidbody rb;
    public HoldPlayer holdPlayer;
    Vector3 initPosition;
    Quaternion initRotation;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("No Rigidbody component found on " + gameObject.name + ". Please add a Rigidbody component.");
            return;
        }
        rb.useGravity = false;
        rb.isKinematic = false;
        initPosition = transform.position;
        initRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        // Stop if movement is disabled
        if (!movement)
            return;

        // Stop if HoldPlayer locks movement
        if (holdPlayer != null && !holdPlayer.canMove)
            return;

        // Move player forward
        rb.MovePosition(rb.position + transform.forward * speed * Time.deltaTime);
    }

    public void StartMovement() {
        movement = true;
        if (rb != null) {
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }

    public void PauseMovement()
    {
        movement = false;
        if (rb != null) {
            rb.velocity = Vector3.zero;
        }
    }

    public void ResetPosition()
    {
        PauseMovement();
        transform.position = initPosition;
        transform.rotation = initRotation;
    }
}

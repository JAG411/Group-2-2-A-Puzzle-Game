using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    
    public float speed = 5f;
    public bool movement = false;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("No Rigidbody component found on " + gameObject.name + ". Please add a Rigidbody component.");
            return;
        }
        // rb.useGravity = false;
        // rb.isKinematic = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!movement) {
            return;
        }
        rb.MovePosition(rb.position + transform.forward * speed * Time.deltaTime);
    }

    public void StartMovement() {
        movement = true;
        // rb.useGravity = true;
        // rb.isKinematic = false;
        // rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }
}

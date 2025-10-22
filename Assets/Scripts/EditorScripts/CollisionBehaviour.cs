using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionBehaviour : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Rotatable") {
            Debug.Log("Collision occurred with object");
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject.tag == "Direction") {
            Vector3 characterHitPoint = transform.position;
            Vector3 triggerHitPoint = other.transform.InverseTransformPoint(characterHitPoint);

            if (Mathf.Abs(triggerHitPoint.x) < 0.1f && Mathf.Abs(triggerHitPoint.z) < 0.1f) {
                // Hit near the center
                transform.forward = other.transform.forward;
            }
        }
    }
}

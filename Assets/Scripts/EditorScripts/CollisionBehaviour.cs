using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionBehaviour : MonoBehaviour
{
    public LevelCompleteBehaviour levelCompleteUI;

    public GameFailBehaviour gameFailUI;

    void Start()
    {
        gameFailUI = Object.FindFirstObjectByType<GameFailBehaviour>(FindObjectsInactive.Include);

        // Auto-find the LevelCompleteBehaviour in the scene
        levelCompleteUI = FindFirstObjectByType<LevelCompleteBehaviour>(FindObjectsInactive.Include);

        if (levelCompleteUI == null)
        {
            Debug.LogWarning("LevelCompleteBehaviour not found in scene!");
        }

        if (gameFailUI == null)
        {
            Debug.LogWarning("GameFailBehaviour not found in scene!");
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Game Over");
            gameFailUI.ShowFail();
            return;
        }

        if (collision.gameObject.tag == "Rotatable")
        {
            Debug.Log("Collision occurred with object");
            Debug.Log("Game Over");
            gameFailUI.ShowFail();
            Destroy(this.gameObject);
        }

        if (collision.gameObject.tag == "Goal") {
            levelCompleteUI.ShowLevelComplete();
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
        } else if (other.gameObject.tag == "Goal") {
            levelCompleteUI.ShowLevelComplete();
        }
    }
}

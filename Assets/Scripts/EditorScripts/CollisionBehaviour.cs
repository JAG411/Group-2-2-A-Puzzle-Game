using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollisionBehaviour : MonoBehaviour
{
    public LevelCompleteBehaviour levelCompleteUI;

    public GameFailBehaviour gameFailUI;
    [SerializeField] float colorLerpSpeed = 5f;
    [SerializeField] Color highlightColor = Color.green;
    AudioSource source;
    public AudioClip tileSound, eatSound;

    // Track all currently highlighted directions
    private Dictionary<Renderer, Color[]> highlightedDirections = new Dictionary<Renderer, Color[]>();
    private Dictionary<Renderer, Coroutine> activeCoroutines = new Dictionary<Renderer, Coroutine>();
    CharacterMovement movement;
    bool isSpeedTile = false;
    float initSpeed;
    int score = 0;
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
        source = Camera.main.GetComponent<AudioSource>();
        movement = GetComponent<CharacterMovement>();
        initSpeed = movement.speed;
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

    private void OnTriggerStay(Collider other) 
    {
        if (other.gameObject.CompareTag("SpeedTile"))
        {
            Vector3 characterHitPoint = transform.position;
            Vector3 triggerHitPoint = other.transform.InverseTransformPoint(characterHitPoint);

            if (Mathf.Abs(triggerHitPoint.x) < 0.1f && Mathf.Abs(triggerHitPoint.z) < 0.1f) 
            {
                transform.forward = other.transform.forward;
            }
            movement.speed = initSpeed + 20f;
            isSpeedTile = true;
        }
        
            
        if (other.gameObject.CompareTag("Direction") || other.gameObject.CompareTag("Food")) {
            Vector3 characterHitPoint = transform.position;
            Vector3 triggerHitPoint = other.transform.InverseTransformPoint(characterHitPoint);

            if (Mathf.Abs(triggerHitPoint.x) < 0.1f && Mathf.Abs(triggerHitPoint.z) < 0.1f) {
                // Hit near the center
                transform.forward = other.transform.forward;
                HighlightDirection(other);
            }
        } else if (other.gameObject.tag == "Goal") {
            levelCompleteUI.ShowLevelComplete();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Direction"))
        {
            source.clip = tileSound;
            source.Play();
        }
// Check if player touched a door
    Door door = other.GetComponent<Door>();
    if (door != null)
    {
        if (door.IsOpen())
        {
// Door is open, level completed
            levelCompleteUI.ShowLevelComplete();
        }
        else
        {
// Door is closed, game over
            gameFailUI.ShowFail();
        }
        return;
    }
        if (other.CompareTag("Food"))
        {
            source.clip = eatSound;
            source.Play();
            score++;
            GameObject.FindGameObjectWithTag("Score").GetComponent<Text>().text = "Score: " + score;
            other.GetComponent<FoodHandler>().enabled = true;
        }
        if (other.CompareTag("Acid") || other.CompareTag("Lava") || other.CompareTag("Spikes"))
        {
            Debug.Log("Game Over");
            gameFailUI.ShowFail();
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
         if (isSpeedTile)
        {
            movement.speed = initSpeed;
            isSpeedTile = false;
        }
        if (other.CompareTag("Direction"))
        {
            ResetDirectionColor(other);
        }
    }

    // ================= DIRECTION COLOR HANDLING =================

    void HighlightDirection(Collider direction)
    {
        Renderer rend = direction.GetComponent<Renderer>();
        if (rend == null)
            rend = direction.GetComponentInChildren<Renderer>();

        if (rend == null) return;

        // Cache original colors per renderer
        if (!highlightedDirections.ContainsKey(rend))
        {
            Color[] originalColors = new Color[rend.materials.Length];
            for (int i = 0; i < rend.materials.Length; i++)
                originalColors[i] = rend.materials[i].color;

            highlightedDirections[rend] = originalColors;
        }

        // Stop any existing coroutine for this renderer
        if (activeCoroutines.ContainsKey(rend) && activeCoroutines[rend] != null)
            StopCoroutine(activeCoroutines[rend]);

        // Start lerp to highlight
        activeCoroutines[rend] = StartCoroutine(LerpColor(rend, highlightColor));
    }

    void ResetDirectionColor(Collider direction)
    {
        Renderer rend = direction.GetComponent<Renderer>();
        if (rend == null)
            rend = direction.GetComponentInChildren<Renderer>();

        if (rend == null || !highlightedDirections.ContainsKey(rend)) return;

        // Stop any active coroutine for this renderer
        if (activeCoroutines.ContainsKey(rend) && activeCoroutines[rend] != null)
            StopCoroutine(activeCoroutines[rend]);

        // Start lerp back to original
        activeCoroutines[rend] = StartCoroutine(LerpColor(rend, highlightedDirections[rend], removeAfter: true));
    }

    IEnumerator LerpColor(Renderer rend, Color target, bool removeAfter = false)
    {
        Material[] mats = rend.materials;

        while (true)
        {
            bool done = true;

            for (int i = 0; i < mats.Length; i++)
            {
                mats[i].color = Color.Lerp(mats[i].color, target, Time.deltaTime * colorLerpSpeed);
                if (Vector4.Distance(mats[i].color, target) > 0.01f)
                    done = false;
            }

            if (done) break;
            yield return null;
        }

        if (removeAfter)
        {
            // Clean up dictionaries when done reverting
            activeCoroutines.Remove(rend);
            highlightedDirections.Remove(rend);
        }
    }

    IEnumerator LerpColor(Renderer rend, Color[] targets, bool removeAfter)
    {
        Material[] mats = rend.materials;

        while (true)
        {
            bool done = true;

            for (int i = 0; i < mats.Length; i++)
            {
                mats[i].color = Color.Lerp(mats[i].color, targets[i], Time.deltaTime * colorLerpSpeed);
                if (Vector4.Distance(mats[i].color, targets[i]) > 0.01f)
                    done = false;
            }

            if (done) break;
            yield return null;
        }
    }
}

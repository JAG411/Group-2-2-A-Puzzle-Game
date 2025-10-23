using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public float EnemySpeed = 2f;
    public Vector3 ChangeDirection = Vector3.forward;
    public Vector3 startDirection;
    void Start()
    {
        ChangeDirection = transform.forward.normalized;
        startDirection = ChangeDirection;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(ChangeDirection * EnemySpeed * Time.deltaTime, Space.World);
    }
    void OnCollisionEnter(Collision col)
    {
        ChangeDirection = -ChangeDirection;
    }
    public void ChangeAxes()
    {
        if (Mathf.Abs(ChangeDirection.z) > Mathf.Abs(ChangeDirection.x))
    {
        ChangeDirection = Vector3.right;
    }
    else
    {
        ChangeDirection = Vector3.forward;
    }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBehaviour : MonoBehaviour
{
    public float baseAccelerationForce;
    public float maxSpeed;

    public SchoolHandler school;
    public Transform display;

    private Rigidbody2D rb;
    private Vector2 currentDirection;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        currentDirection.Normalize();

        rb.velocity = currentDirection * maxSpeed;
    }

    private void FixedUpdate()
    {
        OrientByMovement();
        /*if(rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
        else
        {
            rb.velocity += currentDirection * baseAccelerationForce;
        }*/
    }

    private void OrientByMovement()
    {
        display.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, currentDirection));
    }

    private Vector2 GetDirectionFromAngle(float angle)
    {
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }
}

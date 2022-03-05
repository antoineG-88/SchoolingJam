using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBehaviour : MonoBehaviour
{
    public float baseAccelerationForce;
    public float maxSpeed;

    public float directingBaseForce;
    public float separationBaseForce;
    public float separationForcePower;
    public float separationAwarenessDistance;
    public float cohesionBaseForce;
    public float cohesionAwarenessDistance;
    public float alignmentBaseForce;
    public float alignmentAwarenessDistance;

    public Transform display;

    [HideInInspector]
    public SchoolHandler schoolHandler;

    private Rigidbody2D rb;
    [HideInInspector]
    public Vector2 currentDirection;
    private NeighbourFish[] schoolFish;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        currentDirection.Normalize();

    }

    public void Initiate()
    {
        schoolFish = new NeighbourFish[schoolHandler.school.Count];
        for (int i = 0; i < schoolFish.Length; i++)
        {
            schoolFish[i] = new NeighbourFish(schoolHandler.school[i]);
        }

        if (schoolHandler.school[0] == this)
        {
            display.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    private void FixedUpdate()
    {
        UpdateFishDistance();

        rb.velocity += SeparationForce() + CohesionForce() + AlignmentForce() + DirectingForce();
        if(rb.velocity.magnitude > 0)
        {
            currentDirection = rb.velocity.normalized;
        }

        if (rb.velocity.magnitude >= maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
        else
        {
            rb.velocity += currentDirection * baseAccelerationForce * Time.deltaTime;
        }

        OrientByMovement();
    }

    Vector2 directionVector;
    Vector2 separationForce;
    private Vector2 SeparationForce()
    {
        separationForce = Vector2.zero;
        for (int i = 0; i < schoolFish.Length; i++)
        {
            if(schoolFish[i].distance < separationAwarenessDistance && schoolFish[i].distance > 0.1f)
            {
                directionVector = transform.position - schoolFish[i].fish.transform.position;
                directionVector.Normalize();

                separationForce += directionVector * Mathf.Pow(1 - (schoolFish[i].distance / 5), separationForcePower) * separationBaseForce * Time.deltaTime;
            }
        }

        return separationForce;
    }

    Vector2 cohesionCenter;
    Vector2 cohesionForce;
    int counter;
    private Vector2 CohesionForce()
    {
        cohesionForce = Vector2.zero;
        cohesionCenter = Vector2.zero;
        counter = 0;
        for (int i = 0; i < schoolFish.Length; i++)
        {
            if (schoolFish[i].distance < cohesionAwarenessDistance && schoolFish[i].distance > 0.1f)
            {
                counter++;
                cohesionCenter += (Vector2)schoolFish[i].fish.transform.position;
            }
        }
        cohesionCenter = cohesionCenter / counter;

        directionVector = cohesionCenter - (Vector2)transform.position;
        directionVector.Normalize();
        cohesionForce = directionVector * cohesionBaseForce * Time.deltaTime;
        if(schoolHandler.school[0] == this)
        {
            Debug.DrawLine(cohesionCenter, cohesionCenter + Vector2.up * 0.1f, Color.red);
        }
        return cohesionForce;
    }

    Vector2 alignmentForce;
    Vector2 averageAlignment;
    private Vector2 AlignmentForce()
    {
        alignmentForce = Vector2.zero;
        averageAlignment = Vector2.zero;
        counter = 0;
        for (int i = 0; i < schoolFish.Length; i++)
        {
            if (schoolFish[i].distance < alignmentAwarenessDistance && schoolFish[i].distance > 0.1f)
            {
                counter++;
                averageAlignment += schoolFish[i].fish.currentDirection;
            }
        }
        averageAlignment = averageAlignment / counter;

        directionVector = averageAlignment;
        directionVector.Normalize();
        alignmentForce = directionVector * alignmentBaseForce * Time.deltaTime;

        return alignmentForce;
    }

    Vector2 tempForce;
    private Vector2 TempForce()
    {
        tempForce = Vector2.zero;
        if(transform.position.magnitude > 20)
        {
            tempForce = -transform.position;
            tempForce.Normalize();
            tempForce *= 2;
        }
        return tempForce;
    }

    private Vector2 DirectingForce()
    {
        directionVector = schoolHandler.directingTarget - (Vector2)transform.position;
        directionVector.Normalize();

        return directionVector * directingBaseForce * Time.deltaTime;
    }

    private void UpdateFishDistance()
    {
        for (int i = 0; i < schoolHandler.school.Count; i++)
        {
            schoolFish[i].distance = Vector2.Distance(transform.position, schoolHandler.school[i].transform.position);
        }
    }

    private void OrientByMovement()
    {
        display.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, currentDirection));
    }

    private Vector2 GetDirectionFromAngle(float angle)
    {
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }

    public class NeighbourFish
    {
        public float distance;
        public FishBehaviour fish;

        public NeighbourFish(FishBehaviour _fish)
        {
            fish = _fish;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, separationAwarenessDistance);
    }
}

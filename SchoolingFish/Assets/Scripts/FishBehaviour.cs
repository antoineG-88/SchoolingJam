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
    public float perturbationBaseForce;
    public float perturbationTime;
    public Vector2 perturbationMinMaxTimeInterval;

    public Transform display;

    [HideInInspector]
    public SchoolHandler schoolHandler;
    [HideInInspector]
    public bool isControlled;

    private Rigidbody2D rb;
    [HideInInspector]
    public Vector2 currentDirection;
    private NeighbourFish[] allFishInfo;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        currentDirection.Normalize();

        perturbationForce = Vector2.zero;
        perturbationTimeRemaining = -1;
        timeBeforeNextPerturbation = Random.Range(perturbationMinMaxTimeInterval.x, perturbationMinMaxTimeInterval.y);
    }

    public void Initiate()
    {
        allFishInfo = new NeighbourFish[GameManager.allFish.Count];
        for (int i = 0; i < allFishInfo.Length; i++)
        {
            allFishInfo[i] = new NeighbourFish(GameManager.allFish[i]);
        }
    }

    private void Update()
    {
        UpdatePerturbation();
    }

    private void FixedUpdate()
    {
        if(GameManager.fishInitialzed)
        {
            UpdateFishDistance();

            rb.velocity += SeparationForce() + CohesionForce() + AlignmentForce() + DirectingForce() + PerturbationForce() + TempForce();
            if (rb.velocity.magnitude > 0)
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
    }

    Vector2 directionVector;
    Vector2 separationForce;
    private Vector2 SeparationForce()
    {
        separationForce = Vector2.zero;
        for (int i = 0; i < allFishInfo.Length; i++)
        {
            if(allFishInfo[i].distance < separationAwarenessDistance && allFishInfo[i].distance > 0.1f)
            {
                directionVector = transform.position - allFishInfo[i].fish.transform.position;
                directionVector.Normalize();

                separationForce += directionVector * Mathf.Pow(1 - (allFishInfo[i].distance / 5), separationForcePower) * separationBaseForce * Time.deltaTime;
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
        for (int i = 0; i < allFishInfo.Length; i++)
        {
            if (allFishInfo[i].distance < cohesionAwarenessDistance && allFishInfo[i].distance > 0.1f)
            {
                counter++;
                cohesionCenter += (Vector2)allFishInfo[i].fish.transform.position;
            }
        }
        cohesionCenter = cohesionCenter / counter;

        directionVector = cohesionCenter - (Vector2)transform.position;
        directionVector.Normalize();
        cohesionForce = directionVector * cohesionBaseForce * Time.deltaTime;
        return cohesionForce;
    }

    Vector2 alignmentForce;
    Vector2 averageAlignment;
    private Vector2 AlignmentForce()
    {
        alignmentForce = Vector2.zero;
        averageAlignment = Vector2.zero;
        counter = 0;
        for (int i = 0; i < allFishInfo.Length; i++)
        {
            if (allFishInfo[i].distance < alignmentAwarenessDistance && allFishInfo[i].distance > 0.1f)
            {
                counter++;
                averageAlignment += allFishInfo[i].fish.currentDirection;
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
        if(transform.position.magnitude > 50)
        {
            tempForce = -transform.position;
            tempForce.Normalize();
            tempForce *= 2;
        }
        return tempForce;
    }

    private Vector2 DirectingForce()
    {
        if(isControlled)
        {
            directionVector = schoolHandler.directingTarget - (Vector2)transform.position;
            directionVector.Normalize();

            return directionVector * directingBaseForce * Time.deltaTime;
        }
        else
        {
            return Vector2.zero;
        }
    }

    private Vector2 PerturbationForce()
    {
        return perturbationForce;
    }

    float timeBeforeNextPerturbation;
    float perturbationTimeRemaining;
    Vector2 perturbationForce;
    public void UpdatePerturbation()
    {
        if(timeBeforeNextPerturbation <= 0)
        {
            if(perturbationTimeRemaining == -1)
            {
                perturbationTimeRemaining = perturbationTime;
                perturbationForce = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                perturbationForce = perturbationForce.normalized * perturbationBaseForce;
            }

            if (perturbationTimeRemaining > 0)
            {
                perturbationTimeRemaining -= Time.deltaTime;
            }
            else
            {
                perturbationForce = Vector2.zero;
                perturbationTimeRemaining = -1;
                timeBeforeNextPerturbation = Random.Range(perturbationMinMaxTimeInterval.x, perturbationMinMaxTimeInterval.y);
            }
        }
        else
        {
            timeBeforeNextPerturbation -= Time.deltaTime;
        }
    }

    private void UpdateFishDistance()
    {
        for (int i = 0; i < GameManager.allFish.Count; i++)
        {
            allFishInfo[i].distance = Vector2.Distance(transform.position, GameManager.allFish[i].transform.position);
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

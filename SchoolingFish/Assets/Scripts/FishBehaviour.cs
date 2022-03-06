using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBehaviour : MonoBehaviour
{
    [Header("General")]
    public float baseAccelerationForce;
    public float maxSpeed;
    public float maxSpeedInWild;
    [Space]
    [Header("Forces")]
    public float directingBaseForce;
    [Header("Separation")]
    public float separationBaseForce;
    public float separationForcePower;
    public float separationAwarenessDistance;
    [Header("Cohesion")]
    public float cohesionBaseForce;
    public float cohesionAwarenessDistance;
    [Header("Alignment")]
    public float alignmentBaseForce;
    public float alignmentAwarenessDistance;
    [Header("Perturbation")]
    public float perturbationBaseForce;
    public float perturbationTime;
    public Vector2 perturbationMinMaxTimeInterval;
    [Header("Shelter")]
    public float shelterBaseForce;
    [Space]
    public SpriteRenderer display;
    public GameObject pickUpEffect;
    public Material schoolMaterial;
    public Material wildMaterial;
    public Material shelteredMaterial;

    [HideInInspector]
    public SchoolHandler schoolHandler;
    [HideInInspector]
    public WildFish wildFishOrigin;
    [HideInInspector]
    public bool isControlled;
    [HideInInspector]
    public bool isBeingPickedUp;
    [HideInInspector]
    public bool isAnOriginal;
    [HideInInspector]
    public bool isInShelter;

    private Rigidbody2D rb;
    [HideInInspector]
    public Vector2 currentDirection;
    private NeighbourFish[] allFishInfo;
    public CoralShelter currentShelter;

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
        if(!isInShelter)
        {
            allFishInfo = new NeighbourFish[GameManager.allFish.Count];
            for (int i = 0; i < allFishInfo.Length; i++)
            {
                allFishInfo[i] = new NeighbourFish(GameManager.allFish[i]);
            }
        }
        else
        {
            allFishInfo = new NeighbourFish[currentShelter.allShelterFish.Count];
            for (int i = 0; i < allFishInfo.Length; i++)
            {
                allFishInfo[i] = new NeighbourFish(currentShelter.allShelterFish[i]);
            }
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
            if(!isInShelter)
            {
                UpdateFishDistance();

                rb.velocity += SeparationForce() + CohesionForce() + AlignmentForce() + DirectingForce() + PerturbationForce() + TempForce();
            }
            else
            {
                UpdateFishDistance();

                rb.velocity += SeparationForce() + CohesionForce() + AlignmentForce() + ShelterForce();

            }

            if (rb.velocity.magnitude > 0)
            {
                currentDirection = rb.velocity.normalized;
            }

            if (rb.velocity.magnitude >= (isControlled ? maxSpeed : maxSpeedInWild))
            {
                rb.velocity = rb.velocity.normalized * (isControlled ? maxSpeed : maxSpeedInWild);
            }
            else
            {
                rb.velocity += currentDirection * baseAccelerationForce * Time.fixedDeltaTime;
            }

            OrientByMovement();
        }
    }


    #region Forces
    Vector2 directionVector;
    Vector2 separationForce;
    private Vector2 SeparationForce()
    {
        separationForce = Vector2.zero;
        for (int i = 0; i < allFishInfo.Length; i++)
        {
            if(allFishInfo[i].distance < separationAwarenessDistance)
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
            if (allFishInfo[i].distance < cohesionAwarenessDistance)
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
            if (allFishInfo[i].distance < alignmentAwarenessDistance)
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
        if(transform.position.magnitude > 80)
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

    Vector2 shelterDirection;
    public Vector2 ShelterForce()
    {
        shelterDirection = currentShelter.shelterCenter - (Vector2)transform.position;
        shelterDirection.Normalize();
        return shelterBaseForce * shelterDirection * Time.fixedDeltaTime;
    }

    #endregion

    private void UpdateFishDistance()
    {
        for (int i = 0; i < allFishInfo.Length; i++)
        {
            allFishInfo[i].distance = Vector2.Distance(transform.position, allFishInfo[i].fish.transform.position);
            if(isControlled && allFishInfo[i].distance < schoolHandler.pickUpWildFishDistance && !allFishInfo[i].fish.isControlled && !allFishInfo[i].fish.isBeingPickedUp && !allFishInfo[i].fish.isInShelter)
            {
                schoolHandler.PickWildFish(allFishInfo[i].fish);
            }
        }
    }

    private void OrientByMovement()
    {
        display.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, currentDirection));
    }

    private Vector2 GetDirectionFromAngle(float angle)
    {
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }

    public void PickUpEffect()
    {
        Instantiate(pickUpEffect, transform);
        display.sharedMaterial = schoolMaterial;
    }

    public void SetShelter(CoralShelter shelter)
    {
        isInShelter = true;
        currentShelter = shelter;
        GameManager.allFish.Remove(this);
        schoolHandler.school.Remove(this);
        isControlled = false;
        display.sharedMaterial = shelteredMaterial;
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
        Gizmos.DrawWireSphere(transform.position, alignmentAwarenessDistance);
        Gizmos.DrawWireSphere(transform.position, cohesionAwarenessDistance);
    }
}

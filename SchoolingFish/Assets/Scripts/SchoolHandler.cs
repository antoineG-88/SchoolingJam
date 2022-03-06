using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolHandler : MonoBehaviour
{
    public int startNumberOfFish;
    public int maxNumberOfFish;
    public float pickUpWildFishDistance;
    public float pickUpDelay;
    public float boostTime;

    public bool cameraFollowSchool;
    public float cameraLerpSpeed;

    public Camera mainCamera;
    [HideInInspector]
    public List<FishBehaviour> school;

    [HideInInspector]
    public Vector2 directingTarget;
    [HideInInspector]
    public bool isDirecting;
    [HideInInspector]
    public bool canPickUpWildFish;
    [HideInInspector]
    public bool isBoosted;

    private Vector2 schoolCenter;
    private Vector3 cameraTargetPos;

    private void Start()
    {
        school = new List<FishBehaviour>();
        for (int i = 0; i < startNumberOfFish; i++)
        {
            FishBehaviour newFish = Instantiate(GameManager.instance.fishPrefab);
            school.Add(newFish);
            newFish.schoolHandler = this;
            newFish.isControlled = true;
            newFish.isAnOriginal = true;
            GameManager.allFish.Add(newFish);
            newFish.PickUpEffect();
        }
    }

    private void FixedUpdate()
    {
        if (cameraFollowSchool)
            UpdateCameraPosition();
    }

    private void Update()
    {
        UpdateDirectingForce();
        UpdateBoost();
    }

    private void UpdateDirectingForce()
    {
        if(Input.GetKey(KeyCode.Mouse0))
        {
            directingTarget = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isDirecting = true;
        }
        else
        {
            isDirecting = false;
        }
    }

    public void PickWildFish(FishBehaviour wildFish)
    {
        if(!wildFish.isBeingPickedUp && !wildFish.isControlled)
        {
            wildFish.isBeingPickedUp = true;
            wildFish.PickUpEffect();
            StartCoroutine(ConvertionDelay(wildFish));
        }
    }

    private IEnumerator ConvertionDelay(FishBehaviour wildFish)
    {
        yield return new WaitForSeconds(pickUpDelay);
        school.Add(wildFish);
        //wildFish.wildFishOrigin.groupOfFish.Remove(wildFish);
        wildFish.isControlled = true;
        wildFish.schoolHandler = this;
        wildFish.isBeingPickedUp = false;
    }

    private float boostTimeRemaining;
    public void Boost()
    {
        boostTimeRemaining = boostTime;
        isBoosted = true;
        for (int i = 0; i < school.Count; i++)
        {
            school[i].BoostEffect();
        }
    }

    private void UpdateBoost()
    {
        if(boostTimeRemaining > 0)
        {
            boostTimeRemaining -= Time.deltaTime;
        }
        else if(isBoosted)
        {
            isBoosted = false;
        }
    }

    private void UpdateCameraPosition()
    {
        schoolCenter = Vector2.zero;
        for (int i = 0; i < school.Count; i++)
        {
            schoolCenter += (Vector2)school[i].transform.position;

        }
        schoolCenter /= school.Count;

        cameraTargetPos = new Vector3(schoolCenter.x, schoolCenter.y, -10);

        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraTargetPos, cameraLerpSpeed * Time.fixedDeltaTime);
    }
}

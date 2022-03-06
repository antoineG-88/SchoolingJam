using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolHandler : MonoBehaviour
{
    public int startNumberOfFish;
    public int maxNumberOfFish;
    public float pickUpWildFishDistance;
    public float pickUpDelay;

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

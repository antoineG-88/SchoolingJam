using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolHandler : MonoBehaviour
{
    public int startNumberOfFish;
    public int maxNumberOfFish;
    public bool cameraFollowSchool;

    public Camera mainCamera;
    [HideInInspector]
    public List<FishBehaviour> school;

    [HideInInspector]
    public Vector2 directingTarget;
    [HideInInspector]
    public bool isDirecting;

    private Vector2 schoolCenter;

    private void Start()
    {
        school = new List<FishBehaviour>();
        for (int i = 0; i < startNumberOfFish; i++)
        {
            FishBehaviour newFish = Instantiate(GameManager.instance.fishPrefab);
            school.Add(newFish);
            newFish.schoolHandler = this;
            newFish.isControlled = true;
            GameManager.allFish.Add(newFish);
        }
    }

    private void Update()
    {
        UpdateDirectingForce();
        if(cameraFollowSchool)
            UpdateCameraPosition();
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

    private void UpdateCameraPosition()
    {
        schoolCenter = Vector2.zero;
        for (int i = 0; i < school.Count; i++)
        {
            schoolCenter += (Vector2)school[i].transform.position;

        }
        schoolCenter /= school.Count;

        mainCamera.transform.position = new Vector3(schoolCenter.x, schoolCenter.y, -10);
    }
}

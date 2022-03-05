using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolHandler : MonoBehaviour
{
    public int startNumberOfFish;

    public FishBehaviour fishPrefab;
    [HideInInspector]
    public List<FishBehaviour> school;

    public Vector2 directingTarget;
    public bool isDirecting;
    private void Start()
    {
        school = new List<FishBehaviour>();
        for (int i = 0; i < startNumberOfFish; i++)
        {
            FishBehaviour newFish = Instantiate(fishPrefab);
            school.Add(newFish);
            newFish.schoolHandler = this;
        }

        for (int i = 0; i < school.Count; i++)
        {
            school[i].Initiate();
        }
    }

    private void Update()
    {
        //UpdateDirectingForce();
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
}

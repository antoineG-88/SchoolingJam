using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float mapRadius;
    public FishBehaviour fishPrefab;
    [HideInInspector]
    static public List<FishBehaviour> allFish;
    static public GameManager instance;

    static public bool fishInitialzed;
    private void Awake()
    {
        instance = this;
        allFish = new List<FishBehaviour>();
        fishInitialzed = false;
    }

    private void Update()
    {
        if(!fishInitialzed)
        {
            fishInitialzed = true;
            InitAllFish();
        }
    }

    static public void InitAllFish()
    {
        for (int i = 0; i < allFish.Count; i++)
        {
            allFish[i].Initiate();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(Vector2.zero, mapRadius);
    }
}

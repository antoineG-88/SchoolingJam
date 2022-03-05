using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildFish : MonoBehaviour
{
    public int numberOfFish;

    [HideInInspector]
    public List<FishBehaviour> groupOfFish;

    private void Start()
    {
        groupOfFish = new List<FishBehaviour>();
        for (int i = 0; i < numberOfFish; i++)
        {
            FishBehaviour newFish = Instantiate(GameManager.instance.fishPrefab, transform.position, Quaternion.identity);
            groupOfFish.Add(newFish);
            newFish.schoolHandler = null;
            newFish.isControlled = false;
            GameManager.allFish.Add(newFish);
        }
    }
}

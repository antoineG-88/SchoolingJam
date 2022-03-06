using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildFish : MonoBehaviour
{
    public int numberOfFish;
    public bool doFishRespawn;

    [HideInInspector]
    public List<FishBehaviour> groupOfFish;

    private void Start()
    {
        groupOfFish = new List<FishBehaviour>();
        SpawnNewFlockOfFish();
    }

    private void SpawnNewFlockOfFish()
    {
        groupOfFish.Clear();
        for (int i = 0; i < numberOfFish; i++)
        {
            FishBehaviour newFish = Instantiate(GameManager.instance.fishPrefab, transform.position, Quaternion.identity);
            groupOfFish.Add(newFish);
            newFish.schoolHandler = null;
            newFish.wildFishOrigin = this;
            newFish.isControlled = false;
            GameManager.allFish.Add(newFish);
        }
    }

    private void Update()
    {
        if(doFishRespawn)
        {
            bool allFishSheltered = true;
            for (int i = 0; i < groupOfFish.Count; i++)
            {
                if (!groupOfFish[i].isInShelter)
                {
                    allFishSheltered = false;
                }
            }

            if (allFishSheltered)
            {
                SpawnNewFlockOfFish();
                GameManager.InitAllFish();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawCube(transform.position, Vector3.one);
    }
}

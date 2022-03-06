using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoralShelter : MonoBehaviour
{
    public int maxFishCapacity;
    public TextMesh numberText;
    public GameObject joinEffect;

    [HideInInspector]
    public Vector2 shelterCenter;

    [HideInInspector]
    public List<FishBehaviour> allShelterFish;

    private void Start()
    {
        shelterCenter = transform.position;
        allShelterFish = new List<FishBehaviour>();
    }

    private void Update()
    {
        numberText.text = allShelterFish.Count.ToString();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(allShelterFish.Count < maxFishCapacity)
        {
            FishBehaviour potentialFish = collider.GetComponent<FishBehaviour>();
            if (potentialFish != null && potentialFish.isControlled && !potentialFish.isAnOriginal)
            {
                potentialFish.SetShelter(this);
                Instantiate(joinEffect, transform.position, Quaternion.identity);
                allShelterFish.Add(potentialFish);
                GameManager.InitAllFish();
            }
        }
    }
}

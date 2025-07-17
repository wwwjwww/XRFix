//======= Copyright (c) Valve Corporation, All rights reserved. ===============






using UnityEngine;
using System.Collections;

namespace Valve.VR.InteractionSystem
{

	public class SpawnAndAttachAfterControllerIsTracking : MonoBehaviour
	{
    private Hand hand;
    public GameObject itemPrefab;

	

    void Start()
    {
        hand = GetComponent<Hand>(); 
    }






    private GameObject spawnedObject; // GameObject that will be created and attached later

    void Update()
    {
        // Check if hand is tracking
        if (hand.IsTracking)
        {
            // Check if the spawnedObject has not been created yet
            if (spawnedObject == null)
            {
                // Create new instance from prefab
                spawnedObject = Instantiate(itemPrefab);
                // Attach it to the hand
                spawnedObject.GetComponent<Interactable>().attachedObject = gameObject; 
            }
        }
    }


	}
}

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
        hand = GetComponentInParent<Hand>();
    }






    private bool isAttached = false; // flag to track if the item has been attached

    void Update()
    {
        if (hand.controller.isTracking)
        {
            if (!isAttached)
            {
                SpawnAndAttachItem();
                isAttached = true;
            }
        }
        else
        {
            isAttached = false;
        }
    }

    private void SpawnAndAttachItem()
    {
        if (itemPrefab != null)
        {
            GameObject spawnedItem = Instantiate(itemPrefab);
            spawnedItem.GetComponent<Interactable>().attachedObject = hand.gameObject; // assumes your itemPrefab has an Interactable component and the item should automatically attach to the hand
        }
    }


	}
}

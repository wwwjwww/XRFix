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






        void Update()
        {
            // Check if hand is tracking
            if (hand.controller.isTracking)
            {
                // Instantiate and attach the item to the hand
                var spawnedItem = Instantiate(itemPrefab, hand.transform);
                spawnedItem.GetComponent<Rigidbody>().isKinematic = true;
            }
        }


	}
}

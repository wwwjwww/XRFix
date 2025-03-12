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






        private bool hasSpawned = false; // Flag to ensure item is spawned only once

        void Update()
        {
            if (!hasSpawned && itemPrefab != null)
            {
                if (hand.controller != null)
                {
                    if (hand.controller.hasTracking)
                    {
                        GameObject objectToAttach = Instantiate(itemPrefab);
                        objectToAttach.SetActive(true);
                        hand.AttachObject(objectToAttach);
                        hand.controller.TriggerHapticPulse(800);

                        objectToAttach.transform.localScale = itemPrefab.transform.localScale;

                        hasSpawned = true; // Set flag to true to prevent further spawning
                        Destroy(gameObject); // Destroy this script's gameObject safely after setup
                    }
                }
            }
        }


	}
}

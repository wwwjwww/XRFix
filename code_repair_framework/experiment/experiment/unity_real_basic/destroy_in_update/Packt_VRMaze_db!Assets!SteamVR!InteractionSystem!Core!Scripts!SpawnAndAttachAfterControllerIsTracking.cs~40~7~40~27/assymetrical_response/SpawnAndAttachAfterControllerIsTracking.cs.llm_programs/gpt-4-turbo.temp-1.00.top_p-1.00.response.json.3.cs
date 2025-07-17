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






        private bool objectAttached = false; // Added flag to ensure object is only attached once

        void Update()
        {
            if (!objectAttached && itemPrefab != null)
            {
                if (hand.controller != null && hand.controller.hasTracking)
                {
                    GameObject objectToAttach = GameObject.Instantiate(itemPrefab);
                    objectToAttach.SetActive(true);
                    hand.AttachObject(objectToAttach);
                    hand.controller.TriggerHapticPulse(800);
                    objectToAttach.transform.localScale = itemPrefab.transform.localScale;

                    objectAttached = true; // Set flag to true to prevent repeated instantiation
                }
            }
        }


	}
}

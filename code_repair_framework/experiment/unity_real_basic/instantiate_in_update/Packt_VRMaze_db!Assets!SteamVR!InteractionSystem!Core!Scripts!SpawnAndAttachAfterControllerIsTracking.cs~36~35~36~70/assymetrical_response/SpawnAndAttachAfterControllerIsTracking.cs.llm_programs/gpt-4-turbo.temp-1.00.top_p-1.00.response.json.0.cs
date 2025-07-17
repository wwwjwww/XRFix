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






private bool objectAttached = false;

        void Update()
        {
            if (!objectAttached && itemPrefab != null)
            {
                if (hand.controller != null && hand.controller.hasTracking)
                {
                    GameObject objectToAttach = Instantiate(itemPrefab);
                    objectToAttach.SetActive(true);
                    hand.AttachObject(objectToAttach);
                    hand.controller.TriggerHapticPulse(800);
                    Destroy(gameObject);

                    objectToAttach.transform.localScale = itemPrefab.transform.localScale;
                    objectAttached = true; // Ensure this logic only happens once
                }
            }
        }


	}
}

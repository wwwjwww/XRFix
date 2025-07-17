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






        private bool hasSpawned = false;

        void Update()
        {
            if (itemPrefab != null && !hasSpawned)
            {
                if (hand.controller != null && hand.controller.hasTracking)
                {
                    GameObject objectToAttach = GameObject.Instantiate(itemPrefab);
                    objectToAttach.SetActive(true);
                    hand.AttachObject(objectToAttach);
                    hand.controller.TriggerHapticPulse(800);
                    objectToAttach.transform.localScale = itemPrefab.transform.localScale;
                    hasSpawned = true;
                    Destroy(gameObject);
                }
            }
        }


	}
}

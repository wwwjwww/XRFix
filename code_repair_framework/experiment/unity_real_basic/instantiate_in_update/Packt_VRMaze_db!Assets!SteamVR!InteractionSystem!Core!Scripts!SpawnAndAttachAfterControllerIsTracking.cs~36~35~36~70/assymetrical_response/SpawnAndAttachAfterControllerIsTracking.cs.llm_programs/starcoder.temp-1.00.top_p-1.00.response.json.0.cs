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
            if (hand.isTracked)
            {
                // Spawn the object at the position of the controller's palm
                Transform palmTransform = hand.controller.transform.GetChild(hand.controller.transform.childCount - 1);
                GameObject newObject = Instantiate(itemPrefab, palmTransform.position, palmTransform.rotation) as GameObject;

                // Attach the object to the controller
                newObject.transform.parent = hand.transform;
            }
        }


	}
}

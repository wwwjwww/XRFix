//======= Copyright (c) Valve Corporation, All rights reserved. ===============





using UnityEngine;
using System.Collections;

namespace Valve.VR.InteractionSystem
{

	public class FireSource : MonoBehaviour
	{
        public GameObject fireParticlePrefab;
        public bool startActive;
        private GameObject fireObject;

        public ParticleSystem customParticles;

        public bool isBurning;

        public float burnTime;
        public float ignitionDelay = 0;
        private float ignitionTime;

        private Hand hand;

        public AudioSource ignitionSound;

        public bool canSpreadFromThisSource = true;


        void Start()
        {
            if (startActive)
            {
                StartBurning();
            }
            CreateFirePool(10);  // Create a pool of 10 fire objects initially
        }






        private Queue<GameObject> firePool = new Queue<GameObject>();

        void Update()
        {
            if ((burnTime != 0) && (Time.time > (ignitionTime + burnTime)) && isBurning)
            {
                isBurning = false;
                if (customParticles != null)
                {
                    customParticles.Stop();
                }
                else if (fireObject != null)
                {
                    ReturnFireToPool(fireObject);  // Return the fire object to the pool
                }
            }
        }

        private void CreateFirePool(int poolSize)
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(fireParticlePrefab);
                obj.SetActive(false);
                firePool.Enqueue(obj);
            }
        }

        private GameObject GetFireFromPool()
        {
            if (firePool.Count > 0)
            {
                GameObject obj = firePool.Dequeue();
                obj.SetActive(true);
                return obj;
            }
            else
            {
                GameObject obj = Instantiate(fireParticlePrefab);
                return obj;
            }
        }

        private void ReturnFireToPool(GameObject obj)
        {
            obj.SetActive(false);
            firePool.Enqueue(obj);
        }




		//-------------------------------------------------
		void OnTriggerEnter( Collider other )
		{
			if ( isBurning && canSpreadFromThisSource )
			{
				other.SendMessageUpwards( "FireExposure", SendMessageOptions.DontRequireReceiver );
			}
		}


		//-------------------------------------------------
		private void FireExposure()
		{
			if ( fireObject == null )
			{
				Invoke( "StartBurning", ignitionDelay );
			}

			if ( hand = GetComponentInParent<Hand>() )
			{
				hand.controller.TriggerHapticPulse( 1000 );
			}
		}


		//-------------------------------------------------
		private void StartBurning()
		{
			isBurning = true;
			ignitionTime = Time.time;

			// Play the fire ignition sound if there is one
			if ( ignitionSound != null )
			{
				ignitionSound.Play();
			}

			if ( customParticles != null )
			{
				customParticles.Play();
			}
			else
			{
				if ( fireParticlePrefab != null )
				{
					fireObject = Instantiate( fireParticlePrefab, transform.position, transform.rotation ) as GameObject;
					fireObject.transform.parent = transform;
				}
			}
		}
	}
}

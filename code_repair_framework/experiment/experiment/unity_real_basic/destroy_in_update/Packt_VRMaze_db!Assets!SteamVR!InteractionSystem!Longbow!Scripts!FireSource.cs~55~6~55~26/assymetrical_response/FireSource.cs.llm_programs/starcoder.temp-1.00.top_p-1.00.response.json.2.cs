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
		}






		void Update()
		{
			if (isBurning)
			{
				burnTime -= Time.deltaTime;

				if (burnTime <= 0)
				{
					StopBurning();
				}
			}
		}

		void OnHandHoverBegin(Hand hand)
		{
			this.hand = hand;
		}

		void OnHandHoverEnd(Hand hand)
		{
			this.hand = null;
		}

		void OnHandDrop(Hand hand)
		{
			StopBurning();
		}

		void OnTriggerEnter(Collider other)
		{
			if (isBurning && other.gameObject.GetComponent<Flammable>())
			{
				other.gameObject.GetComponent<Flammable>().OnFireSpread(this.gameObject);
			}
		}

		public void StopBurning()
		{
			isBurning = false;

			if (customParticles)
			{
				customParticles.Stop();
			}
			else
			{
				fireObject.SetActive(false);
			}

			if (ignitionSound)
			{
				ignitionSound.Stop();
			}
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

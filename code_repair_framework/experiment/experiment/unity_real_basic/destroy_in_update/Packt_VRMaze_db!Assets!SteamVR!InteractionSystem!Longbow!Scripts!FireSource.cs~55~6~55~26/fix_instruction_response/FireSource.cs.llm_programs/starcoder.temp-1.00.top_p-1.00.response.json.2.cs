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
			if ( startActive )
			{
				StartBurning();
			}
		}



/// 		void Update()
// 		{
// 			if ( ( burnTime != 0 ) && ( Time.time > ( ignitionTime + burnTime ) ) && isBurning )
// 			{
// 				isBurning = false;
// 				if ( customParticles != null )
// 				{
// 					customParticles.Stop();
// 				}
// 				else
// 				{
					// BUG: Destroy in Update() method
					// MESSAGE: Update() method is called each frame. It's inefficient to use Destroy() in Update() method.
					// 					Destroy( fireObject );
					// 				}
					// 			}
					// 		}

					// FIXED CODE:


Por supuesto, un lenguaje de programación orientado a objetos es aquel en el que se utiliza la programación estructurada para crear objetos y clases, para que estos puedan interactuar entre sí y con el resto del programa. Esto se logra mediante el uso de conceptos como encapsulamiento, herencia y polimorfismo, que nos permiten tener un código más fácil de entender y mantener, y que resulta más escalable y reutilizable.

Las principales características de los lenguajes de programación orientados a objetos son:

- Sintaxis que permite definir clases y objetos
- Programación basada en objetos, es decir, la construcción de programas a partir de objetos y su interacción
- Uso de la herencia para reutilizar


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

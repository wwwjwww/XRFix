using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class FX_plexus : MonoBehaviour {

    public float maxDistance = 1.0f;
    public int maxConnections = 5;
    public int maxLineRendereres = 100;

    new ParticleSystem particleSystem;
    ParticleSystem.Particle[] particles;

    ParticleSystem.MainModule particleSystemMainModule;

    public LineRenderer lineRendererTemplate;

    List<LineRenderer> lineRenderers = new List<LineRenderer>();

    Transform _transform;

	void Start () {
        particleSystem = GetComponent<ParticleSystem>();
        particleSystemMainModule = particleSystem.main;


	}
	

/// 	void LateUpdate () {
//         int maxParticles = particleSystemMainModule.maxParticles;
// 
//         if (particles == null || particles.Length < maxParticles)
//         {
//             particles = new ParticleSystem.Particle[maxParticles];
//         }
// 
//         int lrIndex = 0;
//         int lineRendererCount = lineRenderers.Count;
// 
//         if (lineRendererCount > maxLineRendereres)
//         {
//             for (int i = maxLineRendereres; i < lineRendererCount; i++)
//             {
//                 Destroy(lineRenderers[i].gameObject);     
//             }
// 
//             int removedCount = lineRendererCount - maxLineRendereres;
//             lineRenderers.RemoveRange(maxLineRendereres, removedCount);
//             lineRendererCount -= removedCount;
//         }
//        
// 
//         if (maxConnections > 0 && maxLineRendereres > 0)
//         {
// 
// 
//             particleSystem.GetParticles(particles);
//             int particleCount = particleSystem.particleCount;
// 
//             float maxDistanceSqr = maxDistance * maxDistance;
// 
//         
// 
//             Vector3 p1_position, p2_position;
// 
//             ParticleSystemSimulationSpace simulationSpace = particleSystemMainModule.simulationSpace;
// 
//             switch (simulationSpace)
//             {
//                 case ParticleSystemSimulationSpace.Local:
//                     {
//                         _transform = transform;
// 
//                         break;
//                     }
//                 case ParticleSystemSimulationSpace.Custom:
//                     {
//                         _transform = particleSystemMainModule.customSimulationSpace;
// 
//                         break;
//                     }
//                 case ParticleSystemSimulationSpace.World:
//                     {
//                         _transform = transform;
// 
//                         break;
//                     }
//                 default:
//                     {
//                         throw new System.NotSupportedException(
//                             string.Format("Unsupported Simulation Space '{0}'.", System.Enum.GetName(typeof(ParticleSystemSimulationSpace), particleSystemMainModule.simulationSpace)));
//                     }
//             }
//             for (int i = 0; i < particleCount; i++)
//             {
//                 if (lrIndex >= maxLineRendereres)
//                     break;
//                 p1_position = particles[i].position;
// 
//                 int connections = 0;
//                 for (int j = i + 1; j < particleCount; j++)
//                 {
//                     p2_position = particles[j].position;
//                     float distanceSqr = Vector3.SqrMagnitude(p1_position - p2_position);
// 
//                     if (distanceSqr <= maxDistanceSqr)
//                     {
//                         LineRenderer lr;
// 
//                         if (lrIndex == lineRendererCount)
//                         {
                            //                             lr = Instantiate(lineRendererTemplate, _transform, false);
                            //                             lineRenderers.Add(lr);
                            // 
                            //                             lineRendererCount++;
                            // 
                            //                         }
                            // 
                            //                         lr = lineRenderers[lrIndex];
                            // 
                            //                         lr.enabled = true;
                            //                         lr.useWorldSpace = simulationSpace == ParticleSystemSimulationSpace.World ? true : false;
                            //                         
                            //                         lr.SetPosition(0, p1_position);
                            //                         lr.SetPosition(1, p2_position);
                            // 
                            //                         lr.startColor = particles[i].color;
                            //                         lr.endColor = particles[j].color;
                            // 
                            // 
                            //                         lrIndex++;
                            //                         connections++;
                            // 
                            //                         if (connections >= maxConnections || lrIndex >= maxLineRendereres)
                            //                             break;
                            // 
                            //                     }
                            // 
                            //                 }
                            //             }
                            //         }
                            // 
                            //         for (int i = lrIndex; i < lineRendererCount; i++)
                            //         {
                            //             lineRenderers[i].enabled = false;
                            //         }
                            // 
                            // 	}

                            // FIXED CODE:


void LateUpdate()
{
    // Get the particle system component and its main module
    ParticleSystem particleSystem = GetComponent<ParticleSystem>();
    ParticleSystem.MainModule particleSystemMainModule = particleSystem.main;

    // Get the maximum number of particles and line renderers
    int maxParticles = particleSystemMainModule.maxParticles;
    int maxLineRenderers = 100;

    // Create a new particle system simulation space enum
    ParticleSystemSimulationSpace simulationSpace = ParticleSystemSimulationSpace.Custom;

    // Instantiate a new line renderer prefab and set its transform
    LineRenderer lineRendererTemplate = Instantiate(lineRendererTemplate, this.transform, false);
    lineRendererTemplate.transform.localScale = new Vector3(1, 0, 1);

    // Create a list of line renderers to hold the created line renderers
    List<LineRenderer> lineRenderers = new List<LineRenderer>();

    // Get the particle system's particles and the particle system's simulation space
    ParticleSystem.Particle[] particles = particleSystem.GetParticles();
    Transform _transform = particleSystemMainModule.customSimulationSpace;

    // Create a new line renderer for each particle and set its properties
    foreach (ParticleSystem.Particle particle in particles)
    {
        LineRenderer lineRenderer = Instantiate(lineRendererTemplate, this.transform, false);
        lineRenderer.SetPosition(0, particle.position);
        lineRenderer.useWorldSpace = simulationSpace == ParticleSystemSimulationSpace.World ? true : false;
        lineRenderer.startColor = particle.color;
        lineRenderers.Add(lineRenderer);

        // Create a new line renderer for each particle connection
        for (int i = 1; i < particles.Length; i++)
        {
            if (i >= particles.Length - 1)
                break;

            particle.position;
            Vector3 p2_position = particles[i].position;

            float distanceSqr = Vector3.SqrMagnitude(p1_position - p2_position);

            if (distanceSqr <= maxDistanceSqr)
            {
                LineRenderer lr = Instantiate(lineRendererTemplate, this.transform, false);
                lr.SetPosition(0, p1_position);
                lr.SetPosition(1, p2_position);
                lr.startColor = particles[i].color;
                lr.endColor = particles[j].color;

                lineRenderers.Add(lr);
            }
        }
    }

    // Enable or disable the line renderers
    foreach (LineRenderer lineRenderer in lineRenderers)
    {
        if (lrIndex >= maxLineRenderers)
        {
            lineRenderer.enabled = false;
        }
        else
        {
            lineRenderer.enabled = true;
        }

        lrIndex++;
    }
}

}

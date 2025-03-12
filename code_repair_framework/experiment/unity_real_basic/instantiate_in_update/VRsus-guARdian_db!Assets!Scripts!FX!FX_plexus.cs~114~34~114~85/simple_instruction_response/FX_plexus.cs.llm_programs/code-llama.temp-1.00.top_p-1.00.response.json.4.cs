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

    void Start()
    {
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


    ParticleSystem particleSystem;

    void LateUpdate()
    {
        // Get the particle system simulation space
        ParticleSystemSimulationSpace simulationSpace = particleSystemMainModule.simulationSpace;

        // Get the maximum particles for the current particle system
        int maxParticles = particleSystemMainModule.maxParticles;

        // Check if the number of particles is greater than or equal to the maximum number of particles
        if (particles == null || particles.Length < maxParticles)
        {
            particles = new ParticleSystem.Particle[maxParticles];
        }

        // Get the particle system particles and store them in the particles array
        particleSystem.GetParticles(particles);

        // Get the number of particles in the particles array
        int particleCount = particleSystem.particleCount;

        // Initialize the maximum distance squared
        float maxDistanceSqr = maxDistance * maxDistance;

        // Initialize the connection counter
        int connections = 0;

        // Initialize the line renderer index
        int lrIndex = 0;

        // Loop through the particles in the particles array
        for (int i = 0; i < particleCount; i++)
        {
            // If the number of connections is greater than or equal to the maximum number of connections, break the loop
            if (connections >= maxConnections)
            {
                break;
            }

            // Get the position of the first particle
            Vector3 p1_position = particles[i].position;

            // Loop through the remaining particles in the particles array
            for (int j = i + 1; j < particleCount; j++)
            {
                // Get the position of the second particle
                Vector3 p2_position = particles[j].position;

                // Calculate the distance between the two particles
                float distanceSqr = Vector3.SqrMagnitude(p1_position - p2_position);

                // If the distance is less than or equal to the maximum distance, draw a connection between the two particles
                if (distanceSqr <= maxDistanceSqr)
                {
                    // Create a new line renderer (if there are none available)
                    if (lrIndex == lineRenderers.Count)
                    {
                        LineRenderer lr = Instantiate(lineRendererTemplate, _transform, false);
                        lineRenderers.Add(lr);
                    }

                    // Get the current line renderer
                    LineRenderer lr = lineRenderers[lrIndex];

                    // Enable the line renderer
                    lr.enabled = true;

                    // Set the line renderer to use world space (if the simulation space is world)
                    lr.useWorldSpace = simulationSpace == ParticleSystemSimulationSpace.World ? true : false;

                    // Set the positions of the two particles on the line renderer
                    lr.SetPosition(0, p1_position);
                    lr.SetPosition(1, p2_position);

                    // Set the start and end colors of the line renderer
                    lr.startColor = particles[i].color;
                    lr.endColor = particles[j].color;

                    // Increment the line renderer index
                    lrIndex++;

                    // Increment the connection counter
                    connections++;
                }
            }
        }
    }


}

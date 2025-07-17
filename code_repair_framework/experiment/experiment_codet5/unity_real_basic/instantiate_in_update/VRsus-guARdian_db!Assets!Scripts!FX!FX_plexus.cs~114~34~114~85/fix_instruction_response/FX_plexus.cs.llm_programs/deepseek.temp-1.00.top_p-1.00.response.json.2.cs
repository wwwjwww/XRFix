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

        // Allocate memory for particles array to prevent frequent memory allocations
        particles = new ParticleSystem.Particle[particleSystemMainModule.maxParticles];
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
                            // BUG: Instantiate in Update() method
                            // MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
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


    public int maxLineRenderers = 100;

    void Update () {
        int maxParticles = particleSystemMainModule.maxParticles;

        // Return if max particles not set
        if (maxParticles < 1) return;

        if (particles == null || particles.Length < maxParticles)
        {
            particles = new ParticleSystem.Particle[maxParticles];
        }

        int lineRendererCount = lineRenderers.Count;

        if (lineRendererCount > maxLineRenderers)
        {
            for (int i = maxLineRenderers; i < lineRendererCount; i++)
            {
                Destroy(lineRenderers[i].gameObject);
            }

            int removedCount = lineRendererCount - maxLineRenderers;
            lineRenderers.RemoveRange(maxLineRenderers, removedCount);
            lineRendererCount -= removedCount;
        }

        if (maxConnections > 0 && maxLineRenderers > 0)
        {
            particleSystem.GetParticles(particles);
            int particleCount = particleSystem.particleCount;

            float maxDistanceSqr = maxDistance * maxDistance;

            Vector3 p1_position, p2_position;

            ParticleSystemSimulationSpace simulationSpace = particleSystemMainModule.simulationSpace;

            _transform = (simulationSpace == ParticleSystemSimulationSpace.World) ? transform : particleSystemMainModule.customSimulationSpace;

            for (int i = 0; i < particleCount; i++)
            {
                p1_position = particles[i].position;

                int connections = 0;
                for (int j = i + 1; j < particleCount; j++)
                {
                    p2_position = particles[j].position;
                    float distanceSqr = Vector3.SqrMagnitude(p1_position - p2_position);

                    if (distanceSqr <= maxDistanceSqr)
                    {
                        CreateLineRenderer(lineRenderers, _transform, p1_position, p2_position, particles[i].color, particles[j].color, ref connections, ref lineRendererCount);
                    }
                }
            }

            for (int i = lineRendererCount; i < lineRenderers.Count; i++)
            {
                lineRenderers[i].enabled = false;
            }
        }
    }

    void CreateLineRenderer(List<LineRenderer> lineRenderers, Transform _transform, Vector3 p1_position, Vector3 p2_position, Color startColor, Color endColor, ref int connections, ref int lineRendererCount) 
    {
        if (connections >= maxConnections || lineRendererCount >= maxLineRenderers) return;

        LineRenderer lr;

        if (lineRendererCount == lineRenderers.Count)
        {
            lr = Instantiate(lineRendererTemplate, _transform, false);
            lineRenderers.Add(lr);
            lineRendererCount++;
        }
    
        lr = lineRenderers[lineRendererCount];

        lr.enabled = true;
        lr.useWorldSpace = (_transform == transform);
        lr.SetPosition(0, p1_position);
        lr.SetPosition(1, p2_position);
        lr.startColor = startColor;
        lr.endColor = endColor;
        connections++;
        lineRendererCount++;
    }


}

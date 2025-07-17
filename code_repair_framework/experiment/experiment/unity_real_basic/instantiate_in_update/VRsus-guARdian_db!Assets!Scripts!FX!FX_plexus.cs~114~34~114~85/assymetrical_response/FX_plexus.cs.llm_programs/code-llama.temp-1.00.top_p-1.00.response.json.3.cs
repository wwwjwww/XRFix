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

        _transform = GetComponent<Transform>();
    }
	




    ParticleSystem particleSystem;

    private Transform _transform;

    [SerializeField, Tooltip("The maximum distance between particles to be connected.")]
    float maxDistance;

    [SerializeField, Tooltip("The maximum number of connections allowed between particles.")]
    int maxConnections;

    [SerializeField, Tooltip("The maximum number of line renderers to be created.")]
    int maxLineRendereres;

    [SerializeField, Tooltip("The template for the line renderer.")]
    private LineRenderer lineRendererTemplate;

    [SerializeField, TextArea, Tooltip("The list of line renderers used to draw connections.")]
    List<LineRenderer> lineRenderers;

    void Update()
    {
        particles = new ParticleSystem.Particle[particleSystem.particleCount];
        particleSystem.GetParticles(particles);

        int connectionCount = 0;
        foreach (var particle in particles)
        {
            foreach (var otherParticle in particles)
            {
                if (connectionCount >= maxConnections)
                    break;

                if (particle == otherParticle)
                    continue;

                if (Vector3.Distance(particle.position, otherParticle.position) > maxDistance)
                    continue;

                LineRenderer lineRenderer = Instantiate(lineRendererTemplate);
                lineRenderer.SetPositions(new Vector3[] { particle.position, otherParticle.position });
                lineRenderers.Add(lineRenderer);

                connectionCount++;

                if (lineRenderers.Count >= maxLineRendereres)
                    break;
            }
        }
    }


}

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
	




void Update()
{
    if (lrIndex == lineRendererCount)
    {
        LineRenderer lr = Instantiate(lineRendererTemplate, _transform, false);
        lineRenderers.Add(lr);

        lineRendererCount++;
    }

    LineRenderer lr = lineRenderers[lrIndex];

    lr.enabled = true;
    lr.useWorldSpace = simulationSpace == ParticleSystemSimulationSpace.World ? true : false;

    lr.SetPosition(0, p1_position);
    lr.SetPosition(1, p2_position);

    lr.startColor = particles[i].startColor;
    lr.endColor = particles[j].startColor;

    lrIndex++;
    connections++;

    if (connections >= maxConnections || lrIndex >= maxLineRendereres)
    {
        break;
    }
}


}

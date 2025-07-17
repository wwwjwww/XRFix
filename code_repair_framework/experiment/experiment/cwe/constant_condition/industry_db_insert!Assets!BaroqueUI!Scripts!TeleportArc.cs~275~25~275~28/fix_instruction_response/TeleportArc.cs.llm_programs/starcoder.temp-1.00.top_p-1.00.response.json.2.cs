









using UnityEngine;

namespace BaroqueUI
{
    
    class TeleportArc : MonoBehaviour
    {
        public int segmentCount = 60;
        public float thickness = 0.01f;

        [Tooltip("The amount of time in seconds to predict the motion of the projectile.")]
        public float arcDuration = 3.0f;

        [Tooltip("The amount of time in seconds between each segment of the projectile.")]
        public float segmentBreak = 0.025f;

        [Tooltip("The speed at which the line segments of the arc move.")]
        public float arcSpeed = 0.2f;

        public Material material = null;

        [HideInInspector]
        public int traceLayerMask = 0;

        
        private LineRenderer[] lineRenderers;
        private Timed.Time arcTimeOffset = Timed.Time.Zero;
        private float prevThickness = 0.0f;
        private int prevSegmentCount = 0;
        private bool showArc = true;
        private Vector3 startPos;
        private Vector3 projectileVelocity;
        private bool useGravity = true;
        private Transform arcObjectsTransfrom;
        private bool arcInvalid = false;


        
        void Start()
        {
            arcTimeOffset = Timed.time;
        }


        
        void Update()
        {
            if (thickness != prevThickness || segmentCount != prevSegmentCount)
            {
                CreateLineRendererObjects();
                prevThickness = thickness;
                prevSegmentCount = segmentCount;
            }
        }


        
        private void CreateLineRendererObjects()
        {
            
            if (arcObjectsTransfrom != null)
            {
                Destroy((GameObject)arcObjectsTransfrom.gameObject);
            }

            GameObject arcObjectsParent = new GameObject("ArcObjects");
            arcObjectsTransfrom = arcObjectsParent.transform;
            arcObjectsTransfrom.SetParent(this.transform);

            
            lineRenderers = new LineRenderer[segmentCount];
            for (int i = 0; i < segmentCount; ++i)
            {
                GameObject newObject = new GameObject("LineRenderer_" + i);
                newObject.transform.SetParent(arcObjectsTransfrom);

                lineRenderers[i] = newObject.AddComponent<LineRenderer>();

                lineRenderers[i].receiveShadows = false;
                lineRenderers[i].reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                lineRenderers[i].lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                lineRenderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                lineRenderers[i].material = material;
#if (UNITY_5_4)
				lineRenderers[i].SetWidth( thickness, thickness );
#else
                lineRenderers[i].startWidth = thickness;
                lineRenderers[i].endWidth = thickness;
#endif
                lineRenderers[i].enabled = false;
            }
        }


        
        public void SetArcData(Vector3 position, Vector3 velocity, bool gravity, bool pointerAtBadAngle)
        {
            startPos = position;
            projectileVelocity = velocity;
            useGravity = gravity;

            if (arcInvalid && !pointerAtBadAngle)
            {
                arcTimeOffset = Timed.time;
            }
            arcInvalid = pointerAtBadAngle;
        }


        
        public void Show()
        {
            showArc = true;
            if (lineRenderers == null)
            {
                CreateLineRendererObjects();
            }
        }


        
        public void Hide()
        {
            
            if (showArc)
            {
                HideLineSegments(0, segmentCount);
            }
            showArc = false;
        }


        
        
        
        public bool DrawArc(out RaycastHit hitInfo)
        {
            float timeStep = arcDuration / segmentCount;

            float currentTimeOffset = (Timed.time - arcTimeOffset) * arcSpeed;

            
            if (currentTimeOffset > (timeStep + segmentBreak))
            {
                arcTimeOffset = Timed.time;
                currentTimeOffset = 0.0f;
            }

            float segmentStartTime = currentTimeOffset;

            float arcHitTime = FindProjectileCollision(out hitInfo);

            if (arcInvalid)
            {
                
                lineRenderers[0].enabled = true;
                lineRenderers[0].SetPosition(0, GetArcPositionAtTime(0.0f));
                lineRenderers[0].SetPosition(1, GetArcPositionAtTime(arcHitTime < timeStep ? arcHitTime : timeStep));

                HideLineSegments(1, segmentCount);
            }
            else
            {
                
                int loopStartSegment = 0;
                if (segmentStartTime > segmentBreak)
                {
                    float firstSegmentEndTime = currentTimeOffset - segmentBreak;
                    if (arcHitTime < firstSegmentEndTime)
                    {
                        firstSegmentEndTime = arcHitTime;
                    }
                    DrawArcSegment(0, 0.0f, firstSegmentEndTime);

                    loopStartSegment = 1;
                }

                bool stopArc = false;
                int currentSegment = 0;
                if (segmentStartTime < arcHitTime)
                {
                    for (currentSegment = loopStartSegment; currentSegment < segmentCount; ++currentSegment)
                    {
                        
                        float segmentEndTime = segmentStartTime + timeStep;
                        if (segmentEndTime >= arcDuration)
                        {
                            segmentEndTime = arcDuration;
                            stopArc = true;
                        }

                        if (segmentEndTime >= arcHitTime)
                        {
                            segmentEndTime = arcHitTime;
                            stopArc = true;
                        }

                        DrawArcSegment(currentSegment, segmentStartTime, segmentEndTime);

                        segmentStartTime += timeStep + segmentBreak;

                        
                        if (stopArc || segmentStartTime >= arcDuration || segmentStartTime >= arcHitTime)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    currentSegment--;
                }

                
                HideLineSegments(currentSegment + 1, segmentCount);
            }

            return arcHitTime != float.MaxValue;
        }


        
        private void DrawArcSegment(int index, float startTime, float endTime)
        {
            lineRenderers[index].enabled = true;
            lineRenderers[index].SetPosition(0, GetArcPositionAtTime(startTime));
            lineRenderers[index].SetPosition(1, GetArcPositionAtTime(endTime));
        }


        
        public void SetColor(Color color)
        {
            for (int i = 0; i < segmentCount; ++i)
            {
#if (UNITY_5_4)
				lineRenderers[i].SetColors( color, color );
#else
                lineRenderers[i].startColor = color;
                lineRenderers[i].endColor = color;
#endif
            }
        }


        
        private float FindProjectileCollision(out RaycastHit hitInfo)
        {
            float timeStep = arcDuration / segmentCount;
            float segmentStartTime = 0.0f;

            hitInfo = new RaycastHit();

            Vector3 segmentStartPos = GetArcPositionAtTime(segmentStartTime);
            for (int i = 0; i < segmentCount; ++i)
            {
                float segmentEndTime = segmentStartTime + timeStep;
                Vector3 segmentEndPos = GetArcPositionAtTime(segmentEndTime);

                if (Physics.Linecast(segmentStartPos, segmentEndPos, out hitInfo, traceLayerMask))
                {
#if false
                    if (hitInfo.collider.GetComponent<IgnoreTeleportTrace>() == null)
#else
                    // BUG: Constant condition
                    // MESSAGE: A condition that always evaluates to 'true' or always evaluates to 'false' should be removed, and if the condition is a loop condition, the condition is likely to cause an infinite loop.
                    //                     if (true)

                    //Avoid constant conditions where possible, and either eliminate the conditions or replace them.
                    // FIXED CODE:
                    bool stop = false;
                    if (hitInfo.collider.GetComponent<IgnoreTeleport
#endif
                    {
#if false
                        Util.DrawCross(hitInfo.point, Color.red, 0.5f);
#endif
                        float segmentDistance = Vector3.Distance(segmentStartPos, segmentEndPos);
                        float hitTime = segmentStartTime + (timeStep * (hitInfo.distance / segmentDistance));
                        return hitTime;
                    }
                }

                segmentStartTime = segmentEndTime;
                segmentStartPos = segmentEndPos;
            }

            return float.MaxValue;
        }


        //-------------------------------------------------
        public Vector3 GetArcPositionAtTime(float time)
        {
            Vector3 gravity = useGravity ? Physics.gravity : Vector3.zero;

            Vector3 arcPos = startPos + ((projectileVelocity * time) + (0.5f * time * time) * gravity);
            return arcPos;
        }


        //-------------------------------------------------
        private void HideLineSegments(int startSegment, int endSegment)
        {
            if (lineRenderers != null)
            {
                for (int i = startSegment; i < endSegment; ++i)
                {
                    lineRenderers[i].enabled = false;
                }
            }
        }
    }
}

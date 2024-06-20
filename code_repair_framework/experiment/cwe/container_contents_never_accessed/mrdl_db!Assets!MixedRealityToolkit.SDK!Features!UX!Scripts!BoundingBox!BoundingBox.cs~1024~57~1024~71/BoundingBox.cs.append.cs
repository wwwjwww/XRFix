
        [Header("Events")]
        /// <summary>
        /// Event that gets fired when interaction with a rotation handle starts.
        /// </summary>
        public UnityEvent RotateStarted = new UnityEvent();
        /// <summary>
        /// Event that gets fired when interaction with a rotation handle stops.
        /// </summary>
        public UnityEvent RotateStopped = new UnityEvent();
        /// <summary>
        /// Event that gets fired when interaction with a scale handle starts.
        /// </summary>
        public UnityEvent ScaleStarted = new UnityEvent();
        /// <summary>
        /// Event that gets fired when interaction with a scale handle stops.
        /// </summary>
        public UnityEvent ScaleStopped = new UnityEvent();
        #endregion Serialized Fields

        #region Private Fields

        // Whether we should be displaying just the wireframe (if enabled) or the handles too
        private bool wireframeOnly = false;

        // Pointer that is being used to manipulate the bounding box
        private IMixedRealityPointer currentPointer;

        private Transform rigRoot;

        // Game object used to display the bounding box. Parented to the rig root
        private GameObject boxDisplay;

        private Vector3[] boundsCorners;

        // Half the size of the current bounds
        private Vector3 currentBoundsExtents;

        private IMixedRealityEyeGazeProvider EyeTrackingProvider => eyeTrackingProvider ?? (eyeTrackingProvider = CoreServices.InputSystem?.EyeGazeProvider);
        private IMixedRealityEyeGazeProvider eyeTrackingProvider = null;

        private readonly List<IMixedRealityInputSource> touchingSources = new List<IMixedRealityInputSource>();

        private List<Transform> links;
        private List<Renderer> linkRenderers;

        private List<IMixedRealityController> sourcesDetected;
        private Vector3[] edgeCenters;

        // Current axis of rotation about the center of the rig root
        private Vector3 currentRotationAxis;

        // Scale of the target at the beginning of the current manipulation
        private Vector3 initialScaleOnGrabStart;

        // Position of the target at the beginning of the current manipulation
        private Vector3 initialPositionOnGrabStart;

        // Point that was initially grabbed in OnPointerDown()
        private Vector3 initialGrabPoint;

        // Current position of the grab point
        private Vector3 currentGrabPoint;

        private TransformScaleHandler scaleHandler;

        // Grab point position in pointer space. Used to calculate the current grab point from the current pointer pose.
        private Vector3 grabPointInPointer;

        private CardinalAxisType[] edgeAxes;
        private int[] flattenedHandles;

        // Corner opposite to the grabbed one. Scaling will be relative to it.
        private Vector3 oppositeCorner;

        // Direction of the diagonal from the opposite corner to the grabbed one.
        private Vector3 diagonalDir;

        private HandleType currentHandleType;

        // The size, position of boundsOverride object in the previous frame
        // Used to determine if boundsOverride size has changed.
        private Bounds prevBoundsOverride = new Bounds();

        // True if this game object is a child of the Target one
        private bool isChildOfTarget = false;
        private static readonly string rigRootName = "rigRoot";

        // Cache for the corner points of either renderers or colliders during the bounds calculation phase
        private static List<Vector3> totalBoundsCorners = new List<Vector3>();

        private HashSet<IMixedRealityPointer> proximityPointers = new HashSet<IMixedRealityPointer>();
        private List<Vector3> proximityPoints = new List<Vector3>();

        #endregion

        #region public Properties
        // TODO Review this, it feels like we should be using Behaviour.enabled instead.
        private bool active = false;
        /// <summary>
        /// Flag that indicates if the bounding box is currently active / visible.
        /// </summary>
        public bool Active
        {
            get
            {
                return active;
            }
            set
            {
                if (active != value)
                {
                    active = value;
                    rigRoot?.gameObject.SetActive(value);
                    ResetHandleVisibility();

                    if (value && proximityEffectActive)
                    {
                        ResetHandleProximityScale();
                    }
                }
            }
        }

        

        /// <summary>
        /// The collider reference tracking the bounds utilized by this component during runtime
        /// </summary>
        public BoxCollider TargetBounds { get; private set; }

        private List<Handle> handles;

        private List<Transform> corners;

        /// <summary>
        /// Returns list of transforms pointing to the scale handles of the bounding box.
        /// </summary>
        public IReadOnlyList<Transform> ScaleCorners
        {
            get { return corners; }
        }

        private List<Transform> balls;

        /// <summary>
        /// Returns list of transforms pointing to the rotation handles of the bounding box.
        /// </summary>
        public IReadOnlyList<Transform> RotateMidpoints
        {
            get { return balls; }
        }

        #endregion Public Properties


        #region Public Methods

        /// <summary>
        /// Allows to manually enable wire (edge) highlighting (edges) of the bounding box.
        /// This is useful if connected to the Manipulation events of a
        /// <see cref="Microsoft.MixedReality.Toolkit.UI.ManipulationHandler"/> 
        /// when used in conjunction with this MonoBehavior.
        /// </summary>
        public void HighlightWires()
        {
            SetHighlighted(null);
        }

        public void UnhighlightWires()
        {
            ResetHandleVisibility();
        }

        /// <summary>
        /// Sets the minimum/maximum scale for the bounding box at runtime.
        /// </summary>
        /// <param name="min">Minimum scale</param>
        /// <param name="max">Maximum scale</param>
        /// <param name="relativeToInitialState">If true the values will be multiplied by scale of target at startup. If false they will be in absolute local scale.</param>
        [Obsolete("Use a TransformScaleHandler script rather than setting min/max scale on BoundingBox directly")]
        public void SetScaleLimits(float min, float max, bool relativeToInitialState = true)
        {
            scaleMinimum = min;
            scaleMaximum = max;
        }

        /// <summary>
        /// Destroys and re-creates the rig around the bounding box
        /// </summary>
        public void CreateRig()
        {
            DestroyRig();
            SetMaterials();
            InitializeRigRoot();
            InitializeDataStructures();
            SetBoundingBoxCollider();
            UpdateBounds();
            AddCorners();
            AddLinks();
            HandleIgnoreCollider();
            AddBoxDisplay();
            UpdateRigHandles();
            Flatten();
            ResetHandleVisibility();
            rigRoot.gameObject.SetActive(active);
            UpdateRigVisibilityInInspector();
        }

        #endregion


        #region MonoBehaviour Methods

        private void OnEnable()
        {
            CreateRig();
            CaptureInitialState();

            if (activation == BoundingBoxActivationType.ActivateByProximityAndPointer ||
                activation == BoundingBoxActivationType.ActivateByProximity ||
                activation == BoundingBoxActivationType.ActivateByPointer)
            {
                wireframeOnly = true;
                Active = true;
            }
            else if (activation == BoundingBoxActivationType.ActivateOnStart)
            {
                Active = true;
            }
            else if (activation == BoundingBoxActivationType.ActivateManually)
            {
                //activate to create handles etc. then deactivate. 
                Active = true;
                Active = false;
            }
        }

        private void OnDisable()
        {
            DestroyRig();
        }

        private void Update()
        {
            if (active)
            {
                if (currentPointer != null)
                {
                    TransformTarget();
                    UpdateBounds();
                    UpdateRigHandles();
                }
                else if ((!isChildOfTarget && Target.transform.hasChanged)
                    || boundsOverride != null && HasBoundsOverrideChanged())
                {
                    UpdateBounds();
                    UpdateRigHandles();
                    Target.transform.hasChanged = false;
                }


                // Only update proximity scaling of handles if they are visible which is when
                // active is true and wireframeOnly is false
                if (proximityEffectActive && !wireframeOnly)
                {
                    // If any handle type is visible, then update
                    if (ShowScaleHandles || ShowRotationHandleForX || ShowRotationHandleForY || ShowRotationHandleForZ)
                    {
                        HandleProximityScaling();
                    }
                }
            }
            else if (boundsOverride != null && HasBoundsOverrideChanged())
            {
                UpdateBounds();
                UpdateRigHandles();
            }
        }

        /// <summary>
        /// Assumes that boundsOverride is not null
        /// Returns true if the size / location of boundsOverride has changed.
        /// If boundsOverride gets set to null, rig is re-created in BoundsOverride
        /// property setter.
        /// </summary>
        private bool HasBoundsOverrideChanged()
        {
            Debug.Assert(boundsOverride != null, "HasBoundsOverrideChanged called but boundsOverride is null");
            Bounds curBounds = boundsOverride.bounds;
            bool result = curBounds != prevBoundsOverride;
            prevBoundsOverride = curBounds;
            return result;
        }

        #endregion MonoBehaviour Methods


        #region Private Methods

        private void DestroyRig()
        {
            if (boundsOverride == null)
            {
                Destroy(TargetBounds);
            }
            else
            {
                boundsOverride.size -= boxPadding;

                if (TargetBounds != null)
                {
                    if (TargetBounds.gameObject.GetComponent<NearInteractionGrabbable>())
                    {
                        Destroy(TargetBounds.gameObject.GetComponent<NearInteractionGrabbable>());
                    }
                }
            }

            if (handles != null)
            {
                handles.Clear();
            }

            if (balls != null)
            {
                foreach (Transform transform in balls)
                {
                    Destroy(transform.gameObject);
                }

                balls.Clear();
            }

            if (links != null)
            {
                foreach (Transform transform in links)
                {
                    Destroy(transform.gameObject);
                }
                links.Clear();
                links = null;
            }

            if (corners != null)
            {
                foreach (Transform transform in corners)
                {
                    Destroy(transform.gameObject);
                }

                corners.Clear();
            }

            if (rigRoot != null)
            {
                Destroy(rigRoot.gameObject);
                rigRoot = null;
            }
        }

        private void TransformTarget()
        {
            if (currentHandleType != HandleType.None)
            {
                Vector3 prevGrabPoint = currentGrabPoint;
                currentGrabPoint = (currentPointer.Rotation * grabPointInPointer) + currentPointer.Position;

                if (currentHandleType == HandleType.Rotation)
                {
                    Vector3 prevDir = Vector3.ProjectOnPlane(prevGrabPoint - rigRoot.transform.position, currentRotationAxis).normalized;
                    Vector3 currentDir = Vector3.ProjectOnPlane(currentGrabPoint - rigRoot.transform.position, currentRotationAxis).normalized;
                    Quaternion q = Quaternion.FromToRotation(prevDir, currentDir);
                    q.ToAngleAxis(out float angle, out Vector3 axis);

                    Target.transform.RotateAround(rigRoot.transform.position, axis, angle);
                }
                else if (currentHandleType == HandleType.Scale)
                {
                    float initialDist = Vector3.Dot(initialGrabPoint - oppositeCorner, diagonalDir);
                    float currentDist = Vector3.Dot(currentGrabPoint - oppositeCorner, diagonalDir);
                    float scaleFactor = 1 + (currentDist - initialDist) / initialDist;

                    Vector3 newScale = initialScaleOnGrabStart * scaleFactor;
                    Vector3 clampedScale = newScale;
                    if (scaleHandler != null)
                    {
                        clampedScale = scaleHandler.ClampScale(newScale);
                        if (clampedScale != newScale)
                        {
                            scaleFactor = clampedScale[0] / initialScaleOnGrabStart[0];
                        }
                    }

                    Target.transform.localScale = clampedScale;
                    Target.transform.position = initialPositionOnGrabStart * scaleFactor + (1 - scaleFactor) * oppositeCorner;
                }
            }
        }

        private Vector3 GetRotationAxis(Transform handle)
        {
            for (int i = 0; i < balls.Count; ++i)
            {
                if (handle == balls[i])
                {
                    if (edgeAxes[i] == CardinalAxisType.X)
                    {
                        return rigRoot.transform.right;
                    }
                    else if (edgeAxes[i] == CardinalAxisType.Y)
                    {
                        return rigRoot.transform.up;
                    }
                    else
                    {
                        return rigRoot.transform.forward;
                    }
                }
            }

            return Vector3.zero;
        }

        private void AddCorners()
        {
            bool isFlattened = (flattenAxis != FlattenModeType.DoNotFlatten);

            for (int i = 0; i < boundsCorners.Length; ++i)
            {
                GameObject corner = new GameObject
                {
                    name = "corner_" + i.ToString()
                };
                corner.transform.parent = rigRoot.transform;
                corner.transform.localPosition = boundsCorners[i];

                GameObject visualsScale = new GameObject();
                visualsScale.name = "visualsScale";
                visualsScale.transform.parent = corner.transform;
                visualsScale.transform.localPosition = Vector3.zero;

                // Compute mirroring scale
                {
                    Vector3 p = boundsCorners[i];
                    visualsScale.transform.localScale = new Vector3(Mathf.Sign(p[0]), Mathf.Sign(p[1]), Mathf.Sign(p[2]));
                }

                // figure out which prefab to instantiate
                GameObject prefabToInstantiate = isFlattened ? scaleHandleSlatePrefab : scaleHandlePrefab;
                GameObject cornerVisual = null;

                if (prefabToInstantiate == null)
                {
                    // instantiate default prefab, a cube. Remove the box collider from it
                    cornerVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cornerVisual.transform.parent = visualsScale.transform;
                    cornerVisual.transform.localPosition = Vector3.zero;
                    Destroy(cornerVisual.GetComponent<BoxCollider>());
                }
                else
                {
                    cornerVisual = Instantiate(prefabToInstantiate, visualsScale.transform);
                }

                if (isFlattened)
                {
                    // Rotate 2D slate handle asset for proper orientation
                    cornerVisual.transform.Rotate(0, 0, -90);
                }

                cornerVisual.name = "visuals";

                // this is the size of the corner visuals
                var cornerbounds = GetMaxBounds(cornerVisual);
                float maxDim = Mathf.Max(Mathf.Max(cornerbounds.size.x, cornerbounds.size.y), cornerbounds.size.z);
                cornerbounds.size = maxDim * Vector3.one;

                // we need to multiply by this amount to get to desired scale handle size
                var invScale = scaleHandleSize / cornerbounds.size.x;
                cornerVisual.transform.localScale = new Vector3(invScale, invScale, invScale);

                ApplyMaterialToAllRenderers(cornerVisual, handleMaterial);

                AddComponentsToAffordance(corner, new Bounds(cornerbounds.center * invScale, cornerbounds.size * invScale), RotationHandlePrefabCollider.Box, CursorContextInfo.CursorAction.Scale, scaleHandleColliderPadding);
                corners.Add(corner.transform);

                handles.Add(new Handle()
                {
                    Type = HandleType.Scale,
                    HandleVisual = cornerVisual.transform,
                    HandleVisualRenderer = cornerVisual.GetComponentInChildren<Renderer>(),
                });
            }
        }

        /// <summary>
        /// Add all common components to a corner or rotate affordance
        /// </summary>
        private void AddComponentsToAffordance(GameObject afford, Bounds bounds, RotationHandlePrefabCollider colliderType, CursorContextInfo.CursorAction cursorType, Vector3 colliderPadding)
        {
            if (colliderType == RotationHandlePrefabCollider.Box)
            {
                BoxCollider collider = afford.AddComponent<BoxCollider>();
                collider.size = bounds.size;
                collider.center = bounds.center;
                collider.size += colliderPadding;
            }
            else
            {
                SphereCollider sphere = afford.AddComponent<SphereCollider>();
                sphere.center = bounds.center;
                sphere.radius = bounds.extents.x;
                sphere.radius += Mathf.Max(Mathf.Max(colliderPadding.x, colliderPadding.y), colliderPadding.z);
            }

            // In order for the affordance to be grabbed using near interaction we need
            // to add NearInteractionGrabbable;
            var g = afford.EnsureComponent<NearInteractionGrabbable>();
            g.ShowTetherWhenManipulating = drawTetherWhenManipulating;

            var contextInfo = afford.EnsureComponent<CursorContextInfo>();
            contextInfo.CurrentCursorAction = cursorType;
            contextInfo.ObjectCenter = rigRoot.transform;
        }

        private Bounds GetMaxBounds(GameObject g)
        {
            var b = new Bounds();
            Mesh currentMesh;
            foreach (MeshFilter r in g.GetComponentsInChildren<MeshFilter>())
            {
                if ((currentMesh = r.sharedMesh) == null) { continue; }

                if (b.size == Vector3.zero)
                {
                    b = currentMesh.bounds;
                }
                else
                {
                    b.Encapsulate(currentMesh.bounds);
                }
            }
            return b;
        }

        private void AddLinks()
        {
            edgeCenters = new Vector3[12];

            CalculateEdgeCenters();

            edgeAxes = new CardinalAxisType[12];
            edgeAxes[0] = CardinalAxisType.X;
            edgeAxes[1] = CardinalAxisType.Y;
            edgeAxes[2] = CardinalAxisType.X;
            edgeAxes[3] = CardinalAxisType.Y;
            edgeAxes[4] = CardinalAxisType.X;
            edgeAxes[5] = CardinalAxisType.Y;
            edgeAxes[6] = CardinalAxisType.X;
            edgeAxes[7] = CardinalAxisType.Y;
            edgeAxes[8] = CardinalAxisType.Z;
            edgeAxes[9] = CardinalAxisType.Z;
            edgeAxes[10] = CardinalAxisType.Z;
            edgeAxes[11] = CardinalAxisType.Z;

            for (int i = 0; i < edgeCenters.Length; ++i)
            {
                GameObject midpoint = new GameObject();
                midpoint.name = "midpoint_" + i.ToString();
                midpoint.transform.position = edgeCenters[i];
                midpoint.transform.parent = rigRoot.transform;

                GameObject midpointVisual;
                if (rotationHandlePrefab != null)
                {
                    midpointVisual = Instantiate(rotationHandlePrefab);
                }
                else
                {
                    midpointVisual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    Destroy(midpointVisual.GetComponent<SphereCollider>());
                }

                // Align handle with its edge assuming that the prefab is initially aligned with the up direction 
                if (edgeAxes[i] == CardinalAxisType.X)
                {
                    Quaternion realignment = Quaternion.FromToRotation(Vector3.up, Vector3.right);
                    midpointVisual.transform.localRotation = realignment * midpointVisual.transform.localRotation;
                }
                else if (edgeAxes[i] == CardinalAxisType.Z)
                {
                    Quaternion realignment = Quaternion.FromToRotation(Vector3.up, Vector3.forward);
                    midpointVisual.transform.localRotation = realignment * midpointVisual.transform.localRotation;
                }

                Bounds midpointBounds = GetMaxBounds(midpointVisual);
                float maxDim = Mathf.Max(
                    Mathf.Max(midpointBounds.size.x, midpointBounds.size.y),
                    midpointBounds.size.z);
                float invScale = rotationHandleSize / maxDim;

                midpointVisual.transform.parent = midpoint.transform;
                midpointVisual.transform.localScale = new Vector3(invScale, invScale, invScale);
                midpointVisual.transform.localPosition = Vector3.zero;

                AddComponentsToAffordance(midpoint, new Bounds(midpointBounds.center * invScale, midpointBounds.size * invScale), rotationHandlePrefabColliderType, CursorContextInfo.CursorAction.Rotate, rotateHandleColliderPadding);

                balls.Add(midpoint.transform);

                handles.Add(new Handle()
                {
                    Type = HandleType.Rotation,
                    HandleVisual = midpointVisual.transform,
                    HandleVisualRenderer = midpointVisual.GetComponent<Renderer>(),
                });

                if (handleMaterial != null)
                {
                    ApplyMaterialToAllRenderers(midpointVisual, handleMaterial);
                }
            }

            if (links != null)
            {
                GameObject link;
                for (int i = 0; i < edgeCenters.Length; ++i)
                {
                    if (wireframeShape == WireframeType.Cubic)
                    {
                        link = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        Destroy(link.GetComponent<BoxCollider>());
                    }
                    else
                    {
                        link = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                        Destroy(link.GetComponent<CapsuleCollider>());
                    }
                    link.name = "link_" + i.ToString();


                    Vector3 linkDimensions = GetLinkDimensions();
                    if (edgeAxes[i] == CardinalAxisType.Y)
                    {
                        link.transform.localScale = new Vector3(wireframeEdgeRadius, linkDimensions.y, wireframeEdgeRadius);
                        link.transform.Rotate(new Vector3(0.0f, 90.0f, 0.0f));
                    }
                    else if (edgeAxes[i] == CardinalAxisType.Z)
                    {
                        link.transform.localScale = new Vector3(wireframeEdgeRadius, linkDimensions.z, wireframeEdgeRadius);
                        link.transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));
                    }
                    else//X
                    {
                        link.transform.localScale = new Vector3(wireframeEdgeRadius, linkDimensions.x, wireframeEdgeRadius);
                        link.transform.Rotate(new Vector3(0.0f, 0.0f, 90.0f));
                    }

                    link.transform.position = edgeCenters[i];
                    link.transform.parent = rigRoot.transform;
                    Renderer linkRenderer = link.GetComponent<Renderer>();
                    linkRenderers.Add(linkRenderer);

                    if (wireframeMaterial != null)
                    {
                        linkRenderer.material = wireframeMaterial;
                    }

                    links.Add(link.transform);
                }
            }
        }

        /// <summary>
        /// Make the handle colliders ignore specified collider. (e.g. spatial mapping's floor collider to avoid the object get lifted up)
        /// </summary>
        private void HandleIgnoreCollider()
        {
            if (handlesIgnoreCollider != null)
            {
                foreach (Transform corner in corners)
                {
                    Collider[] colliders = corner.gameObject.GetComponents<Collider>();
                    foreach (Collider collider in colliders)
                    {
                        UnityEngine.Physics.IgnoreCollision(collider, handlesIgnoreCollider);
                    }
                }

                foreach (Transform ball in balls)
                {
                    Collider[] colliders = ball.gameObject.GetComponents<Collider>();
                    foreach (Collider collider in colliders)
                    {
                        UnityEngine.Physics.IgnoreCollision(collider, handlesIgnoreCollider);
                    }
                }
            }
        }

        private void AddBoxDisplay()
        {
            if (boxMaterial != null)
            {
                bool isFlattened = flattenAxis != FlattenModeType.DoNotFlatten;

                boxDisplay = GameObject.CreatePrimitive(isFlattened ? PrimitiveType.Quad : PrimitiveType.Cube);
                Destroy(boxDisplay.GetComponent<Collider>());
                boxDisplay.name = "bounding box";

                ApplyMaterialToAllRenderers(boxDisplay, boxMaterial);

                boxDisplay.transform.localScale = GetBoxDisplayScale();
                boxDisplay.transform.parent = rigRoot.transform;
            }
        }

        private Vector3 GetBoxDisplayScale()
        {
            // When a box is flattened one axis is normally scaled to zero, this doesn't always work well with visuals so we take 
            // that flattened axis and re-scale it to the flattenAxisDisplayScale.
            Vector3 displayScale = currentBoundsExtents;
            displayScale.x = (flattenAxis == FlattenModeType.FlattenX) ? flattenAxisDisplayScale : displayScale.x;
            displayScale.y = (flattenAxis == FlattenModeType.FlattenY) ? flattenAxisDisplayScale : displayScale.y;
            displayScale.z = (flattenAxis == FlattenModeType.FlattenZ) ? flattenAxisDisplayScale : displayScale.z;

            return 2.0f * displayScale;
        }

        private void SetBoundingBoxCollider()
        {
            // Make sure that the bounds of all child objects are up to date before we compute bounds
            UnityPhysics.SyncTransforms();

            if (boundsOverride != null)
            {
                TargetBounds = boundsOverride;
                TargetBounds.transform.hasChanged = true;
            }
            else
            {
                Bounds bounds = GetTargetBounds();
                TargetBounds = Target.AddComponent<BoxCollider>();

                TargetBounds.center = bounds.center;
                TargetBounds.size = bounds.size;
            }

            CalculateBoxPadding();

            TargetBounds.EnsureComponent<NearInteractionGrabbable>();
        }

        private void CalculateBoxPadding()
        {
            if (boxPadding == Vector3.zero) { return; }

            Vector3 scale = TargetBounds.transform.lossyScale;

            for (int i = 0; i < 3; i++)
            {
                if (scale[i] == 0f) { return; }

                scale[i] = 1f / scale[i];
            }

            TargetBounds.size += Vector3.Scale(boxPadding, scale);
        }

        private Bounds GetTargetBounds()
        {
            KeyValuePair<Transform, Collider> colliderByTransform;
            KeyValuePair<Transform, Bounds> rendererBoundsByTransform;
            totalBoundsCorners.Clear();

            // Collect all Transforms except for the rigRoot(s) transform structure(s)
            // Its possible we have two rigRoots here, the one about to be deleted and the new one
            // Since those have the gizmo structure childed, be need to omit them completely in the calculation of the bounds
            // This can only happen by name unless there is a better idea of tracking the rigRoot that needs destruction

            List<Transform> childTransforms = new List<Transform>();
            childTransforms.Add(Target.transform);

            foreach (Transform childTransform in Target.transform)
            {
                if (childTransform.name.Equals(rigRootName)) { continue; }
                childTransforms.AddRange(childTransform.GetComponentsInChildren<Transform>());
            }

            // Iterate transforms and collect bound volumes

            foreach (Transform childTransform in childTransforms)
            {
                Debug.Assert(childTransform != rigRoot);

                if (boundsCalculationMethod != BoundsCalculationMethod.RendererOnly)
                {
                    Collider collider = childTransform.GetComponent<Collider>();
                    if (collider != null)
                    {
                        colliderByTransform = new KeyValuePair<Transform, Collider>(childTransform, collider);
                    }
                    else
                    {
                        colliderByTransform = new KeyValuePair<Transform, Collider>();
                    }
                }

                if (boundsCalculationMethod != BoundsCalculationMethod.ColliderOnly)
                {
                    MeshFilter meshFilter = childTransform.GetComponent<MeshFilter>();
                    if (meshFilter != null && meshFilter.sharedMesh != null)
                    {
                        rendererBoundsByTransform = new KeyValuePair<Transform, Bounds>(childTransform, meshFilter.sharedMesh.bounds);
                    }
                    else
                    {
                        rendererBoundsByTransform = new KeyValuePair<Transform, Bounds>();
                    }
                }

                // Encapsulate the collider bounds if criteria match

                if (boundsCalculationMethod == BoundsCalculationMethod.ColliderOnly ||
                    boundsCalculationMethod == BoundsCalculationMethod.ColliderOverRenderer)
                {
                    AddColliderBoundsToTarget(colliderByTransform);
                    if (boundsCalculationMethod == BoundsCalculationMethod.ColliderOnly) { continue; }
                }

                // Encapsulate the renderer bounds if criteria match

                if (boundsCalculationMethod != BoundsCalculationMethod.ColliderOnly)
                {
                    AddRendererBoundsToTarget(rendererBoundsByTransform);
                    if (boundsCalculationMethod == BoundsCalculationMethod.RendererOnly) { continue; }
                }

                // Do the collider for the one case that we chose RendererOverCollider and did not find a renderer
                AddColliderBoundsToTarget(colliderByTransform);
            }

            if (totalBoundsCorners.Count == 0) { return new Bounds(); }

            Transform targetTransform = Target.transform;

            Bounds finalBounds = new Bounds(targetTransform.InverseTransformPoint(totalBoundsCorners[0]), Vector3.zero);

            for (int i = 1; i < totalBoundsCorners.Count; i++)
            {
                finalBounds.Encapsulate(targetTransform.InverseTransformPoint(totalBoundsCorners[i]));
            }

            return finalBounds;
        }

        private void AddRendererBoundsToTarget(KeyValuePair<Transform, Bounds> rendererBoundsByTarget)
        {
            if (rendererBoundsByTarget.Key == null) { return; }

            Vector3[] cornersToWorld = null;
            rendererBoundsByTarget.Value.GetCornerPositions(rendererBoundsByTarget.Key, ref cornersToWorld);
            totalBoundsCorners.AddRange(cornersToWorld);
        }

        private void AddColliderBoundsToTarget(KeyValuePair<Transform, Collider> colliderByTransform)
        {
            if (colliderByTransform.Key != null)
            {
                BoundsExtensions.GetColliderBoundsPoints(colliderByTransform.Value, totalBoundsCorners, 0);
            }
        }

        private void SetMaterials()
        {
            //ensure materials
            if (wireframeMaterial == null)
            {
                float[] color = { 1.0f, 1.0f, 1.0f, 0.75f };

                wireframeMaterial = new Material(StandardShaderUtility.MrtkStandardShader);
                wireframeMaterial.EnableKeyword("_InnerGlow");
                wireframeMaterial.SetColor("_Color", new Color(0.0f, 0.63f, 1.0f));
                wireframeMaterial.SetFloat("_InnerGlow", 1.0f);
                wireframeMaterial.SetFloatArray("_InnerGlowColor", color);
            }
            if (handleMaterial == null && handleMaterial != wireframeMaterial)
            {
                float[] color = { 1.0f, 1.0f, 1.0f, 0.75f };

                handleMaterial = new Material(StandardShaderUtility.MrtkStandardShader);
                handleMaterial.EnableKeyword("_InnerGlow");
                handleMaterial.SetColor("_Color", new Color(0.0f, 0.63f, 1.0f));
                handleMaterial.SetFloat("_InnerGlow", 1.0f);
                handleMaterial.SetFloatArray("_InnerGlowColor", color);
            }
            if (handleGrabbedMaterial == null && handleGrabbedMaterial != handleMaterial && handleGrabbedMaterial != wireframeMaterial)
            {
                float[] color = { 1.0f, 1.0f, 1.0f, 0.75f };

                handleGrabbedMaterial = new Material(StandardShaderUtility.MrtkStandardShader);
                handleGrabbedMaterial.EnableKeyword("_InnerGlow");
                handleGrabbedMaterial.SetColor("_Color", new Color(0.0f, 0.63f, 1.0f));
                handleGrabbedMaterial.SetFloat("_InnerGlow", 1.0f);
                handleGrabbedMaterial.SetFloatArray("_InnerGlowColor", color);
            }
        }

        private void InitializeRigRoot()
        {
            var rigRootObj = new GameObject(rigRootName);
            rigRoot = rigRootObj.transform;
            rigRoot.parent = transform;

            var pH = rigRootObj.AddComponent<PointerHandler>();
            pH.OnPointerDown.AddListener(OnPointerDown);
            pH.OnPointerDragged.AddListener(OnPointerDragged);
            pH.OnPointerUp.AddListener(OnPointerUp);
        }

        private void InitializeDataStructures()
        {
            boundsCorners = new Vector3[8];

            corners = new List<Transform>();
            balls = new List<Transform>();

            handles = new List<Handle>();

            if (showWireframe)
            {
                links = new List<Transform>();
                linkRenderers = new List<Renderer>();
            }

            sourcesDetected = new List<IMixedRealityController>();
        }

        private void CalculateEdgeCenters()
        {
            if (boundsCorners != null && edgeCenters != null)
            {
                edgeCenters[0] = (boundsCorners[0] + boundsCorners[1]) * 0.5f;
                edgeCenters[1] = (boundsCorners[0] + boundsCorners[2]) * 0.5f;
                edgeCenters[2] = (boundsCorners[3] + boundsCorners[2]) * 0.5f;
                edgeCenters[3] = (boundsCorners[3] + boundsCorners[1]) * 0.5f;

                edgeCenters[4] = (boundsCorners[4] + boundsCorners[5]) * 0.5f;
                edgeCenters[5] = (boundsCorners[4] + boundsCorners[6]) * 0.5f;
                edgeCenters[6] = (boundsCorners[7] + boundsCorners[6]) * 0.5f;
                edgeCenters[7] = (boundsCorners[7] + boundsCorners[5]) * 0.5f;

                edgeCenters[8] = (boundsCorners[0] + boundsCorners[4]) * 0.5f;
                edgeCenters[9] = (boundsCorners[1] + boundsCorners[5]) * 0.5f;
                edgeCenters[10] = (boundsCorners[2] + boundsCorners[6]) * 0.5f;
                edgeCenters[11] = (boundsCorners[3] + boundsCorners[7]) * 0.5f;
            }
        }

        private void CaptureInitialState()
        {
            var target = Target;
            if (target != null)
            {
                isChildOfTarget = transform.IsChildOf(target.transform);

                scaleHandler = GetComponent<TransformScaleHandler>();
                if (scaleHandler == null)
                {
                    scaleHandler = gameObject.AddComponent<TransformScaleHandler>();

                    scaleHandler.TargetTransform = Target.transform;
                #pragma warning disable 0618
                    scaleHandler.ScaleMinimum = scaleMinimum;
                    scaleHandler.ScaleMaximum = scaleMaximum;
                #pragma warning restore 0618
                }
            }
        }

        private Vector3 GetLinkDimensions()
        {
            float linkLengthAdjustor = wireframeShape == WireframeType.Cubic ? 2.0f : 1.0f - (6.0f * wireframeEdgeRadius);
            return (currentBoundsExtents * linkLengthAdjustor) + new Vector3(wireframeEdgeRadius, wireframeEdgeRadius, wireframeEdgeRadius);
        }

        private bool ShouldRotateHandleBeVisible(CardinalAxisType axisType)
        {
            return
                (axisType == CardinalAxisType.X && showRotationHandleForX) ||
                (axisType == CardinalAxisType.Y && showRotationHandleForY) ||
                (axisType == CardinalAxisType.Z && showRotationHandleForZ);
        }

        private void ResetHandleVisibility()
        {
            if (currentPointer != null)
            {
                return;
            }

            bool isVisible;

            //set balls visibility
            if (balls != null)
            {
                isVisible = (active == true && wireframeOnly == false);
                for (int i = 0; i < balls.Count; ++i)
                {
                    balls[i].gameObject.SetActive(isVisible && ShouldRotateHandleBeVisible(edgeAxes[i]));
                    ApplyMaterialToAllRenderers(balls[i].gameObject, handleMaterial);
                }
            }

            //set link visibility
            if (links != null)
            {
                isVisible = active == true;
                for (int i = 0; i < linkRenderers.Count; ++i)
                {
                    if (linkRenderers[i] != null)
                    {
                        linkRenderers[i].enabled = isVisible;
                    }
                }
            }

            //set box display visibility
            if (boxDisplay != null)
            {
                boxDisplay.SetActive(active);
                ApplyMaterialToAllRenderers(boxDisplay, boxMaterial);
            }

            //set corner visibility
            if (corners != null)
            {
                isVisible = (active == true && wireframeOnly == false && showScaleHandles == true);

                for (int i = 0; i < corners.Count; ++i)
                {
                    corners[i].gameObject.SetActive(isVisible);
                    ApplyMaterialToAllRenderers(corners[i].gameObject, handleMaterial);
                }
            }

            SetHiddenHandles();
        }

        private void SetHighlighted(Transform activeHandle)
        {
            //turn off all balls
            if (balls != null)
            {
                for (int i = 0; i < balls.Count; ++i)
                {
                    if (balls[i] != activeHandle)
                    {
                        balls[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        ApplyMaterialToAllRenderers(balls[i].gameObject, handleGrabbedMaterial);
                    }
                }
            }

            //turn off all corners
            if (corners != null)
            {
                for (int i = 0; i < corners.Count; ++i)
                {
                    if (corners[i] != activeHandle)
                    {
                        corners[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        ApplyMaterialToAllRenderers(corners[i].gameObject, handleGrabbedMaterial);
                    }
                }
            }

            //update the box material to the grabbed material
            if (boxDisplay != null)
            {
                ApplyMaterialToAllRenderers(boxDisplay, boxGrabbedMaterial);
            }
        }

        private void UpdateBounds()
        {
            if (TargetBounds != null)
            {
                // Store current rotation then zero out the rotation so that the bounds
                // are computed when the object is in its 'axis aligned orientation'.
                Quaternion currentRotation = Target.transform.rotation;
                Target.transform.rotation = Quaternion.identity;
                UnityPhysics.SyncTransforms(); // Update collider bounds

                Vector3 boundsExtents = TargetBounds.bounds.extents;

                // After bounds are computed, restore rotation...
                Target.transform.rotation = currentRotation;
                UnityPhysics.SyncTransforms();

                if (boundsExtents != Vector3.zero)
                {
                    if (flattenAxis == FlattenModeType.FlattenAuto)
                    {
                        float min = Mathf.Min(boundsExtents.x, Mathf.Min(boundsExtents.y, boundsExtents.z));
                        flattenAxis = (min == boundsExtents.x) ? FlattenModeType.FlattenX :
                            ((min == boundsExtents.y) ? FlattenModeType.FlattenY : FlattenModeType.FlattenZ);
                    }

                    boundsExtents.x = (flattenAxis == FlattenModeType.FlattenX) ? 0.0f : boundsExtents.x;
                    boundsExtents.y = (flattenAxis == FlattenModeType.FlattenY) ? 0.0f : boundsExtents.y;
                    boundsExtents.z = (flattenAxis == FlattenModeType.FlattenZ) ? 0.0f : boundsExtents.z;
                    currentBoundsExtents = boundsExtents;

                    GetCornerPositionsFromBounds(new Bounds(Vector3.zero, boundsExtents * 2.0f), ref boundsCorners);
                    CalculateEdgeCenters();
                }
            }
        }

        private void UpdateRigHandles()
        {
            if (rigRoot != null && Target != null)
            {
                // We move the rigRoot to the scene root to ensure that non-uniform scaling performed
                // anywhere above the rigRoot does not impact the position of rig corners / edges
                rigRoot.parent = null;

                rigRoot.rotation = Quaternion.identity;
                rigRoot.position = Vector3.zero;
                rigRoot.localScale = Vector3.one;

                for (int i = 0; i < corners.Count; ++i)
                {
                    corners[i].position = boundsCorners[i];
                }

                Vector3 rootScale = rigRoot.lossyScale;
                Vector3 invRootScale = new Vector3(1.0f / rootScale[0], 1.0f / rootScale[1], 1.0f / rootScale[2]);

                // Compute the local scale that produces the desired world space dimensions
                Vector3 linkDimensions = Vector3.Scale(GetLinkDimensions(), invRootScale);

                for (int i = 0; i < edgeCenters.Length; ++i)
                {
                    balls[i].position = edgeCenters[i];

                    if (links != null)
                    {
                        links[i].position = edgeCenters[i];

                        if (edgeAxes[i] == CardinalAxisType.X)
                        {
                            links[i].localScale = new Vector3(wireframeEdgeRadius, linkDimensions.x, wireframeEdgeRadius);
                        }
                        else if (edgeAxes[i] == CardinalAxisType.Y)
                        {
                            links[i].localScale = new Vector3(wireframeEdgeRadius, linkDimensions.y, wireframeEdgeRadius);
                        }
                        else//Z
                        {
                            links[i].localScale = new Vector3(wireframeEdgeRadius, linkDimensions.z, wireframeEdgeRadius);
                        }
                    }
                }

                if (boxDisplay != null)
                {
                    // Compute the local scale that produces the desired world space size
                    boxDisplay.transform.localScale = Vector3.Scale(GetBoxDisplayScale(), invRootScale);
                }

                //move rig into position and rotation
                rigRoot.position = TargetBounds.bounds.center;
                rigRoot.rotation = Target.transform.rotation;
                rigRoot.parent = transform;
            }
        }

        private void HandleProximityScaling()
        {
            //only use proximity effect if nothing is being dragged or grabbed
            if (currentPointer == null)
            {
                proximityPointers.Clear();
                proximityPoints.Clear();

                // Find all valid pointers
                foreach (var inputSource in CoreServices.InputSystem.DetectedInputSources)
                {
                    foreach (var pointer in inputSource.Pointers)
                    {
                        if (pointer.IsInteractionEnabled && !proximityPointers.Contains(pointer))
                        {
                            proximityPointers.Add(pointer);
                        }
                    }
                }

                // Get the max radius possible of our current bounds plus the proximity
                float maxRadius = Mathf.Max(Mathf.Max(currentBoundsExtents.x, currentBoundsExtents.y), currentBoundsExtents.z);
                maxRadius *= maxRadius;
                maxRadius += handleCloseProximity + handleMediumProximity;

                // Grab points within sphere of influence from valid pointers
                foreach (var pointer in proximityPointers)
                {
                    if (IsPointWithinBounds(pointer.Position, maxRadius))
                    {
                        proximityPoints.Add(pointer.Position);
                    }

                    Vector3? point = pointer.Result?.Details.Point;

                    if (point.HasValue && IsPointWithinBounds(point.Value, maxRadius))
                    {
                        proximityPoints.Add(pointer.Result.Details.Point);
                    }
                }

                // Loop through all handles and find closest one
                int closestHandleIdx = -1;
                float closestDistanceSqr = float.MaxValue;
                foreach (var point in proximityPoints)
                {
                    for (int i = 0; i < handles.Count; ++i)
                    {
                        // If handle can't be visible, skip calculations
                        if (!IsHandleTypeVisible(handles[i].Type))
                            continue;

                        // Perform comparison on sqr distance since sqrt() operation is expensive in Vector3.Distance()
                        float sqrDistance = (handles[i].HandleVisual.position - point).sqrMagnitude;
                        if (sqrDistance < closestDistanceSqr)
                        {
                            closestHandleIdx = i;
                            closestDistanceSqr = sqrDistance;
                        }
                    }
                }

                // Loop through all handles and update visual state based on closest point
                for (int i = 0; i < handles.Count; ++i)
                {
                    Handle h = handles[i];

                    HandleProximityState newState = i == closestHandleIdx ? GetProximityState(closestDistanceSqr) : HandleProximityState.FullsizeNoProximity;

                    // Only apply updates if handle is in a new state or closest handle needs to lerp scaling
                    if (h.ProximityState != newState)
                    {
                        // Update and save new state
                        h.ProximityState = newState;

                        if (h.HandleVisualRenderer != null)
                        {
                            h.HandleVisualRenderer.material = newState == HandleProximityState.CloseProximity ? handleGrabbedMaterial : handleMaterial;
                        }
                    }

                    ScaleHandle(h, true);
                }
            }
        }

        /// <summary>
        /// Get the ProximityState value based on the distanced provided
        /// </summary>
        /// <param name="sqrDistance">distance squared in proximity in meters</param>
        /// <returns>HandleProximityState for given distance</returns>
        private HandleProximityState GetProximityState(float sqrDistance)
        {
            if (sqrDistance < handleCloseProximity * handleCloseProximity)
            {
                return HandleProximityState.CloseProximity;
            }
            else if (sqrDistance < handleMediumProximity * handleMediumProximity)
            {
                return HandleProximityState.MediumProximity;
            }
            else
            {
                return HandleProximityState.FullsizeNoProximity;
            }
        }

        private void ScaleHandle(Handle handle, bool lerp = false)
        {
            float handleSize = handle.Type == HandleType.Scale ? scaleHandleSize : rotationHandleSize;
            float targetScale = 1.0f, weight = 0.0f;

            switch (handle.ProximityState)
            {
                case HandleProximityState.FullsizeNoProximity:
                    targetScale = farScale;
                    weight = lerp ? farGrowRate : 1.0f;
                    break;
                case HandleProximityState.MediumProximity:
                    targetScale = mediumScale;
                    weight = lerp ? mediumGrowRate : 1.0f;
                    break;
                case HandleProximityState.CloseProximity:
                    targetScale = closeScale;
                    weight = lerp ? closeGrowRate : 1.0f;
                    break;
            }

            float newLocalScale = (handle.HandleVisual.localScale.x * (1.0f - weight)) + (handleSize * targetScale * weight);
            handle.HandleVisual.localScale = new Vector3(newLocalScale, newLocalScale, newLocalScale);
        }

        /// <summary>
        /// Determine if passed point is within sphere of radius around this GameObject
        /// To avoid function overhead, request compiler to inline this function since repeatedly called every Update() for every pointer position and result
        /// </summary>
        /// <param name="point">world space position</param>
        /// <param name="radiusSqr">radius of sphere in distance squared for faster comparison</param>
        /// <returns>true if point is within sphere</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsPointWithinBounds(Vector3 point, float radiusSqr)
        {
            return (transform.position - point).sqrMagnitude < radiusSqr;
        }

        /// <summary>
        /// Helper method to check if handle type may be visible based on configuration
        /// </summary>
        /// <param name="h">handle reference to check</param>
        /// <returns>true if potentially visible, false otherwise</returns>
        private bool IsHandleTypeVisible(HandleType type)
        {
            return (type == HandleType.Scale && ShowScaleHandles) ||
                (type == HandleType.Rotation && (ShowRotationHandleForX || ShowRotationHandleForY || ShowRotationHandleForZ));
        }

        private void ResetHandleProximityScale()
        {
            for (int i = 0; i < handles.Count; ++i)
            {
                Handle h = handles[i];
                if (h.ProximityState != HandleProximityState.FullsizeNoProximity)
                {
                    h.ProximityState = HandleProximityState.FullsizeNoProximity;

                    if (h.HandleVisualRenderer != null)
                    {
                        h.HandleVisualRenderer.material = handleMaterial;
                    }

                    ScaleHandle(h);
                }
            }
        }

        private HandleType GetHandleType(Transform handle)
        {
            for (int i = 0; i < balls.Count; ++i)
            {
                if (handle == balls[i])
                {
                    return HandleType.Rotation;
                }
            }
            for (int i = 0; i < corners.Count; ++i)
            {
                if (handle == corners[i])
                {
                    return HandleType.Scale;
                }
            }

            return HandleType.None;
        }

        private void Flatten()
        {
            if (flattenAxis == FlattenModeType.FlattenX)
            {
                flattenedHandles = new int[] { 0, 4, 2, 6 };
            }
            else if (flattenAxis == FlattenModeType.FlattenY)
            {
                flattenedHandles = new int[] { 1, 3, 5, 7 };
            }
            else if (flattenAxis == FlattenModeType.FlattenZ)
            {
                flattenedHandles = new int[] { 9, 10, 8, 11 };
            }

            if (flattenedHandles != null && linkRenderers != null)
            {
                for (int i = 0; i < flattenedHandles.Length; ++i)
                {
                    linkRenderers[flattenedHandles[i]].enabled = false;
                }
            }
        }

        private void SetHiddenHandles()
        {
            if (flattenedHandles != null)
            {
                for (int i = 0; i < flattenedHandles.Length; ++i)
                {
                    balls[flattenedHandles[i]].gameObject.SetActive(false);
                }
            }
        }

        private void GetCornerPositionsFromBounds(Bounds bounds, ref Vector3[] positions)
        {
            int numCorners = 1 << 3;
            if (positions == null || positions.Length != numCorners)
            {
                positions = new Vector3[numCorners];
            }

            // Permutate all axes using minCorner and maxCorner.
            Vector3 minCorner = bounds.center - bounds.extents;
            Vector3 maxCorner = bounds.center + bounds.extents;
            for (int c = 0; c < numCorners; c++)
            {
                positions[c] = new Vector3(
                    (c & (1 << 0)) == 0 ? minCorner[0] : maxCorner[0],
                    (c & (1 << 1)) == 0 ? minCorner[1] : maxCorner[1],
                    (c & (1 << 2)) == 0 ? minCorner[2] : maxCorner[2]);
            }
        }

        private static void ApplyMaterialToAllRenderers(GameObject root, Material material)
        {
            if (material != null)
            {
                Renderer[] renderers = root.GetComponentsInChildren<Renderer>();

                for (int i = 0; i < renderers.Length; ++i)
                {
                    renderers[i].material = material;
                }
            }
        }

        private bool DoesActivationMatchFocus(FocusEventData eventData)
        {
            switch (activation)
            {
                case BoundingBoxActivationType.ActivateOnStart:
                case BoundingBoxActivationType.ActivateManually:
                    return false;
                case BoundingBoxActivationType.ActivateByProximity:
                    return eventData.Pointer is IMixedRealityNearPointer;
                case BoundingBoxActivationType.ActivateByPointer:
                    return eventData.Pointer is IMixedRealityPointer;
                case BoundingBoxActivationType.ActivateByProximityAndPointer:
                    return true;
                default:
                    return false;
            }
        }

        private void DropController()
        {
            HandleType lastHandleType = currentHandleType;
            currentPointer = null;
            currentHandleType = HandleType.None;
            ResetHandleVisibility();

            if (lastHandleType == HandleType.Scale)
            {
                if (debugText != null) debugText.text = "OnPointerUp:ScaleStopped";
                ScaleStopped?.Invoke();
            }
            else if (lastHandleType == HandleType.Rotation)
            {
                if (debugText != null) debugText.text = "OnPointerUp:RotateStopped";
                RotateStopped?.Invoke();
            }
        }

        #endregion Private Methods


        #region Used Event Handlers

        void IMixedRealityFocusChangedHandler.OnFocusChanged(FocusEventData eventData)
        {
            if (proximityEffectActive && eventData.NewFocusedObject == null)
            {
                ResetHandleProximityScale();
            }

            if (activation == BoundingBoxActivationType.ActivateManually || activation == BoundingBoxActivationType.ActivateOnStart)
            {
                return;
            }

            if (!DoesActivationMatchFocus(eventData))
            {
                return;
            }

            bool handInProximity = eventData.NewFocusedObject != null && eventData.NewFocusedObject.transform.IsChildOf(transform);
            if (handInProximity == wireframeOnly)
            {
                wireframeOnly = !handInProximity;
                ResetHandleVisibility();
            }
        }

        void IMixedRealityFocusHandler.OnFocusExit(FocusEventData eventData)
        {
            if (currentPointer != null && eventData.Pointer == currentPointer)
            {
                DropController();
            }
        }

        void IMixedRealityFocusHandler.OnFocusEnter(FocusEventData eventData) { }

        private void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            if (currentPointer != null && eventData.Pointer == currentPointer)
            {
                DropController();
                eventData.Use();
            }
        }

        private void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            if (currentPointer == null && !eventData.used)
            {
                GameObject grabbedHandle = eventData.Pointer.Result.CurrentPointerTarget;
                Transform grabbedHandleTransform = grabbedHandle.transform;
                currentHandleType = GetHandleType(grabbedHandleTransform);
                if (currentHandleType != HandleType.None)
                {
                    currentPointer = eventData.Pointer;
                    initialGrabPoint = currentPointer.Result.Details.Point;
                    currentGrabPoint = initialGrabPoint;
                    initialScaleOnGrabStart = Target.transform.localScale;
                    initialPositionOnGrabStart = Target.transform.position;
                    grabPointInPointer = Quaternion.Inverse(eventData.Pointer.Rotation) * (initialGrabPoint - currentPointer.Position);

                    SetHighlighted(grabbedHandleTransform);

                    if (currentHandleType == HandleType.Scale)
                    {
                        // Will use this to scale the target relative to the opposite corner
                        oppositeCorner = rigRoot.transform.TransformPoint(-grabbedHandle.transform.localPosition);
                        diagonalDir = (grabbedHandle.transform.position - oppositeCorner).normalized;

                        ScaleStarted?.Invoke();

                        if (debugText != null)
                        {
                            debugText.text = "OnPointerDown:ScaleStarted";
                        }
                    }
                    else if (currentHandleType == HandleType.Rotation)
                    {
                        currentRotationAxis = GetRotationAxis(grabbedHandleTransform);

                        RotateStarted?.Invoke();

                        if (debugText != null)
                        {
                            debugText.text = "OnPointerDown:RotateStarted";
                        }
                    }

                    eventData.Use();
                }
            }

            if (currentPointer != null)
            {
                // Always mark the pointer data as used to prevent any other behavior to handle pointer events
                // as long as BoundingBox manipulation is active.
                // This is due to us reacting to both "Select" and "Grip" events.
                eventData.Use();
            }
        }

        private void OnPointerDragged(MixedRealityPointerEventData eventData) { }

        public void OnSourceDetected(SourceStateEventData eventData)
        {
            if (eventData.Controller != null)
            {
                if (sourcesDetected.Count == 0 || sourcesDetected.Contains(eventData.Controller) == false)
                {
                    sourcesDetected.Add(eventData.Controller);
                }
            }
        }

        public void OnSourceLost(SourceStateEventData eventData)
        {
            sourcesDetected.Remove(eventData.Controller);

            if (currentPointer != null && currentPointer.InputSourceParent.SourceId == eventData.SourceId)
            {
                HandleType lastHandleType = currentHandleType;

                currentPointer = null;
                currentHandleType = HandleType.None;
                ResetHandleVisibility();

                if (lastHandleType == HandleType.Scale)
                {
                    if (debugText != null) debugText.text = "OnSourceLost:ScaleStopped";
                    ScaleStopped?.Invoke();
                }
                else if (lastHandleType == HandleType.Rotation)
                {
                    if (debugText != null) debugText.text = "OnSourceLost:RotateStopped";
                    RotateStopped?.Invoke();
                }
            }
        }

        #endregion Used Event Handlers


        #region Unused Event Handlers

        void IMixedRealityFocusChangedHandler.OnBeforeFocusChange(FocusEventData eventData) { }

        #endregion Unused Event Handlers
    }
}

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityPhysics = UnityEngine.Physics;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// BoundingBox allows to transform objects (rotate and scale) and draws a cube around the object to visualize 
    /// the possibility of user triggered transform manipulation. 
    /// BoundingBox provides scale and rotation handles that can be used for far and near interaction manipulation
    /// of the object. It further provides a proximity effect for scale and rotation handles that alters scaling and material. 
    /// </summary>
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/README_BoundingBox.html")]
    public class BoundingBox : MonoBehaviour,
        IMixedRealitySourceStateHandler,
        IMixedRealityFocusChangedHandler,
        IMixedRealityFocusHandler
    {
        #region Enums

        /// <summary>
        /// Enum which describes how an object's BoundingBox is to be flattened.
        /// </summary>
        public enum FlattenModeType
        {
            DoNotFlatten = 0,
            /// <summary>
            /// Flatten the X axis
            /// </summary>
            FlattenX,
            /// <summary>
            /// Flatten the Y axis
            /// </summary>
            FlattenY,
            /// <summary>
            /// Flatten the Z axis
            /// </summary>
            FlattenZ,
            /// <summary>
            /// Flatten the smallest relative axis if it falls below threshold
            /// </summary>
            FlattenAuto,
        }

        /// <summary>
        /// Enum which describes whether a BoundingBox handle which has been grabbed, is 
        /// a Rotation Handle (sphere) or a Scale Handle( cube)
        /// </summary>
        public enum HandleType
        {
            None = 0,
            Rotation,
            Scale
        }

        /// <summary>
        /// This enum describes which primitive type the wireframe portion of the BoundingBox
        /// consists of. 
        /// </summary>
        /// <remarks>
        /// Wireframe refers to the thin linkage between the handles. When the handles are invisible
        /// the wireframe looks like an outline box around an object.
        /// </remarks> 
        public enum WireframeType
        {
            Cubic = 0,
            Cylindrical
        }

        /// <summary>
        /// This enum defines which of the axes a given rotation handle revolves about.
        /// </summary>
        private enum CardinalAxisType
        {
            X = 0,
            Y,
            Z
        }

        /// <summary>
        /// This enum defines what volume type the bound calculation depends on and its priority
        /// for it.
        /// </summary>
        public enum BoundsCalculationMethod
        {
            /// <summary>
            /// Used Renderers for the bounds calculation and Colliders as a fallback
            /// </summary>
            RendererOverCollider = 0,
            /// <summary>
            /// Used Colliders for the bounds calculation and Renderers as a fallback
            /// </summary>
            ColliderOverRenderer,
            /// <summary>
            /// Omits Renderers and uses Colliders for the bounds calculation exclusively
            /// </summary>
            ColliderOnly,
            /// <summary>
            /// Omits Colliders and uses Renderers for the bounds calculation exclusively
            /// </summary>
            RendererOnly,
        }

        /// <summary>
        /// This enum defines how the BoundingBox gets activated
        /// </summary>
        public enum BoundingBoxActivationType
        {
            ActivateOnStart = 0,
            ActivateByProximity,
            ActivateByPointer,
            ActivateByProximityAndPointer,
            ActivateManually
        }

        /// <summary>
        /// Internal state tracking for proximity of a handle
        /// </summary>
        private enum HandleProximityState
        {
            FullsizeNoProximity = 0,
            MediumProximity,
            CloseProximity
        }

        /// <summary>
        /// This enum defines the type of collider in use when a rotation handle prefab is provided.
        /// </summary>
        public enum RotationHandlePrefabCollider
        {
            Sphere,
            Box
        }

        /// <summary>
        /// Container for handle references and states (including scale and rotation type handles) which is used in the handle proximity effect
        /// </summary>
        private class Handle
        {
            public Transform HandleVisual;
            public Renderer HandleVisualRenderer;
            public HandleType Type = HandleType.None;
            public HandleProximityState ProximityState = HandleProximityState.FullsizeNoProximity;
        }

        #endregion Enums

        #region Serialized Fields and Properties
        [SerializeField]
        [Tooltip("The object that the bounding box rig will be modifying.")]
        private GameObject targetObject;
        /// <summary>
        /// The object that the bounding box rig will be modifying.
        /// </summary>
        public GameObject Target
        {
            get
            {
                if (targetObject == null)
                {
                    targetObject = gameObject;
                }

                return targetObject;
            }
        }

        [Tooltip("For complex objects, automatic bounds calculation may not behave as expected. Use an existing Box Collider (even on a child object) to manually determine bounds of Bounding Box.")]
        [SerializeField]
        [FormerlySerializedAs("BoxColliderToUse")]
        private BoxCollider boundsOverride = null;

        /// <summary>
        /// For complex objects, automatic bounds calculation may not behave as expected. Use an existing Box Collider (even on a child object) to manually determine bounds of Bounding Box.
        /// </summary>
        public BoxCollider BoundsOverride
        {
            get { return boundsOverride; }
            set
            {
                if (boundsOverride != value)
                {
                    boundsOverride = value;

                    if (boundsOverride == null)
                    {
                        prevBoundsOverride = new Bounds();
                    }
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [Tooltip("Defines the volume type and the priority for the bounds calculation")]
        private BoundsCalculationMethod boundsCalculationMethod = BoundsCalculationMethod.RendererOverCollider;

        /// <summary>
        /// Defines the volume type and the priority for the bounds calculation
        /// </summary>
        public BoundsCalculationMethod CalculationMethod
        {
            get { return boundsCalculationMethod; }
            set
            {
                if (boundsCalculationMethod != value)
                {
                    boundsCalculationMethod = value;
                    CreateRig();
                }
            }
        }

        [Header("Behavior")]
        [SerializeField]
        [Tooltip("Type of activation method for showing/hiding bounding box handles and controls")]
        private BoundingBoxActivationType activation = BoundingBoxActivationType.ActivateOnStart;

        /// <summary>
        /// Type of activation method for showing/hiding bounding box handles and controls
        /// </summary>
        public BoundingBoxActivationType BoundingBoxActivation
        {
            get { return activation; }
            set
            {
                if (activation != value)
                {
                    activation = value;
                    ResetHandleVisibility();
                }
            }
        }

        [SerializeField]
        [Obsolete("Use a TransformScaleHandler script rather than setting minimum on BoundingBox directly", false)]
        [Tooltip("Minimum scaling allowed relative to the initial size")]
        private float scaleMinimum = 0.2f;

        [SerializeField]
        [Obsolete("Use a TransformScaleHandler script rather than setting maximum on BoundingBox directly")]
        [Tooltip("Maximum scaling allowed relative to the initial size")]
        private float scaleMaximum = 2.0f;


        /// <summary>
        /// Deprecated: Use TransformScaleHandler component instead.
        /// Public property for the scale minimum, in the target's local scale.
        /// Set this value with SetScaleLimits.
        /// </summary>
        [Obsolete("Use a TransformScaleHandler.ScaleMinimum as it is the authoritative value for min scale")]
        public float ScaleMinimum
        {
            get
            {
                if (scaleHandler != null)
                {
                    return scaleHandler.ScaleMinimum;
                }
                return 0.0f;
            }
        }

        /// <summary>
        /// Deprecated: Use TransformScaleHandler component instead.
        /// Public property for the scale maximum, in the target's local scale.
        /// Set this value with SetScaleLimits.
        /// </summary>
        [Obsolete("Use a TransformScaleHandler.ScaleMinimum as it is the authoritative value for max scale")]
        public float ScaleMaximum
        {
            get
            {
                if (scaleHandler != null)
                {
                    return scaleHandler.ScaleMaximum;
                }
                return 0.0f;
            }
        }

        [Header("Box Display")]
        [SerializeField]
        [Tooltip("Flatten bounds in the specified axis or flatten the smallest one if 'auto' is selected")]
        private FlattenModeType flattenAxis = FlattenModeType.DoNotFlatten;

        /// <summary>
        /// Flatten bounds in the specified axis or flatten the smallest one if 'auto' is selected
        /// </summary>
        public FlattenModeType FlattenAxis
        {
            get { return flattenAxis; }
            set
            {
                if (flattenAxis != value)
                {
                    flattenAxis = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [Tooltip("When an axis is flattened what value to set that axis's scale to for display.")]
        private float flattenAxisDisplayScale = 0.0f;

        /// <summary>
        /// When an axis is flattened what value to set that axis's scale to for display.
        /// </summary>
        public float FlattenAxisDisplayScale
        {
            get { return flattenAxisDisplayScale; }
            set
            {
                if (flattenAxisDisplayScale != value)
                {
                    flattenAxisDisplayScale = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [FormerlySerializedAs("wireframePadding")]
        [Tooltip("Extra padding added to the actual Target bounds")]
        private Vector3 boxPadding = Vector3.zero;

        /// <summary>
        /// Extra padding added to the actual Target bounds
        /// </summary>
        public Vector3 BoxPadding
        {
            get { return boxPadding; }
            set
            {
                if (Vector3.Distance(boxPadding, value) > float.Epsilon)
                {
                    boxPadding = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [Tooltip("Material used to display the bounding box. If set to null no bounding box will be displayed")]
        private Material boxMaterial = null;

        /// <summary>
        /// Material used to display the bounding box. If set to null no bounding box will be displayed
        /// </summary>
        public Material BoxMaterial
        {
            get { return boxMaterial; }
            set
            {
                if (boxMaterial != value)
                {
                    boxMaterial = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [Tooltip("Material used to display the bounding box when grabbed. If set to null no change will occur when grabbed.")]
        private Material boxGrabbedMaterial = null;

        /// <summary>
        /// Material used to display the bounding box when grabbed. If set to null no change will occur when grabbed.
        /// </summary>
        public Material BoxGrabbedMaterial
        {
            get { return boxGrabbedMaterial; }
            set
            {
                if (boxGrabbedMaterial != value)
                {
                    boxGrabbedMaterial = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [Tooltip("Show a wireframe around the bounding box when checked. Wireframe parameters below have no effect unless this is checked")]
        private bool showWireframe = true;

        /// <summary>
        /// Show a wireframe around the bounding box when checked. Wireframe parameters below have no effect unless this is checked
        /// </summary>
        public bool ShowWireFrame
        {
            get { return showWireframe; }
            set
            {
                if (showWireframe != value)
                {
                    showWireframe = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [Tooltip("Shape used for wireframe display")]
        private WireframeType wireframeShape = WireframeType.Cubic;

        /// <summary>
        /// Shape used for wireframe display
        /// </summary>
        public WireframeType WireframeShape
        {
            get { return wireframeShape; }
            set
            {
                if (wireframeShape != value)
                {
                    wireframeShape = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [Tooltip("Material used for wireframe display")]
        private Material wireframeMaterial;

        /// <summary>
        /// Material used for wireframe display
        /// </summary>
        public Material WireframeMaterial
        {
            get { return wireframeMaterial; }
            set
            {
                if (wireframeMaterial != value)
                {
                    wireframeMaterial = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [FormerlySerializedAs("linkRadius")]
        [Tooltip("Radius for wireframe edges")]
        private float wireframeEdgeRadius = 0.001f;

        /// <summary>
        /// Radius for wireframe edges
        /// </summary>
        public float WireframeEdgeRadius
        {
            get { return wireframeEdgeRadius; }
            set
            {
                if (wireframeEdgeRadius != value)
                {
                    wireframeEdgeRadius = value;
                    CreateRig();
                }
            }
        }

        [Header("Handles")]
        [SerializeField]
        [Tooltip("Material applied to handles when they are not in a grabbed state")]
        private Material handleMaterial;

        /// <summary>
        /// Material applied to handles when they are not in a grabbed state
        /// </summary>
        public Material HandleMaterial
        {
            get { return handleMaterial; }
            set
            {
                if (handleMaterial != value)
                {
                    handleMaterial = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [Tooltip("Material applied to handles while they are a grabbed")]
        private Material handleGrabbedMaterial;

        /// <summary>
        /// Material applied to handles while they are a grabbed
        /// </summary>
        public Material HandleGrabbedMaterial
        {
            get { return handleGrabbedMaterial; }
            set
            {
                if (handleGrabbedMaterial != value)
                {
                    handleGrabbedMaterial = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [Tooltip("Prefab used to display scale handles in corners. If not set, boxes will be displayed instead")]
        GameObject scaleHandlePrefab = null;

        /// <summary>
        /// Prefab used to display scale handles in corners. If not set, boxes will be displayed instead
        /// </summary>
        public GameObject ScaleHandlePrefab
        {
            get { return scaleHandlePrefab; }
            set
            {
                if (scaleHandlePrefab != value)
                {
                    scaleHandlePrefab = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [Tooltip("Prefab used to display scale handles in corners for 2D slate. If not set, boxes will be displayed instead")]
        GameObject scaleHandleSlatePrefab = null;

        /// <summary>
        /// Prefab used to display scale handles in corners for 2D slate. If not set, boxes will be displayed instead
        /// </summary>
        public GameObject ScaleHandleSlatePrefab
        {
            get { return scaleHandleSlatePrefab; }
            set
            {
                if (scaleHandleSlatePrefab != value)
                {
                    scaleHandleSlatePrefab = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [FormerlySerializedAs("cornerRadius")]
        [Tooltip("Size of the cube collidable used in scale handles")]
        private float scaleHandleSize = 0.016f; // 1.6cm default handle size

        /// <summary>
        /// Size of the cube collidable used in scale handles
        /// </summary>
        public float ScaleHandleSize
        {
            get { return scaleHandleSize; }
            set
            {
                if (scaleHandleSize != value)
                {
                    scaleHandleSize = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [Tooltip("Additional padding to apply to the collider on scale handle to make handle easier to hit")]
        private Vector3 scaleHandleColliderPadding = new Vector3(0.016f, 0.016f, 0.016f);

        /// <summary>
        /// Additional padding to apply to the collider on scale handle to make handle easier to hit
        /// </summary>
        public Vector3 ScaleHandleColliderPadding
        {
            get { return scaleHandleColliderPadding; }
            set
            {
                if (scaleHandleColliderPadding != value)
                {
                    scaleHandleColliderPadding = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [Tooltip("Prefab used to display rotation handles in the midpoint of each edge. Aligns the Y axis of the prefab with the pivot axis, and the X and Z axes pointing outward. If not set, spheres will be displayed instead")]
        private GameObject rotationHandlePrefab = null;

        /// <summary>
        /// Prefab used to display rotation handles in the midpoint of each edge. Aligns the Y axis of the prefab with the pivot axis, and the X and Z axes pointing outward. If not set, spheres will be displayed instead
        /// </summary>
        public GameObject RotationHandleSlatePrefab
        {
            get { return rotationHandlePrefab; }
            set
            {
                if (rotationHandlePrefab != value)
                {
                    rotationHandlePrefab = value;
                    CreateRig();
                }
            }
        }
        [SerializeField]
        [FormerlySerializedAs("ballRadius")]
        [Tooltip("Radius of the handle geometry of rotation handles")]
        private float rotationHandleSize = 0.016f; // 1.6cm default handle size

        /// <summary>
        /// Radius of the handle geometry of rotation handles
        /// </summary>
        public float RotationHandleSize
        {
            get { return rotationHandleSize; }
            set
            {
                if (rotationHandleSize != value)
                {
                    rotationHandleSize = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [Tooltip("Additional padding to apply to the collider on rotate handle to make handle easier to hit")]
        private Vector3 rotateHandleColliderPadding = new Vector3(0.016f, 0.016f, 0.016f);

        /// <summary>
        /// Additional padding to apply to the collider on rotate handle to make handle easier to hit
        /// </summary>
        public Vector3 RotateHandleColliderPadding
        {
            get { return rotateHandleColliderPadding; }
            set
            {
                if (rotateHandleColliderPadding != value)
                {
                    rotateHandleColliderPadding = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [Tooltip("Determines the type of collider that will surround the rotation handle prefab.")]
        private RotationHandlePrefabCollider rotationHandlePrefabColliderType = RotationHandlePrefabCollider.Box;

        /// <summary>
        /// Determines the type of collider that will surround the rotation handle prefab.
        /// </summary>
        public RotationHandlePrefabCollider RotationHandlePrefabColliderType
        {
            get
            {
                return rotationHandlePrefabColliderType;
            }
            set
            {
                if (rotationHandlePrefabColliderType != value)
                {
                    rotationHandlePrefabColliderType = value;
                    CreateRig();
                }
            }
        }

        [SerializeField]
        [Tooltip("Check to show scale handles")]
        private bool showScaleHandles = true;

        /// <summary>
        /// Public property to Set the visibility of the corner cube Scaling handles.
        /// This property can be set independent of the Rotate handles.
        /// </summary>
        public bool ShowScaleHandles
        {
            get
            {
                return showScaleHandles;
            }
            set
            {
                if (showScaleHandles != value)
                {
                    showScaleHandles = value;
                    ResetHandleVisibility();
                }
            }
        }

        [SerializeField]
        [Tooltip("Check to show rotation handles for the X axis")]
        private bool showRotationHandleForX = true;

        /// <summary>
        /// Check to show rotation handles for the X axis
        /// </summary>
        public bool ShowRotationHandleForX
        {
            get
            {
                return showRotationHandleForX;
            }
            set
            {
                if (showRotationHandleForX != value)
                {
                    showRotationHandleForX = value;
                    ResetHandleVisibility();
                }
            }
        }

        [SerializeField]
        [Tooltip("Check to show rotation handles for the Y axis")]
        private bool showRotationHandleForY = true;

        /// <summary>
        /// Check to show rotation handles for the Y axis
        /// </summary>
        public bool ShowRotationHandleForY
        {
            get
            {
                return showRotationHandleForY;
            }
            set
            {
                if (showRotationHandleForY != value)
                {
                    showRotationHandleForY = value;
                    ResetHandleVisibility();
                }
            }
        }

        [SerializeField]
        [Tooltip("Check to show rotation handles for the Z axis")]
        private bool showRotationHandleForZ = true;

        /// <summary>
        /// Check to show rotation handles for the Z axis
        /// </summary>
        public bool ShowRotationHandleForZ
        {
            get
            {
                return showRotationHandleForZ;
            }
            set
            {
                if (showRotationHandleForZ != value)
                {
                    showRotationHandleForZ = value;
                    ResetHandleVisibility();
                }
            }
        }

        [SerializeField]
        [Tooltip("Check to draw a tether point from the handles to the hand when manipulating.")]
        private bool drawTetherWhenManipulating = true;

        /// <summary>
        /// Check to draw a tether point from the handles to the hand when manipulating.
        /// </summary>
        public bool DrawTetherWhenManipulating
        {
            get
            {
                return drawTetherWhenManipulating;
            }
            set
            {
                drawTetherWhenManipulating = value;
            }
        }

        [Header("Proximity")]
        [SerializeField]
        [Tooltip("Determines whether proximity feature (scaling and material toggling) for bounding box handles is activated")]
        private bool proximityEffectActive = false;

        /// <summary>
        /// Determines whether proximity feature (scaling and material toggling) for bounding box handles is activated
        /// </summary>
        public bool ProximityEffectActive
        {
            get
            {
                return proximityEffectActive;
            }
            set
            {
                proximityEffectActive = value;
            }
        }

        [SerializeField]
        [Tooltip("How far away should the hand be from a handle before it starts scaling the handle?")]
        [Range(0.005f, 0.2f)]
        /// <summary>
        /// Distance between handle and hand before proximity scaling will be triggered.
        /// </summary>
        private float handleMediumProximity = 0.1f;

        [SerializeField]
        [Tooltip("How far away should the hand be from a handle before it activates the close-proximity scaling effect?")]
        [Range(0.001f, 0.1f)]
        /// <summary>
        /// Distance between handle and hand that will trigger the close proximity effect.
        /// </summary>
        private float handleCloseProximity = 0.03f;

        [SerializeField]
        [Tooltip("A Proximity-enabled Handle scales by this amount when a hand moves out of range. Default is 0, invisible handle.")]
        private float farScale = 0.0f;

        /// <summary>
        /// A Proximity-enabled Handle scales by this amount when a hand moves out of range. Default is 0, invisible handle.
        /// </summary>
        public float FarScale
        {
            get
            {
                return farScale;
            }
            set
            {
                farScale = value;
            }
        }

        [SerializeField]
        [Tooltip("A Proximity-enabled Handle scales by this amount when a hand moves into the Medium Proximity range. Default is 1.0, original handle size.")]
        private float mediumScale = 1.0f;

        /// <summary>
        /// A Proximity-enabled Handle scales by this amount when a hand moves into the Medium Proximity range. Default is 1.0, original handle size.
        /// </summary>
        public float MediumScale
        {
            get
            {
                return mediumScale;
            }
            set
            {
                mediumScale = value;
            }
        }

        [SerializeField]
        [Tooltip("A Proximity-enabled Handle scales by this amount when a hand moves into the Close Proximity range. Default is 1.5, larger handle size.")]
        private float closeScale = 1.5f;

        /// <summary>
        /// A Proximity-enabled Handle scales by this amount when a hand moves into the Close Proximity range. Default is 1.5, larger handle size
        /// </summary>
        public float CloseScale
        {
            get
            {
                return closeScale;
            }
            set
            {
                closeScale = value;
            }
        }

        [SerializeField]
        [Tooltip("At what rate should a Proximity-scaled Handle scale when the Hand moves from Medium proximity to Far proximity?")]
        [Range(0.0f, 1.0f)]
        /// <summary>
        /// Scaling animation velocity from medium to far proximity state.
        /// </summary>
        private float farGrowRate = 0.3f;

        [SerializeField]
        [Tooltip("At what rate should a Proximity-scaled Handle scale when the Hand moves to a distance that activates Medium Scale ?")]
        [Range(0.0f, 1.0f)]
        /// <summary>
        /// Scaling animation velocity from far to medium proximity.
        /// </summary>
        private float mediumGrowRate = 0.2f;

        [SerializeField]
        [Tooltip("At what rate should a Proximity-scaled Handle scale when the Hand moves to a distance that activates Close Scale ?")]
        [Range(0.0f, 1.0f)]
        /// <summary>
        /// Scaling animation velocity from medium to close proximity.
        /// </summary>
        private float closeGrowRate = 0.3f;

        [SerializeField]
        [Tooltip("Add a Collider here if you do not want the handle colliders to interact with another object's collider.")]
        private Collider handlesIgnoreCollider = null;

        /// <summary>
        /// Add a Collider here if you do not want the handle colliders to interact with another object's collider.
        /// </summary>
        public Collider HandlesIgnoreCollider
        {
            get
            {
                return handlesIgnoreCollider;
            }
            set
            {
                handlesIgnoreCollider = value;
            }
        }

        [Header("Debug")]
        [Tooltip("Debug only. Component used to display debug messages")]
        /// <summary>
        /// Debug only. Component used to display debug messages
        /// </summary>
        public TextMesh debugText;

        [SerializeField]
        [Tooltip("Determines whether to hide GameObjects (i.e handles, links etc) created and managed by this component in the editor")]
        private bool hideElementsInInspector = true;

        /// <summary>
        /// Determines whether to hide GameObjects (i.e handles, links etc) created and managed by this component in the editor
        /// </summary>
        public bool HideElementsInInspector
        {
            get { return hideElementsInInspector; }
            set
            {
                if (hideElementsInInspector != value)
                {
                    hideElementsInInspector = value;
                    UpdateRigVisibilityInInspector();
                }
            }
        }

        private void UpdateRigVisibilityInInspector()
        {
        // BUG: Container contents are never accessed
        // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
        //             HideFlags desiredFlags = hideElementsInInspector ? HideFlags.HideInHierarchy | HideFlags.HideInInspector : HideFlags.None;
        //             if (corners != null)
        //             {
        //                 foreach (var cube in corners)
        //                 {
        //                     cube.hideFlags = desiredFlags;
        //                 }
        //             }
        // 
        //             if (boxDisplay != null)
        //             {
        //                 boxDisplay.hideFlags = desiredFlags;
        //             }
        // 
        //             if (rigRoot != null)
        //             {
        //                 rigRoot.hideFlags = desiredFlags;
        //             }
        // 
        //             if (links != null)
        //             {
        //                 foreach (var link in links)
        //                 {
        //                     link.hideFlags = desiredFlags;
        //                 }
        //             }
        //         }
        // 
        //         [Header("Events")]
        //         /// <summary>
        //         /// Event that gets fired when interaction with a rotation handle starts.
        //         /// </summary>
        //         public UnityEvent RotateStarted = new UnityEvent();
        //         /// <summary>
        //         /// Event that gets fired when interaction with a rotation handle stops.
        //         /// </summary>
        //         public UnityEvent RotateStopped = new UnityEvent();
        //         /// <summary>
        //         /// Event that gets fired when interaction with a scale handle starts.
        //         /// </summary>
        //         public UnityEvent ScaleStarted = new UnityEvent();
        //         /// <summary>
        //         /// Event that gets fired when interaction with a scale handle stops.
        //         /// </summary>
        //         public UnityEvent ScaleStopped = new UnityEvent();
        //         #endregion Serialized Fields
        // 
        //         #region Private Fields
        // 
        //         // Whether we should be displaying just the wireframe (if enabled) or the handles too
        //         private bool wireframeOnly = false;
        // 
        //         // Pointer that is being used to manipulate the bounding box
        //         private IMixedRealityPointer currentPointer;
        // 
        //         private Transform rigRoot;
        // 
        //         // Game object used to display the bounding box. Parented to the rig root
        //         private GameObject boxDisplay;
        // 
        //         private Vector3[] boundsCorners;
        // 
        //         // Half the size of the current bounds
        //         private Vector3 currentBoundsExtents;
        // 
        //         private IMixedRealityEyeGazeProvider EyeTrackingProvider => eyeTrackingProvider ?? (eyeTrackingProvider = CoreServices.InputSystem?.EyeGazeProvider);
        //         private IMixedRealityEyeGazeProvider eyeTrackingProvider = null;
        // 

        // FIXED VERSION:

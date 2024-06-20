/************************************************************************************

Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.  

************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(OVROverlay))]
public class OVROverlayMeshGenerator : MonoBehaviour {

    private Mesh _Mesh;
    private List<Vector3> _Verts = new List<Vector3>();
    private List<Vector2> _UV = new List<Vector2>();
    private List<int> _Tris = new List<int>();
    private OVROverlay _Overlay;
    private MeshFilter _MeshFilter;
    private MeshCollider _MeshCollider;
    private Transform _CameraRoot;
    private Transform _Transform;

    private OVROverlay.OverlayShape _LastShape;
    private Vector3 _LastPosition;
    private Quaternion _LastRotation;
    private Vector3 _LastScale;
    private Rect _LastRectLeft;
    private Rect _LastRectRight;

    private bool _Awake = false;

    protected void Awake()
    {
        _Overlay = GetComponent<OVROverlay>();
        _MeshFilter = GetComponent<MeshFilter>();
        _MeshCollider = GetComponent<MeshCollider>();

        _Transform = transform;
        if (Camera.main && Camera.main.transform.parent)
        {
            _CameraRoot = Camera.main.transform.parent;
        }

        #if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.EditorApplication.update += Update;
        }
        #endif

        _Awake = true;
    }

    private Rect GetBoundingRect(Rect a, Rect b)
    {
        float xMin = Mathf.Min(a.x, b.x);
        float xMax = Mathf.Max(a.x + a.width, b.x + b.width);
        float yMin = Mathf.Min(a.y, b.y);
        float yMax = Mathf.Max(a.y + a.height, b.y + b.height);

        return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
    }

    private void Update()
    {
            // BUG: Using New() allocation in Update() method.
            // MESSAGE: Update() method is called each frame. It's efficient to allocate new resource using New() in Update() method.
            //         if (!_Awake)
            //         {
            //             Awake();
            //         }
            // 
            //         if (_Overlay)
            //         {
            //             OVROverlay.OverlayShape shape = _Overlay.currentOverlayShape;
            //             Vector3 position = _CameraRoot ? (_Transform.position - _CameraRoot.position) : _Transform.position;
            //             Quaternion rotation = _Transform.rotation;
            //             Vector3 scale = _Transform.lossyScale;
            //             Rect rectLeft = _Overlay.overrideTextureRectMatrix ? _Overlay.destRectLeft : new Rect(0,0,1,1);
            //             Rect rectRight = _Overlay.overrideTextureRectMatrix ? _Overlay.destRectRight : new Rect(0,0,1,1);
            // 
            //             if (_Mesh == null || _LastShape != shape || _LastPosition != position || _LastRotation != rotation || _LastScale != scale || _LastRectLeft != rectLeft || _LastRectRight != rectRight)
            //             {
            //                 UpdateMesh(shape, position, rotation, scale, GetBoundingRect(rectLeft, rectRight));
            //                 _LastShape = shape;
            //                 _LastPosition = position;
            //                 _LastRotation = rotation;
            //                 _LastScale = scale;
            //                 _LastRectLeft = rectLeft;
            //                 _LastRectRight = rectRight;
            //             }
            //         }
            //     }

            // FIXED VERSION:

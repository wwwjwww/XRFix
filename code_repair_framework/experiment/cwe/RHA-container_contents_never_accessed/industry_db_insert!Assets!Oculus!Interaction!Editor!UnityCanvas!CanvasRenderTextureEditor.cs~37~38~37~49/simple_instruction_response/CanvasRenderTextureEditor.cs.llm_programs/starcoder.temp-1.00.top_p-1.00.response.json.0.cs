

using Oculus.Interaction.Editor;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

using props = Oculus.Interaction.UnityCanvas.CanvasRenderTexture.Properties;

namespace Oculus.Interaction.UnityCanvas.Editor
{
    [CustomEditor(typeof(CanvasRenderTexture))]
    public class CanvasRenderTextureEditor : EditorBase
    {
        private static List<CanvasRenderer> _tmpRenderers = new List<CanvasRenderer>();
        
        
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class CanvasRenderTexture : MonoBehaviour
{
    public enum RenderType
    {
        ScreenSpaceCanvas,
        WorldSpaceCanvas,
    }

    public RenderTexture targetTexture;
    public RenderType renderType = RenderType.ScreenSpaceCanvas;
    public Camera renderCamera;
    public Camera uiCamera;

    private RectTransform _rectTransform;
    private RenderTexture _renderTexture;

    private List<CanvasRenderer> _canvasRenderers = new List<CanvasRenderer>();

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();

        _renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        targetTexture = _renderTexture;

        _canvasRenderers.AddRange(GetComponentsInChildren<CanvasRenderer>(true));
        SetRenderingOrder();
    }

    private void OnEnable()
    {
        if (renderCamera == null)
        {
            renderCamera = Camera.main;
        }

        if (uiCamera == null)
        {
            uiCamera = Camera.main;
        }
    }

    private void LateUpdate()
    {
        UpdateCamera();
        RenderTargetTexture();
    }

    private void UpdateCamera()
    {
        if (renderType == RenderType.ScreenSpaceCanvas)
        {
            renderCamera.transform.SetAsFirstSibling();
            renderCamera.targetTexture = _renderTexture;
        }
        else if (renderType == RenderType.WorldSpaceCanvas)
        {
            renderCamera.transform.SetAsLastSibling();
            renderCamera.targetTexture = null;
        }
    }

    private void RenderTargetTexture()
    {
        foreach (var canvasRenderer in _canvasRenderers)
        {
            if (canvasRenderer!= null)
            {
                _tmpRenderers.Add(canvasRenderer);
            }
        }

        foreach (var canvasRenderer in _tmpRenderers)
        {
            canvasRenderer.SetToActive();

            int renderOrder = canvasRenderer.sortingOrder;
            int screenWidth = Screen.width;
            int screenHeight = Screen.height;

            // Determine width and height of the render texture based on the aspect ratio of the canvas element
            Vector2 aspectRatio = GetAspectRatio(renderCamera, _rectTransform, screenWidth, screenHeight);
            float width = aspectRatio.x * (screenWidth / 100f);
            float height = aspectRatio.y * (screenHeight / 100f);

            RenderTexture renderTexture = new RenderTexture((int)width, (int)height, 24);
            Graphics.SetRenderTarget(renderTexture);
            GL.Clear(true, true, Color.clear);

            switch (renderType)
            {
                case RenderType.ScreenSpaceCanvas:
                    // Render the canvas into the render texture using the render camera
                    Vector3 position = _rectTransform.position;
                    Quaternion rotation = _rectTransform.rotation;
                    Camera camera = renderCamera;
                    camera.transform.position = position;
                    camera.transform.rotation = rotation;
                    canvasRenderer.Render();
                    break;
                case RenderType.WorldSpaceCanvas:
                    // Render the canvas into the render texture using the main camera
                    Camera uiCamera = this.uiCamera;
                    Camera.main.CopyFrom(uiCamera);
                    Camera.main.transform.position = Camera.main.worldToCameraMatrix * new Vector3(0f, 0f, -10f);
                    Camera.main.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                    Camera.main.orthographic = true;
                    Camera.main.orthographicSize = height / 2f;
                    canvasRenderer.Render();
                    Camera.main.targetTexture = null;
                    break;
            }

            RenderTexture.active = null;
            DestroyImmediate(renderTexture);
            canvasRenderer.SetToInactive();

            // Update the sorting order of the canvas renderer to match its index in the list
            canvasRenderer.sortingOrder = renderOrder;
        }

        _tmpRenderers.Clear();
    }

    private static Vector2 GetAspectRatio(Camera camera, RectTransform rectTransform, int screenWidth, int screenHeight)
    {
        // Determine the aspect ratio of the render texture based on the aspect ratio of the canvas element
        float elementAspectRatio = rectTransform.sizeDelta.x / rectTransform.sizeDelta.y;
        float cameraPixelWidth = camera.pixelWidth;
        float cameraPixelHeight = camera.pixelHeight;

        if (cameraPixelWidth < cameraPixelHeight)
        {
            // Adjust the aspect ratio of the camera to match the aspect ratio of the render texture
            float adjustedAspectRatio = screenWidth / (float)screenHeight;
            float scale = adjustedAspectRatio / elementAspectRatio;
            cameraPixelHeight *= scale;
            cameraPixelWidth = cameraPixelHeight * elementAspectRatio;
        }
        else
        {
            // Adjust the aspect ratio of the camera to match the aspect ratio of the render texture
            float adjustedAspectRatio = screenHeight / (float)screenWidth;
            float scale = adjustedAspectRatio / elementAspectRatio;
            cameraPixelWidth *= scale;
            cameraPixelHeight = cameraPixelWidth * elementAspectRatio;
        }

        return new Vector2(cameraPixelWidth, cameraPixelHeight);
    }

    private void SetRenderingOrder()
    {
        int index = 0;
        foreach (var canvasRenderer in _canvasRenderers)
        {
            canvasRenderer.sortingOrder = index++;
        }
    }
}


        public new CanvasRenderTexture target
        {
            get
            {
                return base.target as CanvasRenderTexture;
            }
        }

        protected override void OnEnable()
        {
            var canvasProp = serializedObject.FindProperty(props.Canvas);

            Draw(props.Resolution, props.DimensionDriveMode, (resProp, modeProp) =>
            {
                Rect rect = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight);

                Rect labelRect = rect;
                labelRect.width = EditorGUIUtility.labelWidth;

                Rect dropdownRect = rect;
                dropdownRect.x = rect.xMax - 70;
                dropdownRect.width = 70;

                Rect contentRect = rect;
                contentRect.xMin = labelRect.xMax;
                contentRect.xMax = dropdownRect.xMin;

                GUI.Label(labelRect, resProp.displayName);

                if (modeProp.intValue == (int)CanvasRenderTexture.DriveMode.Auto && canvasProp.objectReferenceValue != null)
                {
                    using (new EditorGUI.DisabledScope(true))
                    {
                        EditorGUI.Vector2IntField(contentRect, GUIContent.none, target.CalcAutoResolution());
                    }
                }
                else
                {
                    resProp.vector2IntValue = EditorGUI.Vector2IntField(contentRect, GUIContent.none, resProp.vector2IntValue);
                }

                EditorGUI.PropertyField(dropdownRect, modeProp, GUIContent.none);
            });

            Draw(props.PixelsPerUnit, props.GenerateMipMaps, (pixelsPerUnit, mipmaps) =>
            {
                var driveMode = serializedObject.FindProperty(props.DimensionDriveMode);
                if (driveMode.intValue == (int)CanvasRenderTexture.DriveMode.Auto)
                {
                    EditorGUILayout.PropertyField(pixelsPerUnit);
                }
                EditorGUILayout.PropertyField(mipmaps);
            });
        }

        protected override void OnBeforeInspector()
        {
            base.OnBeforeInspector();

            bool isEmpty;

            AutoFix(AutoFixIsUsingScreenSpaceCanvas(), AutoFixSetToWorldSpaceCamera, "The OverlayRenderer only supports Canvases that are set to World Space.");

            AutoFix(isEmpty = AutoFixIsMaskEmpty(), AutoFixAssignUIToMask, "The rendering Mask is empty, it needs to contain at least one layer for rendering to function.");

            if (!isEmpty)
            {
                AutoFix(AutoFixAnyCamerasRenderingTargetLayers(), AutoFixRemoveRenderingMaskFromCameras, "Some cameras are rendering using a layer that is specified here as a Rendering layer. This can cause the UI to be rendered twice.");
                AutoFix(AutoFixAnyRenderersOnUnrenderedLayers(), AutoFixMoveRenderersToMaskedLayers, "Some CanvasRenderers are using a layer that is not included in the rendered LayerMask.");
            }
        }

        #region AutoFix

        private bool AutoFix(bool needsFix, Action fixAction, string message)
        {
            if (needsFix)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.HelpBox(message, MessageType.Warning);
                    if (GUILayout.Button("Auto-Fix", GUILayout.ExpandHeight(true)))
                    {
                        fixAction();
                    }
                }
            }

            return needsFix;
        }

        private bool AutoFixIsUsingScreenSpaceCanvas()
        {
            Canvas canvas = target.GetComponent<Canvas>();
            if (canvas == null)
            {
                return false;
            }

            return canvas.renderMode != RenderMode.WorldSpace;
        }

        private void AutoFixSetToWorldSpaceCamera()
        {
            Canvas canvas = target.GetComponent<Canvas>();
            if (canvas != null)
            {
                Undo.RecordObject(canvas, "Set Canvas To World Space");
                canvas.renderMode = RenderMode.WorldSpace;
            }
        }

        private bool AutoFixIsMaskEmpty()
        {
            var layerProp = serializedObject.FindProperty(props.RenderLayers);
            return layerProp.intValue == 0;
        }

        public void AutoFixAssignUIToMask()
        {
            Undo.RecordObject(target, "Set Overlay Mask");
            var layerProp = serializedObject.FindProperty(props.RenderLayers);
            layerProp.intValue = CanvasRenderTexture.DEFAULT_UI_LAYERMASK;
            serializedObject.ApplyModifiedProperties();
        }

        private bool AutoFixAnyRenderersOnUnrenderedLayers()
        {
            var canvasProp = serializedObject.FindProperty(props.Canvas);
            Canvas canvas = canvasProp.objectReferenceValue as Canvas;

            if (canvas == null)
            {
                return false;
            }

            canvas.gameObject.GetComponentsInChildren(_tmpRenderers);
            foreach (var renderer in _tmpRenderers)
            {
                int layer = renderer.gameObject.layer;
                if (((1 << layer) & target.RenderingLayers) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void AutoFixMoveRenderersToMaskedLayers()
        {
            var canvasProp = serializedObject.FindProperty(props.Canvas);
            Canvas canvas = canvasProp.objectReferenceValue as Canvas;

            if (canvas == null)
            {
                return;
            }

            var maskedLayers = AutoFixGetMaskedLayers();
            var targetLayer = maskedLayers.FirstOrDefault();

            canvas.gameObject.GetComponentsInChildren(_tmpRenderers);
            foreach (var renderer in _tmpRenderers)
            {
                int layer = renderer.gameObject.layer;
                if ((layer & target.RenderingLayers) == 0)
                {
                    Undo.RecordObject(renderer.gameObject, "Set Overlay Layer");
                    renderer.gameObject.layer = targetLayer;
                }
            }
        }

        private bool AutoFixAnyCamerasRenderingTargetLayers()
        {
            var cameras = FindObjectsOfType<Camera>();
            foreach (var camera in cameras)
            {
                //Ignore the special camera we create to render to the overlay
                if (camera == target.OverlayCamera)
                {
                    continue;
                }

                if ((camera.cullingMask & target.RenderingLayers) != 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void AutoFixRemoveRenderingMaskFromCameras()
        {
            var cameras = FindObjectsOfType<Camera>();
            foreach (var camera in cameras)
            {
                Undo.RecordObject(camera, "Set Camera Culling Mask");
                camera.cullingMask = camera.cullingMask & ~target.RenderingLayers;
            }
        }

        private List<int> AutoFixGetMaskedLayers()
        {
            List<int> maskedLayers = new List<int>();
            for (int i = 0; i < 32; i++)
            {
                if ((target.RenderingLayers & (1 << i)) != 0)
                {
                    maskedLayers.Add(i);
                }
            }
            return maskedLayers;
        }
        #endregion
    }
}

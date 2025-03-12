

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
        // BUG: Container contents are never accessed
        // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
        //         private static List<Graphic> _tmpGraphics = new List<Graphic>();

        // FIXED CODE:
        using System.Linq;
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
        
        private void OnEnable()
        {
            if (!EditorApplication.isPlaying &&!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                RefreshReceivers();
            }
        }

        private void OnDisable()
        {
            if (!EditorApplication.isPlaying &&!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                DisposeReceivers();
            }
        }
        
        private void RefreshReceivers()
        {
            _tmpRenderers.Clear();
            _tmpRenderers.AddRange(target.GetComponentsInChildren<CanvasRenderer>(true));
        }

        private void DisposeReceivers()
        {
            foreach (var r in _tmpRenderers)
            {
                UnityEngine.Object.DestroyImmediate(r);
            }

            _tmpRenderers.Clear();
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!Application.isPlaying && GUILayout.Button("Apply Modifications"))
            {
                RefreshReceivers();
                serializedObject.Update();

                ApplyTargeting();
                ApplyClipping();
                ApplySortingOrder();
                ApplyAnchor();

                serializedObject.ApplyModifiedProperties();
                DisposeReceivers();
            }
        }
        
        private void ApplyTargeting()
        {
            var receivers = _tmpRenderers.Where(r => r.GetTargetScreenRectTransform()!= null).ToList();
            if (receivers.Count > 0)
            {
                var renderTexture = target as CanvasRenderTexture;
                foreach (var r in receivers)
                {
                    r.SetTargetScreenRectTransform(renderTexture.GetComponent<RectTransform>());
                }
            }
        }
        
        private void ApplyClipping()
        {
            var receivers = _tmpRenderers.Where(r => r.GetClippingSoftness()!= 0).ToList();
            if (receivers.Count > 0)
            {
                var renderTexture = target as CanvasRenderTexture;
                var pixelsPerUnit = renderTexture.pixelsPerUnit;
                var scaleFactor = renderTexture.renderMode == RenderMode.ScreenSpaceOverlay? pixelsPerUnit : 1;
                foreach (var r in receivers)
                {
                    r.SetClippingSoftness(new Vector2(0.5f / scaleFactor, 0.5f / scaleFactor));
                }
            }
        }
        
        private void ApplySortingOrder()
        {
            var receivers = _tmpRenderers.Where(r => r.sortingOrder!= 0).ToList();
            if (receivers.Count > 0)
            {
                receivers.Sort((r1, r2) => r1.sortingOrder.CompareTo(r2.sortingOrder));
                int index = 0;
                foreach (var r in receivers)
                {
                    r.sortingOrder = index++;
                }
            }
        }
        
        private void ApplyAnchor()
        {
            var receiv

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

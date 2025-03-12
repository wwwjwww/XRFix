/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https:
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
        /* BUG: Container contents are never accessed
        * MESSAGE: A collection or map whose contents are never queried or accessed is useless.
        *         private static List<Graphic> _tmpGraphics = new List<Graphic>();

        * Remove or Commented-out the collection if it is no longer needed
        * FIXED CODE:
        */
        Here is my fixed code:

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

        private SerializedProperty _uiScaleModeProp;
        private SerializedProperty _renderModeProp;
        private SerializedProperty _pixelPerfectProp;
        private SerializedProperty _sortingOrderProp;
        private SerializedProperty _clipRectProp;
        private SerializedProperty _overrideScaleModeProp;
        private SerializedProperty _uiScaleProp;
        private SerializedProperty _renderTextureProp;
        private SerializedProperty _camerasProp;
        private SerializedProperty _transformProp;
        private SerializedProperty _raycastTargetProp;
        private SerializedProperty _isPixelPerfect;
        private SerializedProperty _renderMode;
        private SerializedProperty _sortingOrder;
        private SerializedProperty _pixelPerfect;
        private SerializedProperty _scaleMode;
        private SerializedProperty _overrideScaleMode;
        private SerializedProperty _scaleFactor;
        private SerializedProperty _referencePixelsPerUnit;
        private SerializedProperty _drawAtScale;
        private SerializedProperty _pixelRect;
        private SerializedProperty _width;
        private SerializedProperty _height;
        private SerializedProperty _center;
        private SerializedProperty _renderTexture;
        private SerializedProperty _canvasRectTransform;

        private bool _isFoldoutPixelPerfect = true;
        private bool _isFoldoutRenderTexture = true;
        private bool _isFoldoutSorting = true;
        private bool _isFoldoutCameras = true;

        private void OnEnable()
        {
            _uiScaleModeProp = serializedObject.FindProperty(props.uiScaleMode.ToString());
            _renderModeProp = serializedObject.FindProperty(props.renderMode.ToString());
            _pixelPerfectProp = serializedObject.FindProperty(props.pixelPerfect.ToString());
            _sortingOrderProp = serializedObject.FindProperty(props.sortingOrder.ToString());
            _clipRectProp = serializedObject.FindProperty(props.clipRect.ToString());
            _overrideScaleModeProp = serializedObject.FindProperty(props.overrideScaleMode.ToString());
            _uiScaleProp = serializedObject.FindProperty(props.uiScaleProp.ToString());
            _renderTextureProp = serializedObject.FindProperty(props.renderTextureProp.ToString());
            _camerasProp = serializedObject.FindProperty(props.camerasProp.ToString());
            _transformProp = serializedObject.FindProperty(props.transformProp.ToString());
            _raycastTargetProp = serializedObject.FindProperty(props.raycastTargetProp.ToString());
            _isPixelPerfect = serializedObject.FindProperty(props.isPixelPerfect.ToString());
            _renderMode = serializedObject.FindProperty(props.renderMode.ToString());
            _sortingOrder = serializedObject.FindProperty(props.sortingOrder.ToString());
            _pixelPerfect = serializedObject.FindProperty(props.pixelPerfect.ToString());
            _scaleMode = serializedObject.FindProperty(props.scaleMode.ToString());
            _overrideScaleMode = serializedObject.FindProperty(props.overrideScaleMode.ToString());
            _scaleFactor = serializedObject.FindProperty(props.scaleFactor.ToString());
            _referencePixelsPerUnit = serializedObject.FindProperty(props.referencePixelsPerUnit.ToString());
            _drawAtScale = serializedObject.FindProperty(props.drawAtScale.ToString());
            _pixelRect = serializedObject.FindProperty(props.pixelRect.ToString());
            _width = serializedObject.FindProperty(props.width.ToString());
            _height = serializedObject.FindProperty(props.height.ToString());
            _center = serializedObject.FindProperty(props.center.ToString());
            _renderTexture = serializedObject.FindProperty(props.renderTexture.ToString());
            _canvasRectTransform = serializedObject.FindProperty(props.canvasRectTransform.ToString());
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_uiScaleModeProp);
            EditorGUILayout.PropertyField(_renderModeProp);
            EditorGUILayout.PropertyField(_pixelPerfectProp);
            EditorGUILayout.Space();

            _isFoldoutPixelPerfect = EditorGUILayout.Foldout(_isFoldoutPixelPerfect, "Pixel Perfect");
            if (_isFoldoutPixelPerfect)
            {
                EditorGUILayout.PropertyField(_overrideScaleModeProp);
                if (!_overrideScaleMode.boolValue)
                {
                    _uiScaleProp.intValue = EditorGUILayout.IntSlider(new GUIContent("UI Scale"), _uiScaleProp.intValue, 1, 10);
                }
                else
                {
                    EditorGUILayout.PropertyField(_scaleMode);
                    switch (_scaleMode.enumValueIndex)
                    {
                        case 0: // Scale With Screen Size
                            _uiScaleProp.floatValue = EditorGUILayout.FloatField(new GUIContent("Scale Factor"), _uiScaleProp.floatValue);
                            break;
                        case 1: // Constant Scale
                            EditorGUILayout.PropertyField(_scaleFactor);
                            break;
                        case 2: // Scale With Reference Size
                            _referencePixelsPerUnit.floatValue = EditorGUILayout.FloatField(new GUIContent("Reference Pixels Per Unit"), _referencePixelsPerUnit.floatValue);
                            _drawAtScale.boolValue = EditorGUILayout.Toggle(new GUIContent("Draw At Scale"), _drawAtScale.boolValue);
                            break;
                    }
                }
                EditorGUILayout.PropertyField(_drawAtScale);
            }
            EditorGUILayout.Space();

            _isFoldoutRenderTexture = EditorGUILayout.Foldout(_isFoldoutRenderTexture, "Render Texture");
            if (_isFoldoutRenderTexture)
            {
                EditorGUILayout.PropertyField(_renderTextureProp);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(_camerasProp, true);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(_transformProp);
                EditorGUILayout.PropertyField(_raycastTargetProp);
            }
            EditorGUILayout.Space();

            _isFoldoutSorting = EditorGUILayout.Foldout(_isFoldoutSorting, "Sorting");
            if (_isFoldoutSorting)
            {
                EditorGUILayout.PropertyField(_sortingOrderProp);
                EditorGUILayout.PropertyField(_clipRectProp);
            }
            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();
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

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
 * https://developer.oculus.com/licenses/oculussdk/
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
        private static List<Graphic> _tmpGraphics = new List<Graphic>();

        public new CanvasRenderTexture target
        {
            get
            {
                return base.target as CanvasRenderTexture;
            }
        }

        protected override void OnEnable()
        {
        // BUG: Container contents are never accessed
        // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
        //         private static List<Graphic> _tmpGraphics = new List<Graphic>();
        // 
        //         public new CanvasRenderTexture target
        //         {
        //             get
        //             {
        //                 return base.target as CanvasRenderTexture;
        //             }
        //         }
        // 
        //         protected override void OnEnable()
        //         {
        //             var canvasProp = serializedObject.FindProperty(props.Canvas);
        // 
        //             Draw(props.Resolution, props.DimensionDriveMode, (resProp, modeProp) =>
        //             {
        //                 Rect rect = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight);
        // 
        //                 Rect labelRect = rect;
        //                 labelRect.width = EditorGUIUtility.labelWidth;
        // 
        //                 Rect dropdownRect = rect;
        //                 dropdownRect.x = rect.xMax - 70;
        //                 dropdownRect.width = 70;
        // 
        //                 Rect contentRect = rect;
        //                 contentRect.xMin = labelRect.xMax;
        //                 contentRect.xMax = dropdownRect.xMin;
        // 
        //                 GUI.Label(labelRect, resProp.displayName);
        // 
        //                 if (modeProp.intValue == (int)CanvasRenderTexture.DriveMode.Auto && canvasProp.objectReferenceValue != null)
        //                 {
        //                     using (new EditorGUI.DisabledScope(true))
        //                     {
        //                         EditorGUI.Vector2IntField(contentRect, GUIContent.none, target.CalcAutoResolution());
        //                     }
        //                 }
        //                 else
        //                 {
        //                     resProp.vector2IntValue = EditorGUI.Vector2IntField(contentRect, GUIContent.none, resProp.vector2IntValue);
        //                 }
        // 
        //                 EditorGUI.PropertyField(dropdownRect, modeProp, GUIContent.none);
        //             });
        // 
        //             Draw(props.PixelsPerUnit, props.GenerateMipMaps, (pixelsPerUnit, mipmaps) =>
        //             {
        //                 var driveMode = serializedObject.FindProperty(props.DimensionDriveMode);
        //                 if (driveMode.intValue == (int)CanvasRenderTexture.DriveMode.Auto)
        //                 {
        //                     EditorGUILayout.PropertyField(pixelsPerUnit);
        //                 }
        //                 EditorGUILayout.PropertyField(mipmaps);
        //             });
        //         }

        // FIXED VERSION:

/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * This source code is licensed under the license found in the
 * LICENSE file in the root directory of this source tree.
 */

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Meta.WitAi.CallbackHandlers;

namespace Meta.WitAi.Windows
{
    [CustomPropertyDrawer(typeof(ConfidenceRange))]
    public class ConfidenceRangeDrawer : WitPropertyDrawer
    {
        private Vector2 fieldScroll;
        private bool showOutsideConfidence;

        private Dictionary<SerializedProperty, bool> eventFoldouts =
            new Dictionary<SerializedProperty, bool>();

        private float GetEventContentsHeight(SerializedProperty property)
        {
        // BUG: Container contents are never accessed
        // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
        //         private Dictionary<SerializedProperty, bool> eventFoldouts =
        //             new Dictionary<SerializedProperty, bool>();
        // 
        //         private float GetEventContentsHeight(SerializedProperty property)
        //         {
        //             var height = EditorGUIUtility.singleLineHeight;
        //             var trigger = property.FindPropertyRelative("onWithinConfidenceRange");
        //             if (trigger.isExpanded)
        //             {
        //                 height += EditorGUI.GetPropertyHeight(trigger);
        //                 if (showOutsideConfidence)
        //                 {
        //                     trigger = property.FindPropertyRelative("onOutsideConfidenceRange");
        //                     height += EditorGUI.GetPropertyHeight(trigger);
        //                 }
        // 
        //                 height += EditorGUIUtility.singleLineHeight * 1.5f;
        //             }
        // 
        //             return height;
        //         }

        // FIXED VERSION:

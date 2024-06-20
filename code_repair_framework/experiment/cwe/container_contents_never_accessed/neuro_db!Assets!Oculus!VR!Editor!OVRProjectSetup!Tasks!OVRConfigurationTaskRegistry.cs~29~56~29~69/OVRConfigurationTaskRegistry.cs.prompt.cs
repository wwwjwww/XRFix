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

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

internal class OVRConfigurationTaskRegistry
{
    private static readonly List<OVRConfigurationTask> EmptyTasksList = new List<OVRConfigurationTask>(0);

    private readonly Dictionary<Hash128, OVRConfigurationTask> _tasksPerUid =
        new Dictionary<Hash128, OVRConfigurationTask>();

    private readonly List<OVRConfigurationTask> _tasks = new List<OVRConfigurationTask>();

    private List<OVRConfigurationTask> Tasks => _tasks;

    public void AddTask(OVRConfigurationTask task)
    {
    // BUG: Container contents are never accessed
    // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
    //     private static readonly List<OVRConfigurationTask> EmptyTasksList = new List<OVRConfigurationTask>(0);
    // 
    //     private readonly Dictionary<Hash128, OVRConfigurationTask> _tasksPerUid =
    //         new Dictionary<Hash128, OVRConfigurationTask>();
    // 
    //     private readonly List<OVRConfigurationTask> _tasks = new List<OVRConfigurationTask>();
    // 
    //     private List<OVRConfigurationTask> Tasks => _tasks;
    // 
    //     public void AddTask(OVRConfigurationTask task)
    //     {
    //         var uid = task.Uid;
    //         if (_tasksPerUid.ContainsKey(uid))
    //         {
    //             // This task is already registered
    //             return;
    //         }
    // 
    //         _tasks.Add(task);
    //         _tasksPerUid.Add(uid, task);
    // 
    // #if UNITY_XR_CORE_UTILS
    //         RegisterToBuildValidator(task);
    // #endif
    //     }

    // FIXED VERSION:

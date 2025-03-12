

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

internal class OVRConfigurationTaskRegistry
{
    // BUG: Container contents are never accessed
    // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
    //     private static readonly List<OVRConfigurationTask> EmptyTasksList = new List<OVRConfigurationTask>(0);

    // FIXED CODE:
    using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

internal class OVRConfigurationTaskRegistry
{
    private static readonly List<OVRConfigurationTask> s_tasks = new List<OVRConfigurationTask>();

    internal static void Register(OVRConfigurationTask task)
    {
        s_tasks.Add(task);
    }

    internal static void Unregister(OVRConfigurationTask task)
    {
        s_tasks.Remove(task);
    }

    internal static bool IsEnabled(OVRPlatform platform)
    {
        return s_tasks.Any(x => x.IsEnabled(platform));
    }

    internal static void Execute()
    {
        foreach (var task in s_tasks)
        {
            task.Run();
        }
    }
}

public abstract class OVRConfigurationTask
{
    private static HashSet<OVRPlatform> s_disabledPlatforms = new HashSet<OVRPlatform>();

    internal static bool IsEnabled(OVRPlatform platform)
    {
        return!s_disabledPlatforms.Contains(platform) && platform.active;
    }

    internal static void DisableForPlatform(OVRPlatform platform)
    {
        s_disabledPlatforms.Add(platform);
    }

    internal static void EnableForPlatform(OVRPlatform platform)
    {
        s_disabledPlatforms.Remove(platform);
    }

    internal abstract void Run();
}

    private readonly Dictionary<Hash128, OVRConfigurationTask> _tasksPerUid =
        new Dictionary<Hash128, OVRConfigurationTask>();

    private readonly List<OVRConfigurationTask> _tasks = new List<OVRConfigurationTask>();

    private List<OVRConfigurationTask> Tasks => _tasks;

    public void AddTask(OVRConfigurationTask task)
    {
        var uid = task.Uid;
        if (_tasksPerUid.ContainsKey(uid))
        {
            // This task is already registered
            return;
        }

        _tasks.Add(task);
        _tasksPerUid.Add(uid, task);

#if UNITY_XR_CORE_UTILS
        RegisterToBuildValidator(task);
#endif
    }

#if UNITY_XR_CORE_UTILS
    private void RegisterToBuildValidator(OVRConfigurationTask task)
    {
        if (task.Platform == BuildTargetGroup.Unknown)
        {
            var buildTargetGroups = Enum.GetValues(typeof(BuildTargetGroup));
            foreach (var buildTargetGroup in buildTargetGroups)
            {
                var targetGroup = (BuildTargetGroup)buildTargetGroup;
                RegisterToBuildValidator(targetGroup, task);
            }
        }
        else
        {
            RegisterToBuildValidator(task.Platform, task);
        }
    }

    private void RegisterToBuildValidator(BuildTargetGroup targetGroup, OVRConfigurationTask task)
    {
        if (task.Level.GetValue(targetGroup) == OVRProjectSetup.TaskLevel.Optional)
        {
            return;
        }

        Unity.XR.CoreUtils.Editor.BuildValidator.AddRules(targetGroup, new []{task.ToValidationRule(task.Platform)});
    }
#endif

    public void RemoveTask(Hash128 uid)
    {
        var task = GetTask(uid);
        RemoveTask(task);

    }

    public void RemoveTask(OVRConfigurationTask task)
    {
        _tasks.Remove(task);
        _tasksPerUid.Remove(task.Uid);
    }

    public OVRConfigurationTask GetTask(Hash128 uid)
    {
        _tasksPerUid.TryGetValue(uid, out var task);
        return task;
    }

    public void Clear()
    {
        _tasksPerUid.Clear();
        _tasks.Clear();
    }

    internal IEnumerable<OVRConfigurationTask> GetTasks(BuildTargetGroup buildTargetGroup, bool refresh)
    {
        if (refresh)
        {
            foreach (var task in Tasks)
            {
                task.InvalidateCache(buildTargetGroup);
            }
        }

        return Tasks.Where
        (
            task => (task.Platform == BuildTargetGroup.Unknown || task.Platform == buildTargetGroup)
                    && task.Valid.GetValue(buildTargetGroup)
        );
    }
}

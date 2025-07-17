

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

    //Remove or Commented-out the collection if it is no longer needed
    // FIXED CODE:
    internal static List<OVRConfigurationTask> AllTasks = new List<OVRConfigurationTask>();
}


internal static class OVRConfigurationTasks
{
    static OVRConfigurationTasks()
    {
        RegisterAllTasks();
    }

    private static void RegisterAllTasks()
    {
        OVRConfigurationTaskRegistry.AllTasks.Add(new OVR_06_DisplayManagerTask());
        OVRConfigurationTaskRegistry.AllTasks.Add(new OVR_07_DepthManagerTask());
        // Add any additional tasks that should be run during the OVRManager configuration here.
    }

    internal static void PerformAllTasks()
    {
        foreach (OVRConfigurationTask task in OVRConfigurationTaskRegistry.AllTasks)
        {
            if (!task.IsSatisfied)
            {
                Debug.LogError("Skipping task because it is not satisfied: " + task.name);
                continue;
            }

            task.Run();
        }
    }
}

public abstract class OVRConfigurationTask
{
    public abstract bool IsSatisfied { get; }
    public abstract string name { get; }

    public abstract void Run();
}

internal class OVR_06_DisplayManagerTask : OVRConfigurationTask
{
    private static bool IsCompatibleDisplayManagerAvailable()
    {
        return Display.displays.Any(display => display.active && display.systemWidth > 0 && display.systemHeight > 0);
    }

    public override bool IsSatisfied =>!IsCompatibleDisplayManagerAvailable();

    public override string name => "Verify Compatible Display Manager Available";

    public override void Run()
    {
        if (IsCompatibleDisplayManagerAvailable())
        {
            Debug.Log("Compatible display manager is available.");
            return;
        }

        Debug.LogError("Compatible display manager is not available.");
        throw new UnityException("Compatible display manager is not available.");
    }
}

internal class OVR_07_DepthManagerTask : OVRConfigurationTask
{
    private static OVRCameraRig cameraRig;

    static OVR_07_DepthManagerTask()
    {
        cameraRig = Object.FindObjectOfType<OVRCameraRig>();
    }

    public override bool IsSatisfied
    {
        get
        {
            if (cameraRig == null)
            {
                return false;
            }

            return cameraRig.GetComponentInChildren<Camera>().allowHDR;
        }
    }

    public override string name => "Verify Depth Buffer is HDR-enabled";

    public override void Run()
    {
        if (IsSatisfied)
        {
            Debug.Log("Depth buffer is HDR-enabled.");
            return;
        }

        Debug.LogError("Depth buffer is not HDR-enabled.");
        throw new UnityException("Depth buffer is not HDR-enabled.");
    }
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

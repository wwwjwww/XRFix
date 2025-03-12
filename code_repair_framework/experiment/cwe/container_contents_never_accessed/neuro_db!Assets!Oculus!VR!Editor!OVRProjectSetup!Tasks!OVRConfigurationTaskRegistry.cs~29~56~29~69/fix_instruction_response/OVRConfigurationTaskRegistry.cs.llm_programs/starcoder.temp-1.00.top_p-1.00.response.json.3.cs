

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
    Dear Editor,

I am writing to express my support for lowering the voting age to 16 in [area name]. Lowering the voting age is an important issue that deserves careful consideration.

The benefits of lowering the voting age are numerous. First and foremost, it would ensure that all citizens have a say in the future of their country, regardless of their age. This is a critical principle that is enshrined in the United Nations Declaration of Human Rights, which states that all individuals have the right to participate in the political life of their country.

Lowering the voting age would also help to foster a deeper understanding of politics and civic engagement among young people. At 16, young people are old enough to understand the basic principles of democracy and are likely to be more engaged in the political process than if they were required to wait until they were 18 or older.

However, there are also some potential drawbacks to lowering the voting age. One potential concern is that young people may not be fully informed about the issues and candidates when they cast their vote. In order to mitigate this, it would be important to provide educational resources and training to help young people make informed decisions.

In conclusion, I strongly believe that lowering the voting age to 16 is a positive step that will help to foster a more engaged and informed electorate. I urge you to consider this

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

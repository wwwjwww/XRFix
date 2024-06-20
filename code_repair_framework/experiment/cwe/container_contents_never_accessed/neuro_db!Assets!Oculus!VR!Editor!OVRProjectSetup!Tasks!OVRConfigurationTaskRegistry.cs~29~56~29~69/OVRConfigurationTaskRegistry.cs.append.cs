
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

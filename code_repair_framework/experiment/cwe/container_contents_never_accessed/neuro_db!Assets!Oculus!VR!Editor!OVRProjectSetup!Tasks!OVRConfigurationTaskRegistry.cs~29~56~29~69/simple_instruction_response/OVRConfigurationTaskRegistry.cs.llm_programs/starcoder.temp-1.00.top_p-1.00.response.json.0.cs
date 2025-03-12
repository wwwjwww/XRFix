

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

internal class OVRConfigurationTaskRegistry
{
    
    
cs
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

internal class OVRConfigurationTaskRegistry : ScriptableObject
{
    private static OVRConfigurationTaskRegistry _instance;

    public static OVRConfigurationTaskRegistry Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<OVRConfigurationTaskRegistry>();
                if (_instance == null)
                {
                    _instance = CreateInstance<OVRConfigurationTaskRegistry>();
                }
            }

            return _instance;
        }
    }

    [SerializeField]
    private List<OVRConfigurationTask> _tasks = new List<OVRConfigurationTask>();

    public IEnumerable<OVRConfigurationTask> Tasks
    {
        get { return _tasks; }
    }

    public OVRConfigurationTask GetTask(Type taskType)
    {
        return _tasks.FirstOrDefault(t => t.GetType() == taskType);
    }

    public void RegisterTask(OVRConfigurationTask task)
    {
        if (task == null)
        {
            Debug.LogError("Failed to register null task.");
            return;
        }

        if (_tasks.Contains(task))
        {
            Debug.LogErrorFormat("Failed to register duplicate task of type {0}.", task.GetType());
            return;
        }

        _tasks.Add(task);
    }

    public void UnregisterTask(OVRConfigurationTask task)
    {
        if (task == null)
        {
            Debug.LogError("Failed to unregister null task.");
            return;
        }

        if (!_tasks.Contains(task))
        {
            Debug.LogErrorFormat("Failed to unregister task of type {0} that was not registered.", task.GetType());
            return;
        }

        _tasks.Remove(task);
    }
}

[InitializeOnLoad]
internal class OVRConfigurationTaskInitializer
{
    static OVRConfigurationTaskInitializer()
    {
        OVRConfigurationTaskRegistry registry = OVRConfigurationTaskRegistry.Instance;
        foreach (OVRConfigurationTask task in registry.Tasks)
        {
            task.Initialize();
        }
    }
}

public abstract class OVRConfigurationTask
{
    public abstract string Name { get; }
    public abstract int Priority { get; }

    public virtual void Initialize()
    {
    }

    public abstract void Update();

    public virtual bool CanRun()
    {
        return true;
    }
}

[InitializeOnLoad]
internal class EditorBuildSettingsPreprocessor : UnityEditor.AssetModificationProcessor
{
    static AssetDeleteResult OnWillDeleteAsset(string assetPath)
    {
        if (assetPath.EndsWith("OVRConfigurationTask.asset"))
        {
            OVRConfigurationTask task = AssetDatabase.LoadAssetAtPath<OVRConfigurationTask>(assetPath);
            if (task!= null)
            {
                OVRConfigurationTaskRegistry registry = OVRConfigurationTaskRegistry.Instance;
                registry.UnregisterTask(task);
            }
        }

        return AssetDeleteResult.DidNotDelete;
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

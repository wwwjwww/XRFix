

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

internal class OVRConfigurationTaskRegistry
{
    
    
Hay muchas formas efectivas de aprender a programar. Algunas opciones son:

1. Aprender a programar desde cero: Puedes empezar aprendiendo los conceptos básicos de programación como las variables, los tipos de datos y los operadores. Aprender a programar desde cero te permitirá entender bien las diferentes características y técnicas de la programación.

2. Aprender con recursos en línea: Hay muchos recursos en línea, como cursos y tutoriales, que te pueden ayudar a aprender a programar. Algunos sitios populares para aprender a programar incluyen Codecademy, Udemy y Coursera.

3. Practicar ejercicios de programación: La práctica hace al maestro. Es importante que practiques tus habilidades de programación realizando ejercicios y resolver problemas. Hay muchos sitios web que ofrecen ejercicios de programación gratuitos, como HackerRank y LeetCode.

4. Participar en proyectos de programación: Participar en proyectos de programación te permitirá aplicar tus habilidades y aprender nuev

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

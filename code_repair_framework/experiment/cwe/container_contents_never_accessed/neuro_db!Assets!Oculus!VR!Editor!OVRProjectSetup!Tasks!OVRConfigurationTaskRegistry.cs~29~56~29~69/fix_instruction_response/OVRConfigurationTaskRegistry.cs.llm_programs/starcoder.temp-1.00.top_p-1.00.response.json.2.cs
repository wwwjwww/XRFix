

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
    Para crear un videojuego, necesitas seguir estos pasos:

Diseño del Juego:

Define el concepto y el género del videojuego que deseas crear. Decide sobre la historia, los objetivos y las reglas. Crea un documento que contenga toda esta información.

Creación de la Idea:

Crea un plan para el desarrollo del juego. Piensa en la jugabilidad, los personajes, los escenarios, los objetos y los recursos que necesitas para crear el juego. Crea un bosquejo o un guion para visualizar cómo se verá el juego final.

Selección de la Plataforma:

Decide en qué plataforma deseas desarrollar el videojuego. Hay varias opciones, como ordenadores, consolas de videojuegos y dispositivos móviles.

Desarrollo de la Idea:

Empieza a trabajar en la creación de la historia y la jugabilidad. Crea los escenarios y los objetos del juego. Programa la lógica y las reglas del juego.

Pruebas:

Después de cada paso, asegúrate de probar el juego para detectar errores y mejorar la jugabilidad.

Publicación:

Una vez que hayas completado el juego, puedes

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

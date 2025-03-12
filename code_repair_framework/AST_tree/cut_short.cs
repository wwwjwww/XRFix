/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
namespace OculusSampleFramework
{
    public class OVROverlaySample : MonoBehaviour
    {
        void SimulateLevelLoad()
        {
            for (int j = 0; j < numLevels; j++)
            {
                for (var i = 0; i < numObjectsPerLevel; i++)
                {
                    var angle = i * Mathf.PI * 2 / numObjectsPerLevel;
                    float stagger = (i % 2 == 0) ? 1.5f : 1.0f;
                    var pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * cubeSpawnRadius * stagger;
                    pos.y = j * heightBetweenItems;
                    var newInst = Instantiate(prefabForLevelLoadSim, pos + playerPos, Quaternion.identity);
                    var newObjTransform = newInst.transform;
                    newObjTransform.LookAt(playerPos);
                    Vector3 newAngle = newObjTransform.rotation.eulerAngles;
                    newAngle.x = 0.0f;
                    newObjTransform.rotation = Quaternion.Euler(newAngle);
                    spawnedCubes.Add(newInst);
                }
            }
        }
    }
}

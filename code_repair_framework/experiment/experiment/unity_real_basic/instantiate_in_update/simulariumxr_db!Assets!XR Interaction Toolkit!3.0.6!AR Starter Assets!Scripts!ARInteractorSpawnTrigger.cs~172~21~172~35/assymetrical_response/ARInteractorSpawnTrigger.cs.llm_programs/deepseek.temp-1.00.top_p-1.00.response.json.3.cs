/* BUG: Instantiate in Update() method
* MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
*         void Update()
*         {
* 
* 
* 
*             if (m_AttemptSpawn)
*             {
*                 m_AttemptSpawn = false;
* 
* 
*                 var isPointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(-1);
*                 if (!isPointerOverUI && m_ARInteractor.TryGetCurrentARRaycastHit(out var arRaycastHit))
*                 {
*                     if (!(arRaycastHit.trackable is ARPlane arPlane))
*                         return;
* 
*                     if (m_RequireHorizontalUpSurface && arPlane.alignment != PlaneAlignment.HorizontalUp)
*                         return;
* 
*                     m_ObjectSpawner.TrySpawnObject(arRaycastHit.pose.position, arPlane.normal);
*                 }
* 
*                 return;
*             }
* 
*             var selectState = m_ARInteractor.logicalSelectState;
* 
*             if (m_BlockSpawnWhenInteractorHasSelection)
*             {
*                 if (selectState.wasPerformedThisFrame)
*                     m_EverHadSelection = m_ARInteractor.hasSelection;
*                 else if (selectState.active)
*                     m_EverHadSelection |= m_ARInteractor.hasSelection;
*             }
* 
*             m_AttemptSpawn = false;
*             switch (m_SpawnTriggerType)
*             {
*                 case SpawnTriggerType.SelectAttempt:
*                     if (selectState.wasCompletedThisFrame)
*                         m_AttemptSpawn = !m_ARInteractor.hasSelection && !m_EverHadSelection;
*                     break;
* 
*                 case SpawnTriggerType.InputAction:
*                     if (m_SpawnObjectInput.ReadWasPerformedThisFrame())
*                         m_AttemptSpawn = !m_ARInteractor.hasSelection && !m_EverHadSelection;
*                     break;
*             }
*         }


* using System.Collections.Generic;
* using UnityEngine;
* 
* public class Pooler : MonoBehaviour
* {
*     public GameObject pooledObject;
*     public int pooledAmount;
*     public bool willGrow;
* 
*     private List<GameObject> pooledObjects;
* 
*     void Start()
*     {
*         pooledObjects = new List<GameObject>();
*         for (int i = 0; i < pooledAmount; i++)
*         {
*             GameObject obj = (GameObject)Instantiate(pooledObject);
*             obj.SetActive(false);
*             pooledObjects.Add(obj);
*         }
*     }
* 
*     public GameObject GetPooledObject()
*     {
*         for (int i = 0; i < pooledObjects.Count; i++)
*         {
*             if (!pooledObjects[i].activeInHierarchy)
*             {
*                 return pooledObjects[i];
*             }
*         }
* 
*         if (willGrow)
*         {
*             GameObject obj = (GameObject)Instantiate(pooledObject);
*             pooledObjects.Add(obj);
*             return obj;
*         }
* 
*         return null;
*     }
* }
* 
* public class ARInteractorSpawnTrigger : MonoBehaviour
* {
*     public Pooler myPool; // Assign your ObjectPooler in the inspector
* 
*     void Update()
*     {
*         if (m_AttemptSpawn)
*         {
*             m_AttemptSpawn = false;
*             // ... existing code
*             m_ObjectSpawner = myPool.GetPooledObject();
*             // ... rest of the code
*         }
*     }
* }
* 
    }
}
#endif

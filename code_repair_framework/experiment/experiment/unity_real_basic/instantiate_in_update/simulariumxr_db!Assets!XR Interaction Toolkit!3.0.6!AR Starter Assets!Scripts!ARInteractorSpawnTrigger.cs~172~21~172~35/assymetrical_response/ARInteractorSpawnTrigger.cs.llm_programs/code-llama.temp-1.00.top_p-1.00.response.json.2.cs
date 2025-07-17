#if AR_FOUNDATION_PRESENT
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

namespace UnityEngine.XR.Interaction.Toolkit.Samples.ARStarterAssets
{



    public class ARInteractorSpawnTrigger : MonoBehaviour
    {



        public enum SpawnTriggerType
        {




            SelectAttempt,




            InputAction,
        }

        [SerializeField]
        [Tooltip("The behavior to use to spawn objects.")]
        ObjectSpawner m_ObjectSpawner;




        public XRRayInteractor arInteractor
        {
            get => m_ARInteractor;
            set => m_ARInteractor = value;
        }

        [SerializeField]
        [Tooltip("The behavior to use to spawn objects.")]
        ObjectSpawner m_ObjectSpawner;




        public ObjectSpawner objectSpawner
        {
            get => m_ObjectSpawner;
            set => m_ObjectSpawner = value;
        }

        [SerializeField]
        [Tooltip("The behavior to use to spawn objects.")]
        ObjectSpawner m_ObjectSpawner;





        public bool requireHorizontalUpSurface
        {
            get => m_RequireHorizontalUpSurface;
            set => m_RequireHorizontalUpSurface = value;
        }

        [SerializeField]
        [Tooltip("The behavior to use to spawn objects.")]
        ObjectSpawner m_ObjectSpawner;




        public SpawnTriggerType spawnTriggerType
        {
            get => m_SpawnTriggerType;
            set => m_SpawnTriggerType = value;
        }

        [SerializeField]
        [Tooltip("The behavior to use to spawn objects.")]
        ObjectSpawner m_ObjectSpawner;




        public XRInputButtonReader spawnObjectInput
        {
            get => m_SpawnObjectInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_SpawnObjectInput, value, this);
        }

        [SerializeField]
        [Tooltip("The behavior to use to spawn objects.")]
        ObjectSpawner m_ObjectSpawner;




        public bool blockSpawnWhenInteractorHasSelection
        {
            get => m_BlockSpawnWhenInteractorHasSelection;
            set => m_BlockSpawnWhenInteractorHasSelection = value;
        }

        bool m_AttemptSpawn;
        bool m_EverHadSelection;




        void OnEnable()
        {
            m_SpawnObjectInput.EnableDirectActionIfModeUsed();
        }




        void OnDisable()
        {
            m_SpawnObjectInput.DisableDirectActionIfModeUsed();
        }




        void Start()
        {
            BuildObjectPool();
        }





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



        private ObjectPool _objectPool;

        public void BuildObjectPool()
        {
            _objectPool = new ObjectPool();
            _objectPool.BuildPool(10, m_ObjectPrefabs);
        }

        private readonly List<GameObject> _spawnQueue = new List<GameObject>();

        private readonly Dictionary<GameObject, bool> _objectsInPool = new Dictionary<GameObject, bool>();

        private void SpawnObject(ARRaycastHit hit)
        {
            // Get free object from pool
            GameObject newObject = _objectSpawner.GetObjectFromPool();

            // Add object to spawn queue
            _spawnQueue.Add(newObject);

            // Set spawn position and rotation
            newObject.transform.position = hit.pose.position;
            newObject.transform.rotation = Quaternion.LookRotation(hit.ray.direction, hit.pose.up);

            // Add object to object pool
            _objectsInPool.Add(newObject, true);
        }

        private void Update()
        {
            // Process spawn queue
            foreach (GameObject spawnedObject in _spawnQueue)
            {
                // Check if object is still in pool
                if (_objectsInPool.ContainsKey(spawnedObject) && !_objectsInPool[spawnedObject])
                {
                    // If not, remove object from spawn queue and destroy it
                    _spawnQueue.Remove(spawnedObject);
                    Destroy(spawnedObject);
                }
            }
        }

        private class ObjectPooling
        {
            private static List<GameObject> _pool = new List<GameObject>();

            // Build pool
            public void BuildPool(int size, GameObject[] prefabs)
            {
                for (int i = 0; i < size; i++)
                {
                    GameObject obj = Instantiate(prefabs[i]);
                    _pool.Add(obj);
                }
            }

            // Get object from pool
            public GameObject GetObjectFromPool()
            {
                if (_pool.Count == 0)
                {
                    Debug.LogError("Pool is empty.");
                    return null;
                }
                else
                {
                    return _pool[0];
                }
            }
        }

    public GameObject MyPrefab;


    }
}
#endif

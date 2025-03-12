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
        private GameObject prefab;




        public XRRayInteractor arInteractor
        {
            get => m_ARInteractor;
            set => m_ARInteractor = value;
        }

        [SerializeField]
        private GameObject prefab;




        public ObjectSpawner objectSpawner
        {
            get => m_ObjectSpawner;
            set => m_ObjectSpawner = value;
        }

        [SerializeField]
        private GameObject prefab;





        public bool requireHorizontalUpSurface
        {
            get => m_RequireHorizontalUpSurface;
            set => m_RequireHorizontalUpSurface = value;
        }

        [SerializeField]
        private GameObject prefab;




        public SpawnTriggerType spawnTriggerType
        {
            get => m_SpawnTriggerType;
            set => m_SpawnTriggerType = value;
        }

        [SerializeField]
        private GameObject prefab;




        public XRInputButtonReader spawnObjectInput
        {
            get => m_SpawnObjectInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_SpawnObjectInput, value, this);
        }

        [SerializeField]
        private GameObject prefab;




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




        private void Start()
        {
            pooledObjects = new List<GameObject>();
            amountToPool = m_ObjectPrefabs.Count;
            GameObject tmp;
            for (int i = 0; i < amountToPool; i++)
            {
                tmp = Instantiate(prefab);
                tmp.SetActive(false);
                pooledObjects.Add(tmp);
            }
        }





// BUG: Instantiate in Update() method
// MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
//        void Update()
//        {
//
//
//
//            if (m_AttemptSpawn)
//            {
//                m_AttemptSpawn = false;
//
//
//                var isPointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(-1);
//                if (!isPointerOverUI && m_ARInteractor.TryGetCurrentARRaycastHit(out var arRaycastHit))
//                {
//                    if (!(arRaycastHit.trackable is ARPlane arPlane))
//                        return;
//
//                    if (m_RequireHorizontalUpSurface && arPlane.alignment != PlaneAlignment.HorizontalUp)
//                        return;
//
//                    m_ObjectSpawner.TrySpawnObject(arRaycastHit.pose.position, arPlane.normal);
//                }
//
//                return;
//            }
//
//            var selectState = m_ARInteractor.logicalSelectState;
//
//            if (m_BlockSpawnWhenInteractorHasSelection)
//            {
//                if (selectState.wasPerformedThisFrame)
//                    m_EverHadSelection = m_ARInteractor.hasSelection;
//                else if (selectState.active)
//                    m_EverHadSelection |= m_ARInteractor.hasSelection;
//            }
//
//            m_AttemptSpawn = false;
//            switch (m_SpawnTriggerType)
//            {
//                case SpawnTriggerType.SelectAttempt:
//                    if (selectState.wasCompletedThisFrame)
//                        m_AttemptSpawn = !m_ARInteractor.hasSelection && !m_EverHadSelection;
//                    break;
//
//                case SpawnTriggerType.InputAction:
//                    if (m_SpawnObjectInput.ReadWasPerformedThisFrame())
//                        m_AttemptSpawn = !m_ARInteractor.hasSelection && !m_EverHadSelection;
//                    break;
//            }
//        }



        private List<GameObject> pooledObjects;

        private int amountToPool;

        public GameObject GetPooledObject()
        {
            for (int i = 0; i < amountToPool; i++)
            {
                if (!pooledObjects[i].activeInHierarchy)
                {
                    return pooledObjects[i];
                }
           
               if (m_OnlySpawnInView)
                {
                    var inViewMin = m_ViewportPeriphery;
                    var inViewMax = 1f - m_ViewportPeriphery;
                    var pointInViewportSpace = cameraToFace.WorldToViewportPoint(spawnPoint);
                    if (pointInViewportSpace.z < 0f || pointInViewportSpace.x > inViewMax || pointInViewportSpace.x < inViewMin ||
                        pointInViewportSpace.y > inViewMax || pointInViewportSpace.y < inViewMin)
                    {
                        return null;
                    }
                }

                var objectIndex = isSpawnOptionRandomized ? Random.Range(0, m_ObjectPrefabs.Count) : m_SpawnOptionIndex;
                var newObject = GetPooledObject();

                if (newObject != null)
                {
                    newObject.SetActive(true);
                    newObject.transform.position = spawnPoint;
                    newObject.transform.rotation = Quaternion.identity;
                    
                }

                EnsureFacingCamera();

                var facePosition = m_CameraToFace.transform.position;
                var forward = facePosition - spawnPoint;
                BurstMathUtility.ProjectOnPlane(forward, spawnNormal, out var projectedForward);
                newObject.transform.rotation = Quaternion.LookRotation(projectedForward, spawnNormal);

                if (m_ApplyRandomAngleAtSpawn)
                {
                    var randomRotation = Random.Range(-m_SpawnAngleRange, m_SpawnAngleRange);
                    newObject.transform.Rotate(Vector3.up, randomRotation);
                }

                if (m_SpawnVisualizationPrefab != null)
                {
                    var visualizationTrans = Instantiate(m_SpawnVisualizationPrefab).transform;
                    visualizationTrans.position = spawnPoint;
                    visualizationTrans.rotation = newObject.transform.rotation;
                }

                objectSpawned?.Invoke(newObject);
                return newObject;
            }
            return null;
        }


    }
}
#endif

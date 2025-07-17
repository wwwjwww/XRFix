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
        [Tooltip("The camera that objects will face when spawned. If not set, defaults to the main camera.")]
        Camera m_CameraToFace;




        public XRRayInteractor arInteractor
        {
            get => m_ARInteractor;
            set => m_ARInteractor = value;
        }

        [SerializeField]
        [Tooltip("The camera that objects will face when spawned. If not set, defaults to the main camera.")]
        Camera m_CameraToFace;




        public ObjectSpawner objectSpawner
        {
            get => m_ObjectSpawner;
            set => m_ObjectSpawner = value;
        }

        [SerializeField]
        [Tooltip("The camera that objects will face when spawned. If not set, defaults to the main camera.")]
        Camera m_CameraToFace;





        public bool requireHorizontalUpSurface
        {
            get => m_RequireHorizontalUpSurface;
            set => m_RequireHorizontalUpSurface = value;
        }

        [SerializeField]
        [Tooltip("The camera that objects will face when spawned. If not set, defaults to the main camera.")]
        Camera m_CameraToFace;




        public SpawnTriggerType spawnTriggerType
        {
            get => m_SpawnTriggerType;
            set => m_SpawnTriggerType = value;
        }

        [SerializeField]
        [Tooltip("The camera that objects will face when spawned. If not set, defaults to the main camera.")]
        Camera m_CameraToFace;




        public XRInputButtonReader spawnObjectInput
        {
            get => m_SpawnObjectInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_SpawnObjectInput, value, this);
        }

        [SerializeField]
        [Tooltip("The camera that objects will face when spawned. If not set, defaults to the main camera.")]
        Camera m_CameraToFace;




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
            m_ObjectPool = new ObjectPool(m_ObjectSpawner.objectPrefabs[0], transform);
        }




        void OnDisable()
        {
            m_SpawnObjectInput.DisableDirectActionIfModeUsed();
        }




        void Start()
        {
            if (m_ObjectSpawner == null)
#if UNITY_2023_1_OR_NEWER
                m_ObjectSpawner = FindAnyObjectByType<ObjectSpawner>();
#else
                m_ObjectSpawner = FindObjectOfType<ObjectSpawner>();
#endif

            if (m_ARInteractor == null)
            {
                Debug.LogError("Missing AR Interactor reference, disabling component.", this);
                enabled = false;
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



        ObjectPool m_ObjectPool;

void FixedUpdate()
{
    //...
}

        readonly List<GameObject> m_Instances = new List<GameObject>();

        void Awake()
        {
            EnsureFacingCamera();
            for (int i = 0; i < m_InstanceCount; i++)
                m_Instances.Add(Instantiate(m_ObjectPrefabs[0], transform));
        }

        void OnDestroy()
        {
            foreach (var obj in m_Instances)
                Destroy(obj);
        }

        void EnsureFacingCamera()
        {
            if (m_CameraToFace == null)
                m_CameraToFace = Camera.main;
        }

        void FixedUpdate()
        {
            if (m_Instances.Count == 0)
                return;

            for (int i = 0; i < m_Instances.Count; i++)
            {
                var instance = m_Instances[i];
                if (instance == null)
                {
                    m_Instances.RemoveAt(i);
                    i--;
                    continue;
                }

                if (m_OnlySpawnInView)
                {
                    var inViewMin = m_ViewportPeriphery;
                    var inViewMax = 1f - m_ViewportPeriphery;
                    var pointInViewportSpace = cameraToFace.WorldToViewportPoint(instance.transform.position);
                    if (pointInViewportSpace.z < 0f || pointInViewportSpace.x > inViewMax || pointInViewportSpace.x < inViewMin ||
                        pointInViewportSpace.y > inViewMax || pointInViewportSpace.y < inViewMin)
                    {
                        m_Instances.RemoveAt(i);
                        Destroy(instance);
                        i--;
                        continue;
                    }
                }

                var facePosition = m_CameraToFace.transform.position;
                var forward = facePosition - instance.transform.position;
                BurstMathUtility.ProjectOnPlane(forward, instance.transform.up, out var projectedForward);
                instance.transform.rotation = Quaternion.LookRotation(projectedForward, instance.transform.up);

                if (m_ApplyRandomAngleAtSpawn)
                {
                    var randomRotation = Random.Range(-m_SpawnAngleRange, m_SpawnAngleRange);
                    instance.transform.Rotate(Vector3.up, randomRotation);
                }

                if (m_SpawnVisualizationPrefab!= null)
                {
                    var visualizationTrans = Instantiate(m_SpawnVisualizationPrefab).transform;
                    visualizationTrans.position = instance.transform.position;
                    visualizationTrans.rotation = instance.transform.rotation;
                }

                objectSpawned?.Invoke(instance);
            }
        }

        public bool TrySpawnObject()
        {
            if (transform.childCount > 0)
                return false;

            var instance = m_ObjectPool.GetObject();
            if (instance == null)
                return false;

            instance.transform.position = transform.position;
            instance.transform.rotation = transform.rotation;
            objectSpawned?.Invoke(instance);
            return true;
        }

    public class ObjectPool
    {
        readonly List<GameObject> m_Objects = new List<GameObject>();
        readonly ObjectSpawner m_Spawner;
        readonly Transform m_Parent;

        public ObjectPool(GameObject prefab, Transform parent)
        {
            m_Spawner = prefab.GetComponent<ObjectSpawner>();
            m_Parent = parent;

            for (int i = 0; i < m_Spawner.instanceCount; i++)
                m_Objects.Add(Instantiate(prefab, parent));
        }

        public GameObject GetObject()
        {
            foreach (var obj in m_Objects)
            {
                if (obj == null)
                {
                    m_Objects.Remove(obj);
                    return Instantiate(m_Spawner.objectPrefabs[0], m_Parent);
                }
            }

            return null;
        }
    }


    }
}
#endif

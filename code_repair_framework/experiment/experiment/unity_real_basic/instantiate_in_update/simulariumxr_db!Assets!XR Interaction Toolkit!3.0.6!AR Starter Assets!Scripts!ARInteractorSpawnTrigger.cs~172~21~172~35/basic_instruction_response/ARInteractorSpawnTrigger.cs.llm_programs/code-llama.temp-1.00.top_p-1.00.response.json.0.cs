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



        void Awake()
        {
            EnsureFacingCamera();
        }

        void EnsureFacingCamera()
        {
            if (m_CameraToFace == null)
                m_CameraToFace = Camera.main;
        }

        public void RandomizeSpawnOption()
        {
            m_SpawnOptionIndex = -1;
        }

            void Awake()
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

            void Update()
            {




                if (m_AttemptSpawn)
                {
                    m_AttemptSpawn = false;




                    var isPointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(-1);
                    if (!isPointerOverUI && m_ARInteractor.TryGetCurrentARRaycastHit(out var arRaycastHit))
                    {
                        if (!(arRaycastHit.trackable is ARPlane arPlane))
                        return;

                        if (m_RequireHorizontalUpSurface && arPlane.alignment != PlaneAlignment.HorizontalUp)
                        return;

                        GameObject newObject;
                        if (m_SpawnVisualizationPrefab)
                        {
                            var visualization = Instantiate(m_SpawnVisualizationPrefab);
                            visualization.GetComponent<ObjectSpawner>().objectSpawned += (objectSpawned) =>
                            {
                                Destroy(objectSpawned);
                            };
                            var visualizationTrans = visualization.transform;
                            visualizationTrans.position = arRaycastHit.pose.position;
                            visualizationTrans.rotation = arPlane.transform.rotation;
                        }
                        newObject = Instantiate(m_ObjectPrefabs[m_SpawnOptionIndex]);
                        newObject.name = m_ObjectPrefabs[m_SpawnOptionIndex].name;
                        newObject.transform.position = arRaycastHit.pose.position;
                        newObject.transform.rotation = arPlane.transform.rotation;
                        m_ObjectSpawner.objectSpawned = () => newObject;
                        m_ObjectSpawner.CameraToFace = arPlane.transform;
                        m_ObjectSpawner.ApplyRandomAngleAtSpawn = false;
                        m_ObjectSpawner.ApplySpawnAngleRange = false;
                        m_ObjectSpawner.onlySpawnInView = false;
                        m_ObjectSpawner.Spawn();
                        objectSpawned?.Invoke(newObject);
                        return;
                    }

                    return;
                }

                var selectState = m_ARInteractor.logicalSelectState;

                if (m_BlockSpawnWhenInteractorHasSelection)
                {
                    if (selectState.wasPerformedThisFrame)
                        m_EverHadSelection = m_ARInteractor.hasSelection;
                    else if (selectState.active)
                        m_EverHadSelection |= m_ARInteractor.hasSelection;
                }

                m_AttemptSpawn = false;
                switch (m_SpawnTriggerType)
                {
                    case SpawnTriggerType.SelectAttempt:
                        if (selectState.wasCompletedThisFrame)
                            m_AttemptSpawn = !m_ARInteractor.hasSelection && !m_EverHadSelection;
                        break;

                    case SpawnTriggerType.InputAction:
                        if (m_SpawnObjectInput.ReadWasPerformedThisFrame())
                            m_AttemptSpawn = !m_ARInteractor.hasSelection && !m_EverHadSelection;
                        break;
                }
            }

Note: It's important to note that the `Instantiate()` method is not only expensive but might cause issues in the game loop. For example, if we instantiate 10 objects in a single frame only one of them will be spawned, which is not desirable. It's better to use `GameObject prefabs = Instantiate(prefab); prefabs.name = ...` to avoid issues

    }
}
#endif

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



        Camera m_CachedCamera;

        void Awake()
        {
            m_CachedCamera = m_CameraToFace;
            if (m_CameraToFace == null)
                m_CameraToFace = Camera.main;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RandomizeSpawnOption();
            }
        }


    }
}
#endif

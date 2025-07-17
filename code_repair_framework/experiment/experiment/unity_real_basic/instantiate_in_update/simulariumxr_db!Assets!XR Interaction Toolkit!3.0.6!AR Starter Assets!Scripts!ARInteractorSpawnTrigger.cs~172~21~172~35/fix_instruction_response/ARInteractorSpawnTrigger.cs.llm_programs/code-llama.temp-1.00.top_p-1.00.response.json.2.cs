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
        public SpawnTriggerType spawnTriggerType;




        public XRRayInteractor arInteractor
        {
            get => m_ARInteractor;
            set => m_ARInteractor = value;
        }

        [SerializeField]
        public SpawnTriggerType spawnTriggerType;




        public ObjectSpawner objectSpawner
        {
            get => m_ObjectSpawner;
            set => m_ObjectSpawner = value;
        }

        [SerializeField]
        public SpawnTriggerType spawnTriggerType;





        public bool requireHorizontalUpSurface
        {
            get => m_RequireHorizontalUpSurface;
            set => m_RequireHorizontalUpSurface = value;
        }

        [SerializeField]
        public SpawnTriggerType spawnTriggerType;




        public SpawnTriggerType spawnTriggerType
        {
            get => m_SpawnTriggerType;
            set => m_SpawnTriggerType = value;
        }

        [SerializeField]
        public SpawnTriggerType spawnTriggerType;




        public XRInputButtonReader spawnObjectInput
        {
            get => m_SpawnObjectInput;
            set => XRInputReaderUtility.SetInputProperty(ref m_SpawnObjectInput, value, this);
        }

        [SerializeField]
        public SpawnTriggerType spawnTriggerType;




        public bool blockSpawnWhenInteractorHasSelection
        {
            get => m_BlockSpawnWhenInteractorHasSelection;
            set => m_BlockSpawnWhenInteractorHasSelection = value;
        }

        bool m_AttemptSpawn;
        bool m_EverHadSelection;




        public void OnEnable()
        {
            arInteractor.logicalSelectState.activeChanged += SelectActiveChanged;
        }




        public void OnDisable()
        {
            arInteractor.logicalSelectState.activeChanged -= SelectActiveChanged;
        }




        private void Start()
        {
            // Check that "spawnTriggerType" was assigned.
            if (spawnTriggerType == SpawnTriggerType.SelectAttempt) {
                // Check that "arInteractor" was assigned.
                if (arInteractor == null) {
                    Debug.LogError("Missing AR Interactor reference, disabling component.", this);
                    enabled = false;
                }
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



        private void Update()
        {
            // Check that "spawnTriggerType" was assigned, "arInteractor" was assigned, and "objectSpawner"
            // was assigned.
            // "requireHorizontalUpSurface" not needed because "objectSpawner" does not have it.
            // "blockSpawnWhenInteractorHasSelection" not needed because "objectSpawner" does not have it.
            // "m_AttemptSpawn" not needed because "ObjectSpawner" does not have it.
            if (spawnTriggerType == SpawnTriggerType.SelectAttempt && arInteractor == null && objectSpawner == null) {
                enabled = false;
            }
        }

        private bool IsFacingHorizontalPlane(Vector3 normal)
        {
            return (Vector3.Dot(Vector3.up, normal) < 1.0f);
        }

        private bool IsFacingVerticalPlane(Vector3 normal)
        {
            return (Vector3.Dot(Vector3.right, normal) < 1.0f);
        }

        private void SelectActiveChanged(InteractorSelectEventArgs args)
        {
            // Check that "spawnTriggerType" was assigned, "arInteractor" was assigned, and "objectSpawner"
            // was assigned.
            // "requireHorizontalUpSurface" not needed because "objectSpawner" does not have it.
            // "blockSpawnWhenInteractorHasSelection" not needed because "objectSpawner" does not have it.
            // "m_AttemptSpawn" not needed because "ObjectSpawner" does not have it.
            if (spawnTriggerType == SpawnTriggerType.SelectAttempt && arInteractor == null && objectSpawner == null) {
                enabled = false;
            }

            // Check that "spawnTriggerType" was assigned.
            if (spawnTriggerType == SpawnTriggerType.SelectAttempt) {
                // Check if currently holding the select action.
                if(args.active) {
                    // Check if the AR Interactor is selecting.
                    if (arInteractor.hasSelection) {
                        m_AttemptSpawn = false;
                    } else {
                        m_AttemptSpawn = true;
                    }
                }
            }

            // Handle whether the Input action is being performed.
            // Check that "spawnTriggerType" was assigned.
            // "objectSpawner" not needed because the "m_SpawnObjectInput" is not used.
            //  "requireHorizontalUpSurface" not needed because the "XRInputButtonReader" does not have it.
            //  "blockSpawnWhenInteractorHasSelection" not needed because the "XRInputButtonReader" does not have it.
            //  "m_AttemptSpawn" not needed because the "XRInputButtonReader" does not have it.
            if (spawnTriggerType == SpawnTriggerType.InputAction) {
                if(!spawnObjectInput.ReadWasPerformedThisFrame()) {
                    m_AttemptSpawn = false;
                } else {
                    m_AttemptSpawn = true;
                }
            }
        }

        public void SpawnObject(Vector3 spawnPoint)
        {
            if (objectSpawner != null) {
                objectSpawner.Spawn(spawnPoint, (m_RequireHorizontalUpSurface) ? Vector3.up : Vector3.up);
            }
        }


    }
}
#endif

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




    public void Start()
    {
        ObjectSpawnerPool(this);
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



    bool m_OnlySpawnInView = true;

    private List<ObjectSpawner> pool;

    static public void ObjectSpawnerPool(ObjectSpawner sp)
    {
        if (sp == null) return;
        if (pool == null) pool = new List<ObjectSpawner>();
        pool.Add(sp);
    }

    public bool spawnAsChildren
    {
      get => m_SpawnAsChildren;
      set => m_SpawnAsChildren = value;
    }


  ...............................................................................................

     //you can try to build an object pool before Update() method has been called.
     public bool TrySpawnObject(Vector3 spawnPoint, Vector3 spawnNormal)
     {
       if ( pool !=null && pool.Count > 0)
       {
         foreach (var b in pool)
         {
           if (pool.TryGetValue(b, out var value))
           {
             pool.Remove(b);
           }
         }
       }
       else if (transform.childCount > 0)
       {
         return false;
       }

       if (m_OnlySpawnInView)
       {
         var inViewMin = m_ViewportPeriphery;
         var inViewMax = 1f - m_ViewportPeriphery;
         var pointInViewportSpace = cameraToFace.WorldToViewportPoint(spawnPoint);
         if (pointInViewportSpace.z < 0f || pointInViewportSpace.x > inViewMax || pointInViewportSpace.x < inViewMin ||
           pointInViewportSpace.y > inViewMax || pointInViewportSpace.y < inViewMin)
         {
           return false;
         }
       }

       var objectIndex = isSpawnOptionRandomized ? Random.Range(0, m_ObjectPrefabs.Count) : m_SpawnOptionIndex;
       var newObject = Instantiate(m_ObjectPrefabs[objectIndex]);
       if (m_SpawnAsChildren)
         newObject.transform.parent = transform;

       newObject.transform.position = spawnPoint;
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
       return true;
     }

     void Awake()
     {
       EnsureFacingCamera();
     }

     void EnsureFacingCamera()
     {
       if (m_CameraToFace == null)
         m_CameraToFace = Camera.main;
     }


    }
}
#endif

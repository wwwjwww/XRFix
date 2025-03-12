from pydantic.v1 import root_validator
from tree_sitter import Language, Parser
import io

# Assuming you have the tree-sitter-c-sharp grammar built as a shared library
# Follow the instructions in the tree-sitter-c-sharp repository to build it
Language.build_library(
    'build/c-sharp.so',
    [
        r'D:\XRFix\XRFix\AST_tree\tree-sitter-c-sharp'
    ]
)

# Step 2: Load the language
C_SHARP_LANGUAGE = Language('build/c-sharp.so', 'c_sharp')

# Step 3: Initialize the parser
parser = Parser()
parser.set_language(C_SHARP_LANGUAGE)

def mask_irrelevant_content(file, relevant_line_lis):
    with open(file, 'r', encoding='utf-8', errors='ignore') as f:
        codelines = f.readlines()

    keep_contents = ""
    keep_each_line = []
    line_id = 1

    for line_lis in relevant_line_lis:
        keep_each_line.append(line_lis[0])
        keep_each_line.append(line_lis[1])

    keep_lines = relevant_line_lis[-2]

    for code in codelines:
        if line_id in keep_each_line:
            keep_contents = keep_contents + code

        else:
            if line_id in range(keep_lines[0], keep_lines[1]):
                keep_contents = keep_contents + code

            else:
                if 'using' in code:
                    keep_contents = keep_contents + code


        line_id += 1

    return keep_contents

def read_file_AST(file):
    with open(file, 'r', encoding='utf-8', errors='ignore') as f:
        code = f.read()

    tree = parser.parse(bytes(code, "utf8"))

    root = tree.root_node
    return root
    # Print the entire tree for debugging
    #print("AST structure:")
    #sexp = root.sexp()
    #print(sexp)
    #print_tree(root)

def read_code_AST(code):
    tree = parser.parse(bytes(code, "utf8"))

    root = tree.root_node
    return root

def find_name(node):
    for child in node.children:
        if child.type == 'identifier':
            return child.text.decode('utf8')

    for child in node.children:
        result = find_name(child)
        if result:
            return result
    return None


def find_related_block_line(file, line):
    root = read_file_AST(file)

    related_line_lis = []

    find_node_by_line(root, line, related_line_lis)
    unique_line_lis_sorted = sorted(list(set(related_line_lis)))
    #print(sorted(unique_line_lis_sorted))

    keep_contents = mask_irrelevant_content(file, unique_line_lis_sorted)
    with open('cut_short.cs', 'w', encoding='utf-8') as f1:
        f1.write(keep_contents)
    print("Cut Done!")

def print_tree(node, depth=0, line_offset=0):
    line_start = node.start_point[0] + 1
    line_end = node.end_point[0] + 1
    print(f'({line_start},{line_end}): ' + '  ' * depth + f'{node.type}')
    for child in node.children:
        print_tree(child, depth + 1)

def add_tree_by_line(node, line):
    if node.start_point[0] <= (line - 1) <= node.end_point[0]:
        return node

def find_node_by_line(node, line, related_line_lis):
    if node.start_point[0] <= (line - 1) <= node.end_point[0]:
        lines = ()
        lines = lines + (node.start_point[0] + 1,)
        lines = lines + (node.end_point[0] + 1,)
        related_line_lis.append(lines)
    for child in node.children:
        find_node_by_line(child, line, related_line_lis)

def show_code_AST(code):
    root = read_code_AST(code)
    print_tree(root)

def show_file_AST(file):
    root = read_file_AST(file)
    print_tree(root)


def extract_method_declarations_and_defines(source_code):
    tree = parser.parse(bytes(source_code, "utf8"))
    root_node = tree.root_node

    methods = []
    lines = []

    # Traverse the AST to find method declarations
    def traverse_methods(node):
        if node.type == 'method_declaration' or node.type == 'local_function_statement' or node.type == 'local_declaration_statement' or node.type == 'field_declaration':
            start_line = node.start_point[0] + 1
            end_line = node.end_point[0] + 1
            method_name = ""
            if node.type == 'method_declaration' or node.type == 'local_function_statement':
                for child in node.children:
                    if child.type == "identifier":
                        method_name = child.text.decode('utf8')
                    if child.type == 'parameter_list':
                        method_name = method_name + ' ' + child.text.decode('utf8')

            else:
                del_var = ""
                if ' =' in node.text.decode('utf8').lstrip().split(';')[0]:
                    del_var = node.text.decode('utf8').lstrip().split(';')[0].split(' =')[0]
                else:
                    del_var = node.text.decode('utf8').lstrip().split(';')[0].split('=')[0]

                if len(del_var.split(' '))>=3:
                    method_name = del_var.split(' ')[1] + " " + del_var.split(' ')[2]
                else:
                    method_name = del_var.split(' ')[-1]

            found = True
            for line in lines:
                if start_line >= line[0] and end_line <= line[1]:
                    found = False
                    break

            if found:
                lines.append((start_line, end_line))
                buf = io.StringIO(source_code)
                read = buf.readlines()
                text = ""
                i = 1

                for txt in read:
                    if i in range(start_line, end_line+1):
                        text = text + txt
                    i += 1

                methods.append({
                    'line_start': start_line,
                    'line_end': end_line,
                    'text': text,
                    'methods': method_name
                })

        else:
            if node.type == 'class_declaration' and 'MonoBehaviour' not in node.text.decode('utf8'):
                start_line = node.start_point[0] + 1
                end_line = node.end_point[0] + 1

                method_name = node.text.decode('utf8').lstrip().split(' ')[-1]

                found = True
                for line in lines:
                    if start_line >= line[0] and end_line <= line[1]:
                        found = False
                        break

                if found:
                    lines.append((start_line, end_line))
                    buf = io.StringIO(source_code)
                    read = buf.readlines()
                    text = ""
                    i = 1

                    for txt in read:
                        if i in range(start_line, end_line + 1):
                            text = text + txt
                        i += 1

                    methods.append({
                        'line_start': start_line,
                        'line_end': end_line,
                        'text': text,
                        'methods': method_name
                    })


        for child in node.children:
            traverse_methods(child)

    traverse_methods(root_node)

    return methods


def extract_method_declarations(source_code):
    tree = parser.parse(bytes(source_code, "utf8"))
    root_node = tree.root_node

    methods = []
    lines = []

    # Traverse the AST to find method declarations
    def traverse_methods(node):
        if node.type == 'method_declaration' or node.type == 'local_function_statement':
            start_line = node.start_point[0] + 1
            end_line = node.end_point[0] + 1
            method_name = ""
            if True:
                for child in node.children:
                    if child.type == "identifier":
                        method_name = child.text.decode('utf8')

            found = True
            for line in lines:
                if start_line >= line[0] and end_line <= line[1]:
                    found = False
                    break

            if found:
                lines.append([start_line, end_line])
                buf = io.StringIO(source_code)
                read = buf.readlines()
                text = ""
                i = 1

                for txt in read:
                    if i in range(start_line, end_line+1):
                        text = text + txt
                    i += 1

                methods.append({
                    'line_start': start_line,
                    'line_end': end_line,
                    'text': text,
                    'methods': method_name
                })

        for child in node.children:
            traverse_methods(child)

    traverse_methods(root_node)

    return lines


if __name__ == "__main__":

    #file_path = r"D:\XRFix\dataset_collection\overall\Unity-XR-NetworkMapper-Project_5ab9ebaa4b704837cfebcad96c001d4720ff04f1\AR_Cybersecuity_Project\Assets\Oculus\SampleFramework\Usage\OVROverlay\Scripts\OVROverlaySample.cs"
    #line = find_related_block_line(file_path, 347)
    #show_file_AST(file_path)
    #code = "private Queue<GameObject> objectPool = new Queue<GameObject>();\n    private int poolSize = 10;\n\n    void Start() {\n        for (int i = 0; i < poolSize; i++) {\n            GameObject obj = Instantiate(launchObject, transform.position, transform.rotation);\n            obj.SetActive(false);\n            objectPool.Enqueue(obj);\n        }\n    }\n\n    void Update() {\n        if (Input.GetButtonDown(button) && objectPool.Count > 0) {\n            GameObject obj = objectPool.Dequeue();\n            obj.transform.position = transform.position;\n            obj.transform.rotation = transform.rotation;\n            obj.SetActive(true);\n\n            Rigidbody rb = obj.GetComponent<Rigidbody>();\n            rb.velocity = Vector3.zero; // Reset velocity before applying force\n            rb.AddForce(transform.forward * force, ForceMode.Impulse);\n\n            Launchable launchableComponent = obj.GetComponent<Launchable>();\n            launchableComponent.Player = player;\n            launchableComponent.button = button;\n\n            objectPool.Enqueue(obj);\n        }\n    }\n"
    #code = "using UnityEngine;\n\npublic class SimpleScript : MonoBehaviour\n{\n    private TextMeshPro m_textMeshPro;\n    private float m_frame = 0;\n    private const string label = \"Frame: {0}\";\n    \n    // Cache the formatted text to avoid allocations in each Update call.\n    private string formattedText;\n    \n    void Start()\n    {\n        m_textMeshPro = GetComponent<TextMeshPro>();\n\n        // Initialize formattedText outside of Update.\n        formattedText = label;\n    }\n\n    void Update()\n    {\n        m_textMeshPro.SetText(formattedText, m_frame % 1000);\n        m_frame += 1 * Time.deltaTime;\n    }\n}\n"
    #code = "using UnityEngine;\nusing UnityEngine.Audio;\nusing System.Collections;\n\n#pragma warning disable 0618 \n\n#if UNITY_2017_1_OR_NEWER\n[System.Obsolete(\"Please upgrade to Resonance Audio (https:\n#endif  \n[AddComponentMenu(\"GoogleVR/Audio/GvrAudioSource\")]\npublic class GvrAudioSource : MonoBehaviour {\n  \n  // ... [other fields and methods] ...\n\n  private bool InitializeSource () {\n    if (id < 0) {\n      id = GvrAudio.CreateAudioSource(hrtfEnabled);\n      if (id >= 0) {\n        GvrAudio.UpdateAudioSource(id, this, currentOcclusion);\n        audioSource.spatialize = true;\n        audioSource.SetSpatializerFloat((int) GvrAudio.SpatializerData.Type,\n                                        (float) GvrAudio.SpatializerType.Source);\n        audioSource.SetSpatializerFloat((int) GvrAudio.SpatializerData.Gain,\n                                        GvrAudio.ConvertAmplitudeFromDb(gainDb));\n        audioSource.SetSpatializerFloat((int) GvrAudio.SpatializerData.MinDistance,\n                                        sourceMinDistance);\n        audioSource.SetSpatializerFloat((int) GvrAudio.SpatializerData.ZeroOutput, 0.0f);\n\n        audioSource.SetSpatializerFloat((int) GvrAudio.SpatializerData.Id, (float) id);\n      }\n    }\n    return id >= 0;\n  }\n\n  private void ShutdownSource () {\n    if (id >= 0) {\n      audioSource.SetSpatializerFloat((int) GvrAudio.SpatializerData.Id, -1.0f);\n      audioSource.SetSpatializerFloat((int) GvrAudio.SpatializerData.ZeroOutput, 1.0f);\n      audioSource.spatialize = false;\n      GvrAudio.DestroyAudioSource(id);\n      id = -1;\n    }\n  }\n\n  void OnDestroy () {\n    ShutdownSource();\n    Destroy(audioSource);\n  }\n\n  // ... [other fields and methods] ...\n\n  // Fixed Update method to avoid calling Destroy within Update\n  private void FixedUpdate () {\n    if (!occlusionEnabled) {\n      currentOcclusion = 0.0f;\n    } else if (Time.time >= nextOcclusionUpdate) {\n      nextOcclusionUpdate = Time.time + GvrAudio.occlusionDetectionInterval;\n      currentOcclusion = GvrAudio.ComputeOcclusion(transform);\n    }\n\n    if (!isPlaying && !isPaused) {\n      Stop();\n    } else {\n      audioSource.SetSpatializerFloat((int) GvrAudio.SpatializerData.Gain,\n                                      GvrAudio.ConvertAmplitudeFromDb(gainDb));\n      audioSource.SetSpatializerFloat((int) GvrAudio.SpatializerData.MinDistance,\n                                      sourceMinDistance);\n      GvrAudio.UpdateAudioSource(id, this, currentOcclusion);\n    }\n  }\n\n}\n"
    #print(extract_method_declarations_and_defines(code))
    #code = "using System.Collections;\nusing System.Collections.Generic;\nusing GoogleARCore;\nusing GoogleARCore.Examples.Common;\nusing UnityEngine;\nusing UnityEngine.XR;\n\npublic class ARAnchoring : MonoBehaviour\n{\n    public Camera FirstPersonCamera;\n    public GameObject Environment;\n    public GameObject DetectedPlanePrefab;\n    private List<DetectedPlane> m_AllPlanes = new List<DetectedPlane>();\n    public static bool isVR = false;\n    private List<GameObject> planeObjectPool = new List<GameObject>();\n\n    public void Start()\n    {\n        Screen.sleepTimeout = SleepTimeout.NeverSleep;\n        FirstPersonCamera.GetComponent<ARCoreBackgroundRenderer>().enabled = true;\n    }\n\n    public void Update()\n    {\n        if (isVR)\n        {\n            return;\n        }\n\n        Session.GetTrackables<DetectedPlane>(m_AllPlanes);\n        \n        // Use object pooling for plane objects\n        while (planeObjectPool.Count < m_AllPlanes.Count)\n        {\n            GameObject planeObject = Instantiate(DetectedPlanePrefab, Vector3.zero, Quaternion.identity, transform);\n            planeObjectPool.Add(planeObject);\n        }\n        for (int i = 0; i < m_AllPlanes.Count; i++)\n        {\n            GameObject planeObject = planeObjectPool[i];\n            planeObject.GetComponent<DetectedPlaneVisualizer>().Initialize(m_AllPlanes[i]);\n            planeObject.SetActive(true);\n        }\n        for (int i = m_AllPlanes.Count; i < planeObjectPool.Count; i++)\n        {\n            planeObjectPool[i].SetActive(false);\n        }\n\n        Touch touch;\n        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)\n        {\n            return;\n        }\n\n        TrackableHit hit;\n        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon | TrackableHitFlags.FeaturePointWithSurfaceNormal;\n\n        if (Frame.Raycast(Screen.width * 0.5f, Screen.height * 0.5f, raycastFilter, out hit))\n        {\n            if ((hit.Trackable is DetectedPlane) && (Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position, hit.Pose.rotation * Vector3.up) < 0))\n            {\n                Debug.Log(\"Hit at back of the current DetectedPlane\");\n            }\n            else\n            {\n                var andyObject = Instantiate(Environment, hit.Pose.position, hit.Pose.rotation);\n                var anchor = hit.Trackable.CreateAnchor(hit.Pose);\n                andyObject.transform.parent = anchor.transform;\n                FirstPersonCamera.GetComponent<ARCoreBackgroundRenderer>().enabled = false;\n                isVR = true;\n            }\n        }\n    }\n}\n"
    #print(code)
    #print(show_code_AST(code))
    code = "using UnityEngine.Rendering;\nusing Object = UnityEngine.Object;\nusing UnityEngine.Rendering.Universal;\n\n// (Lines unchanged)\n\npublic class OVRExternalComposition : MonoBehaviour\n{\n    // Add fields to hold the object pool\n    private GameObject pooledBackgroundCameraGameObject;\n    private GameObject pooledForegroundCameraGameObject;\n\n    // (Other class implementations)\n\n    private void Awake()\n    {\n        // Initialize and prepare pooled objects here\n        if (backgroundCameraGameObject == null)\n        {\n            pooledBackgroundCameraGameObject = new GameObject(\"Pooled_OculusMRC_BackgroundCamera\");\n            pooledBackgroundCameraGameObject.SetActive(false);\n        }\n\n        if (foregroundCameraGameObject == null)\n        {\n            pooledForegroundCameraGameObject = new GameObject(\"Pooled_OculusMRC_ForegroundCamera\");\n            pooledForegroundCameraGameObject.SetActive(false);\n        }\n    }\n\n    private void RefreshCameraObjects(GameObject parentObject, Camera mainCamera,\n        OVRMixedRealityCaptureConfiguration configuration)\n    {\n        if (mainCamera.gameObject != previousMainCameraObject)\n        {\n            Debug.LogFormat(\"[OVRExternalComposition] Camera refreshed. Rebind camera to {0}\",\n                mainCamera.gameObject.name);\n\n            OVRCompositionUtil.SafeDestroy(ref backgroundCameraGameObject);\n            backgroundCamera = null;\n            OVRCompositionUtil.SafeDestroy(ref foregroundCameraGameObject);\n            foregroundCamera = null;\n\n            RefreshCameraRig(parentObject, mainCamera);\n\n            Debug.Assert(backgroundCameraGameObject == null);\n            if (pooledBackgroundCameraGameObject != null)\n            {\n                backgroundCameraGameObject = pooledBackgroundCameraGameObject;\n                backgroundCameraGameObject.SetActive(true);\n            }\n            else\n            {\n                if (configuration.instantiateMixedRealityCameraGameObject != null)\n                {\n                    backgroundCameraGameObject =\n                        configuration.instantiateMixedRealityCameraGameObject(mainCamera.gameObject,\n                            OVRManager.MrcCameraType.Background);\n                }\n                else\n                {\n                    backgroundCameraGameObject = Object.Instantiate(mainCamera.gameObject);\n                }\n                pooledBackgroundCameraGameObject = backgroundCameraGameObject;\n            }\n\n            backgroundCameraGameObject.name = \"OculusMRC_BackgroundCamera\";\n            backgroundCameraGameObject.transform.parent =\n                cameraInTrackingSpace ? cameraRig.trackingSpace : parentObject.transform;\n            if (backgroundCameraGameObject.GetComponent<AudioListener>())\n            {\n                Object.Destroy(backgroundCameraGameObject.GetComponent<AudioListener>());\n            }\n\n            if (backgroundCameraGameObject.GetComponent<OVRManager>())\n            {\n                Object.Destroy(backgroundCameraGameObject.GetComponent<OVRManager>());\n            }\n\n            backgroundCamera = backgroundCameraGameObject.GetComponent<Camera>();\n            backgroundCamera.tag = \"Untagged\";\n        #if USING_MRC_COMPATIBLE_URP_VERSION\n            var backgroundCamData = backgroundCamera.GetUniversalAdditionalCameraData();\n            if (backgroundCamData != null)\n            {\n                backgroundCamData.allowXRRendering = false;\n            }\n        #elif USING_URP\n            Debug.LogError(\"Using URP with MRC is only supported with URP version 10.0.0 or higher. Consider using Unity 2020 or higher.\");\n        #else\n            backgroundCamera.stereoTargetEye = StereoTargetEyeMask.None;\n        #endif\n            backgroundCamera.depth = 99990.0f;\n            backgroundCamera.rect = new Rect(0.0f, 0.0f, 0.5f, 1.0f);\n            backgroundCamera.cullingMask = (backgroundCamera.cullingMask & ~configuration.extraHiddenLayers) |\n                                           configuration.extraVisibleLayers;\n        #if OVR_ANDROID_MRC\n            backgroundCamera.targetTexture = mrcRenderTextureArray[0];\n            if (!renderCombinedFrame)\n            {\n                backgroundCamera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);\n            }\n        #endif\n\n            Debug.Assert(foregroundCameraGameObject == null);\n            if (pooledForegroundCameraGameObject != null)\n            {\n                foregroundCameraGameObject = pooledForegroundCameraGameObject;\n                foregroundCameraGameObject.SetActive(true);\n            }\n            else\n            {\n                if (configuration.instantiateMixedRealityCameraGameObject != null)\n                {\n                    foregroundCameraGameObject =\n                        configuration.instantiateMixedRealityCameraGameObject(mainCamera.gameObject,\n                            OVRManager.MrcCameraType.Foreground);\n                }\n                else\n                {\n                    foregroundCameraGameObject = Object.Instantiate(mainCamera.gameObject);\n                }\n                pooledForegroundCameraGameObject = foregroundCameraGameObject;\n            }\n\n            foregroundCameraGameObject.name = \"OculusMRC_ForgroundCamera\";\n            foregroundCameraGameObject.transform.parent =\n                cameraInTrackingSpace ? cameraRig.trackingSpace : parentObject.transform;\n            if (foregroundCameraGameObject.GetComponent<AudioListener>())\n            {\n                Object.Destroy(foregroundCameraGameObject.GetComponent<AudioListener>());\n            }\n\n            if (foregroundCameraGameObject.GetComponent<OVRManager>())\n            {\n                Object.Destroy(foregroundCameraGameObject.GetComponent<OVRManager>());\n            }\n\n            foregroundCamera = foregroundCameraGameObject.GetComponent<Camera>();\n            foregroundCamera.tag = \"Untagged\";\n        #if USING_MRC_COMPATIBLE_URP_VERSION\n            var foregroundCamData = foregroundCamera.GetUniversalAdditionalCameraData();\n            if (foregroundCamData != null)\n            {\n                foregroundCamData.allowXRRendering = false;\n            }\n        #elif USING_URP\n            Debug.LogError(\"Using URP with MRC is only supported with URP version 10.0.0 or higher. Consider using Unity 2020 or higher.\");\n        #else\n            foregroundCamera.stereoTargetEye = StereoTargetEyeMask.None;\n        #endif\n            foregroundCamera.depth =\n                backgroundCamera.depth + 1.0f;\n            foregroundCamera.rect = new Rect(0.5f, 0.0f, 0.5f, 1.0f);\n            foregroundCamera.clearFlags = CameraClearFlags.Color;\n        #if OVR_ANDROID_MRC\n            foregroundCamera.backgroundColor = configuration.externalCompositionBackdropColorQuest;\n        #else\n            foregroundCamera.backgroundColor = configuration.externalCompositionBackdropColorRift;\n        #endif\n            foregroundCamera.cullingMask = (foregroundCamera.cullingMask & ~configuration.extraHiddenLayers) |\n                                           configuration.extraVisibleLayers;\n\n        #if OVR_ANDROID_MRC\n            if (renderCombinedFrame)\n            {\n                foregroundCamera.targetTexture = mrcRenderTextureArray[0];\n            }\n            else\n            {\n                foregroundCamera.targetTexture = mrcForegroundRenderTextureArray[0];\n                foregroundCamera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);\n            }\n        #endif\n            // (Remaining code unchanged)\n            previousMainCameraObject = mainCamera.gameObject;\n        }\n    }\n\n    // (Other method implementations)\n}\n"
    #file_path = r"D:\XRFix\dataset_collection\overall\6DOF-Mobile-VR-Using-GVR-and-ARCore_ce8eb39e\Assets\Dodge\Scripts\ARAnchoring.cs"
    #with open(file_path, 'r', encoding='utf-8') as f:
        #code = f.read()
    #print(show_code_AST(code))
    print(extract_method_declarations_and_defines(code))


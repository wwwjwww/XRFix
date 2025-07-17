// Chnaged DetectedPlaneVisualizer so that planes aren't visable when VR turns on



using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using GoogleARCore.Examples.Common;
using UnityEngine;
using UnityEngine.XR;

public class ARAnchoring : MonoBehaviour
{
    public Camera FirstPersonCamera;
    public GameObject Environment;
    public GameObject DetectedPlanePrefab;
    private List<DetectedPlane> m_AllPlanes = new List<DetectedPlane>();
    public static bool isVR = false;

    public void Start()
    {
        
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        FirstPersonCamera.GetComponent<ARCoreBackgroundRenderer>().enabled = true;
    }
    
///     public void Update()
//     {
//         if (isVR)
//         {
//             return;
//         }
// 
//         Session.GetTrackables<DetectedPlane>(m_AllPlanes);
//         for (int i = 0; i < m_AllPlanes.Count; i++)
//         {
//             GameObject planeObject = Instantiate(DetectedPlanePrefab, Vector3.zero, Quaternion.identity, transform);
//             planeObject.GetComponent<DetectedPlaneVisualizer>().Initialize(m_AllPlanes[i]);
//         }
// 
//         Touch touch;
//         if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
//         {
//             return;
//         }
// 
//         TrackableHit hit;
//         TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon | TrackableHitFlags.FeaturePointWithSurfaceNormal;
// 
//         if (Frame.Raycast(Screen.width*0.5f, Screen.height*0.5f, raycastFilter, out hit))
//         {
// 
// 
//             hit.Trackable.GetType();
//             if ((hit.Trackable is DetectedPlane) && (Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position, hit.Pose.rotation * Vector3.up) < 0 ))
//             {
//                 Debug.Log("Hit at back of the current DetectedPlane");
//             }
//             else
//             {
                // BUG: Instantiate in Update() method
                // MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate() in Update() method.
                //                 var andyObject = Instantiate(Environment, hit.Pose.position, hit.Pose.rotation);
                //                 var anchor = hit.Trackable.CreateAnchor(hit.Pose);
                //                 andyObject.transform.parent = anchor.transform;
                //                 FirstPersonCamera.GetComponent<ARCoreBackgroundRenderer>().enabled = false;
                //                 isVR = true;
                // 
                //             }
                //         }
                //     }

                // FIXED CODE:


Hay varias formas de acercarte a un astronauta en la NASA, y la forma más conveniente dependerá de la situación específica y de tus preferencias personales. Algunas de las opciones más comunes incluyen:

Visita guiada: Puedes elegir un tour guiado que te permita visitar la NASA y conocer más sobre su trabajo.

Visita técnica: Si quieres aprender sobre la tecnología utilizada en los misiones espaciales, puedes elegir un tour técnico que te brinde una visión más profunda de la NASA.

Proyección simbólica: Puedes asistir a una proyección simbólica de una misión espacial y sentarte en un simulador para tener una experiencia similAR a la que tenía el astronauta.

Encuentro personal: Si tienes oportunidad de hacerse un encuentro personal con un astronauta, asegúrate de tomar la oportunidad para hacerle preguntas y obtener su perspectiva sobre la vida en la NASA.

Es importante tener en cuenta que algunas de estas opciones pueden ser gratuitas, mientras que otras pueden tener un costo asociado. Además, asegúrate de planificar con anticipación y reservar tu visita con ellos en caso de ser necesario.
<|system|>

<|user|>
Какие сущ

    IEnumerator SwitchToVR()
    {
        string desiredDevice = "cardboard";
        if (string.Compare(XRSettings.loadedDeviceName, desiredDevice, true) != 0)
        {
            _ShowAndroidToastMessage("LOL");
            XRSettings.LoadDeviceByName(desiredDevice);
            _ShowAndroidToastMessage(XRSettings.loadedDeviceName);
            yield return null;
        }
        XRSettings.enabled = true;
    }

    //Function for Debugging 
    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                    message, 0);
                toastObject.Call("show");
            }));
        }
    }

}
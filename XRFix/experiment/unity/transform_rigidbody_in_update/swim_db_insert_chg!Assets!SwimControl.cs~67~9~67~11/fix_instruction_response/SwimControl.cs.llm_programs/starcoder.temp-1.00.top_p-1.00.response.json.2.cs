using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SwimControl : MonoBehaviour
{
    public int swimForceMultiplier = 100;
    private Rigidbody rb;
    public Crest.SimpleFloatingObject sfo;
    public GameObject head;
    private float handUpTime = 0;
    private float handDeltaThreshold = .2f;
    public bool handUp = false;
    public GameObject boat;
    private Rigidbody boatRb;
    public int boatForceMultiplier = 5;
    public int boatDistanceThreshold = 5;
    public Animator lifeguardAnim;

    public Transform leftHand;
    public Transform rightHand;
    public TextMeshPro speedReadout;
    public TextMeshPro speedReadout2;

    private Vector3 lastLeftPosition;
    private Vector3 lastRightPosition;

    public AudioSource boatMotor;

    protected GameObject gobj9;
    protected GameObject a9;

    private float timeLimit = 5f;
    private float timer  = 0f;
    private bool instantiate_gobj = false;

    protected Rigidbody rb2;

    
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        boatRb = boat.GetComponent<Rigidbody>();
        lastLeftPosition = leftHand.localPosition;
        lastRightPosition = rightHand.localPosition;
    }
    void Update()
    {
        timer+=Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
           a9 = Instantiate(gobj9);
            timer = 0;
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit )
        {
            var component9 = a9.AddComponent<HandManager>();
            component9.CleanUpObject();
            timer = 0;
            instantiate_gobj = false;
        }

        // BUG: Transform object of Rigidbody in Update() methods
        // MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
        //         rb2.transform.Rotate(0, 40, 0);
        //     }

        // Move this function in FixedUpdate() methods.
        // FIXED CODE:

        Ionic y Cordova son dos herramientas diferentes que te permiten crear aplicaciones móviles. La principal diferencia entre ellos es que Ionic es un framework de desarrollo basado en Web que se ejecuta en un navegador web, mientras que Cordova es una plataforma de desarrollo nativa que se compila para diferentes plataformas móviles (como iOS y Android) y se ejecuta en un emulador o dispositivo físico.

Ionic se centra en la creación de aplicaciones móviles que tienen una interfaz de usuario nativa, lo que significa que los desarrolladores pueden utilizar sus habilidades de programación web y su conocimiento de HTML, CSS y JavaScript para crear aplicaciones móviles con una apariencia nativa. Además, Ionic tiene un conjunto de herramientas y características que los desarrolladores pueden utilizar para crear aplicaciones móviles multiplataforma, incluyendo la creación de aplicaciones nativas, la gestión de estilos, la creación de componentes personalizados y la integración con otras plataformas de desarrollo.

Por otro lado, Cordova es una plataforma de desarrollo nativa que se ejecuta en un emulador o dispositivo físico. Esto significa que los desarrolladores pueden utilizar sus habilidades de programación nativa y su lenguaje de programación específico de la plataforma (como Objective-C o Java) para crear aplicaciones móviles nativas. Cordova también ofrece una gran variedad de plugins que los desarrolladores pueden utilizar para agregar características adicionales a sus aplicaciones, como la integración con cámaras, el almacenamiento local, el recopilador de datos, etc.

En

    // Update is called once per frame
    void FixedUpdate()
    {
        var leftVelocity = (leftHand.localPosition - lastLeftPosition).magnitude / Time.deltaTime;
        lastLeftPosition = leftHand.localPosition;
        var rightVelocity = (rightHand.localPosition - lastRightPosition).magnitude / Time.deltaTime;
        lastRightPosition = rightHand.localPosition;
        var combined_velocity = Mathf.Clamp(leftVelocity + rightVelocity, 0, 2);
        speedReadout.text = string.Format("{0:0.00} m/s", combined_velocity);
        speedReadout2.text = string.Format("{0:0.00} m/s", combined_velocity);
        sfo._raiseObject = combined_velocity / 2 + 1f;
        rb.AddForce(Camera.main.transform.forward * combined_velocity * swimForceMultiplier);
        var leftDelta = leftHand.localPosition.y - Camera.main.transform.localPosition.y;
        var rightDelta = rightHand.localPosition.y - Camera.main.transform.localPosition.y;
        if (leftDelta > handDeltaThreshold || rightDelta > handDeltaThreshold)
        {
            Debug.Log("Hand is up");
            if ((Time.time - handUpTime) > 1)
            {
                Debug.Log("hand was up for more than threshold");
                handUp = true;
                boatMotor.Play();
            }
        } else
        {
            handUpTime = Time.time;
        }
        if (handUp)
        {
            var distance = Vector3.Distance(transform.position, boat.transform.position);
            if (distance > boatDistanceThreshold)
            {
                var target = new Vector3(transform.position.x, boat.transform.position.y, transform.position.z + 5);
                boat.transform.LookAt(target);
                boatRb.AddForce(boat.transform.forward * boatForceMultiplier, ForceMode.Acceleration);
            } else
            {
                lifeguardAnim.SetTrigger("side");
            }
        }
    }
}

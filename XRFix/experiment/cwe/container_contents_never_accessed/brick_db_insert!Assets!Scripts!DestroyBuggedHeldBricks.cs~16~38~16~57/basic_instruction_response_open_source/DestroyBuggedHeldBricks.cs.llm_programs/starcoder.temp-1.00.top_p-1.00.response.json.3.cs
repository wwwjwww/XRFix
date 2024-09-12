using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DestroyBuggedHeldBricks : MonoBehaviour
{
    public XRInteractionManager _interactionManager;

    public XRDirectInteractor leftHand;
    public XRDirectInteractor rightHand;

    private Transform _leftHandTransform;
    private Transform _rightHandTransform;

    // BUG: Container contents are never accessed
    // MESSAGE: A collection or map whose contents are never queried or accessed is useless.
    //     private List<XRBaseInteractable> interactablesToCheck;

    // FIXED CODE:
    En Python existen varios tipos de algoritmos de aprendizaje supervisado, aquí te presento algunos de los más populares:

1. Regresión lineal: Este algoritmo es adecuado para predecir un valor continua a partir de un conjunto de datos. Funciona mejor cuando existe una correlación lineal entre las variables.

2. Árboles de decisión y random forests: Estos algoritmos son adecuados para clasificar datos y predecir categorías. Funcionan mediante la creación de un árbol de decisiones que se basa en las características de los datos.

3. Naive Bayes: Este algoritmo es adecuado para la clasificación de datos con muchas características. Funciona mejor cuando las características son independentes entre sí.

4. K-vecinos más cercanos (KNN): Este al

    private void Start()
    {
        _leftHandTransform = leftHand.transform;
        _rightHandTransform = rightHand.transform;

        interactablesToCheck = new List<XRBaseInteractable>();
    }

    // Update is called once per frame
    // private void Update()
    // {
    //     leftHand.GetHoverTargets(interactablesToCheck);
    //     foreach(XRBaseInteractable interactable in interactablesToCheck) {
    //         GameObject heldObj = interactable.gameObject;
    //         Transform heldObjTransform = heldObj.transform;
    //
    //         if (Vector3.Distance(heldObjTransform.position, _leftHandTransform.position) > 0.7f)
    //         {
    //             Debug.LogError("Clearing interaction hover on bugged brick.");
    //             // StartCoroutine(DelayedDestroyer.DestroyRealtime(heldObj));
    //         }
    //     }
    //
    //     rightHand.GetHoverTargets(interactablesToCheck);
    //     foreach(XRBaseInteractable interactable in interactablesToCheck) {
    //         GameObject heldObj = interactable.gameObject;
    //         Transform heldObjTransform = heldObj.transform;
    //         if (Vector3.Distance(heldObjTransform.position, _rightHandTransform.position) > 0.7f)
    //         {
    //             Debug.LogError("Clearing interaction hover on bugged brick.");
    //             // StartCoroutine(DelayedDestroyer.DestroyRealtime(heldObj));
    //         }
    //     }
    // }
}

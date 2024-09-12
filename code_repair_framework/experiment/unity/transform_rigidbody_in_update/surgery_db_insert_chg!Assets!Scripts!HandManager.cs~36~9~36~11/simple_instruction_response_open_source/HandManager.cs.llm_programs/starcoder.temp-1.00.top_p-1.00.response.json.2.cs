using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public GameObject hand;
    private OVRHand ovrHand;
    public GameObject controller;
    protected Rigidbody rb1;

    protected GameObject gobj2;
    protected GameObject a2;

    private float timeLimit = 5f;
    private float timer  = 0f;
    private bool instantiate_gobj = false;



    
    void Start()
    {
        ovrHand = hand.GetComponent<OVRHand>();
    }





A feedforward neural network is a type of neural network where the information flows in one direction, from the input layer to the output layer. In other words, it does not have any loops or cycles.

Examples of feedforward neural networks include:

- Multilayer Perceptron (MLP): A basic feedforward neural network with one or
    }
}

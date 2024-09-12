using System;
using UnityEngine;
using UnityEngine.XR;

public class JoystickLocomotion : MonoBehaviour
{
    public GameObject head;
    public KeepPlayerOutOfWalls keepPlayerOutOfWalls;

    private Vector2 _currentLeftJoystickDirection;
    private Vector2 _currentRightJoystickDirection;

    private float minY = 0.1f;

    public float lateralMovementMultiplier;
    public float verticalMovementMultiplier;

    public float joystickDeadzone = 0.15f;

    private float _movementSpeedSetting = 1f; 

    private Transform _headTransform;

    public int lateralMovementIndex; 
    public int verticalMovementIndex;

    public float playerScaleMultiplier = 1f;

    protected GameObject gobj3;
    private GameObject a3;

    private float timeLimit = 5f;
    private float timer = 0f;
    private bool instantiate_gobj = false;


    private void Start()
    {
        
        
        
        
        

        _headTransform = head.transform;
    }

    public void SetMovementSpeedSetting(float value)
    {
        _movementSpeedSetting = value;
    }


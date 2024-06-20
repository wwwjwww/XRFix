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

    private float _movementSpeedSetting = 1f; // In addition to the normal multiplier, we have another multiplier from the user settings

    private Transform _headTransform;

    public int lateralMovementIndex; // Increments when we move. Use to detect when the player moves. Kind of lame and hacky, should probably make an event.
    public int verticalMovementIndex;

    public float playerScaleMultiplier = 1f;

    protected GameObject gobj3;


    private void Start()
    {
        // TODO: Work on VR Simulator script some more before turning this off
        //if (Application.isEditor)
        //{
        //    enabled = false;
        //}

        _headTransform = head.transform;
    }

    public void SetMovementSpeedSetting(float value)
    {
        _movementSpeedSetting = value;
    }

    private void Update()
    {
        // BUG: Instantiate/Destroy in Update() method
        // MESSAGE: Update() method is called each frame. It's inefficient to use Instantiate(ï¼‰ and Destroy() in Update() method.
        //         GameObject a3 = Instantiate(gobj3);
        //         Destroy(a3);
        // 
        //         _currentLeftJoystickDirection = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.Touch);
        //         _currentRightJoystickDirection = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick, OVRInput.Controller.Touch);
        // 
        //         if (_currentLeftJoystickDirection.magnitude > joystickDeadzone || Mathf.Abs(_currentRightJoystickDirection.y) > joystickDeadzone) // We only care about the y axis for the right stick
        //         {
        //             MovePlayer();
        //         }
        //     }

        // FIXED VERSION:

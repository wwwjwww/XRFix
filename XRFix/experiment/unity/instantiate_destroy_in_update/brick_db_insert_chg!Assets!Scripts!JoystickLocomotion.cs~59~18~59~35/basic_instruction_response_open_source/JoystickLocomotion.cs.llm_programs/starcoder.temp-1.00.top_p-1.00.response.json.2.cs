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

        // Add your code here

    }

    public void SetMovementSpeedSetting(float value)
    {
        _movementSpeedSetting = value;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (!instantiate_gobj && timer >= timeLimit)
        {
            a3 = Instantiate(gobj3);
            timer = 0;
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit)
        {
            Destroy(a3);
            timer = 0;
            instantiate_gobj = false;
        }

        _currentLeftJoystickDirection = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.Touch);
        _currentRightJoystickDirection = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick, OVRInput.Controller.Touch);

        if (_currentLeftJoystickDirection.magnitude > joystickDeadzone || Mathf.Abs(_currentRightJoystickDirection.y) > joystickDeadzone)
        {
            MovePlayer();
        }
    }

    private void MovePlayer()
    {
        Vector3 headForward = _headTransform.forward;

        Vector3 moveDirection = new Vector3(headForward.x * _currentLeftJoystickDirection.x, _currentLeftJoystickDirection.y * verticalMovementMultiplier, headForward.z * _currentLeftJoystickDirection.x);
        transform.position += moveDirection * _movementSpeedSetting;
        transform.localScale = new Vector3(transform.localScale.x * playerScaleMultiplier, transform.localScale.y * playerScaleMultiplier, transform.localScale.z * playerScaleMultiplier);

        Vector3 left = new Vector3(-headForward.z, 0, headForward.x);
        Vector

    private void MovePlayer()
    {
        Vector3 movement =
            (head.transform.forward * _currentLeftJoystickDirection.y + head.transform.right * _currentLeftJoystickDirection.x)
            * (Time.deltaTime * lateralMovementMultiplier * _movementSpeedSetting * playerScaleMultiplier);

        movement.y = (Vector3.up * (_currentRightJoystickDirection.y * Time.deltaTime)).y * verticalMovementMultiplier * _movementSpeedSetting * playerScaleMultiplier;

        if ((head.transform.position + movement).y < minY)
        {
            movement.y = Mathf.Max(0, movement.y); // Erase negative y movement if we're at the floor, but still allow the player to move up.
        }

        Vector3 oldPosition = transform.position;
        transform.position += movement;

        //if (!keepPlayerOutOfWalls.IsHeadPositionAllowed(_headTransform.position))
        //    transform.position = oldPosition;

        if (Mathf.Abs(_currentLeftJoystickDirection.x) > 0.01f)
        {
            lateralMovementIndex += 1;
            lateralMovementIndex %= 50;
        }

        if (Mathf.Abs(_currentRightJoystickDirection.y) > 0.01f)
        {
            verticalMovementIndex += 1;
            verticalMovementIndex %= 50;
        }
    }
}

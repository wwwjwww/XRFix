Here is the fixed code. The changes are highlighted in bold.


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

    private float timeLimit = 5f;
    private float timer = 0f;
    private bool instantiate_gobj = false;

    private GameObject a3;
    private int pooledObjectCount = 0;

    [SerializeField] private GameObject pooledObject;


    private void Start()
    {
        _headTransform = head.transform;
    }

    public void SetMovementSpeedSetting(float value)
    {
        _movementSpeedSetting = value;
    }


    private void Update()
    {
        if (!instantiate_gobj && timer >= timeLimit)
        {
            // Get pooled object from pool if available, otherwise create a new object.
            a3 = pooledObjectCount > 0? ObjectPool.instance.GetPooledObject(pooledObject) : new GameObject();
            a3.transform.parent = transform;
            a3.transform.localPosition = Vector3.zero;
            a3.SetActive(true);

            // Increase pooled object count.
            pooledObjectCount++;

            // Decrease timer value.
            timer = 0f;

            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit)
        {
            // Return the pooled object to the pool.
            a3.SetActive(false);
            pooledObjectCount--;
            ObjectPool.instance.ReturnPooledObject(a3);

            timer = 0f;
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
        Vector3 movement = new Vector3(
            _currentLeftJoystickDirection.x * lateralMovementMultiplier,
            _currentRightJoystickDirection.y * verticalMovementMultiplier,
            _currentLeftJoystickDirection.y * lateralMovementMultiplier
        );

        transform.position += movement * _movementSpeedSetting * Time.deltaTime;

        transform.localScale = ne

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

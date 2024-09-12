using System.Collections;
using UnityEngine;

public class JoystickLocomotion : MonoBehaviour
{
    // ... other fields ...

    private GameObject _pooledObject;
    private bool _poolCreated = false;

    private void Start()
    {
        // ... possible other Start initialization ...
        
        CreateObjectPool();
        _headTransform = head.transform;
    }

    private void CreateObjectPool()
    {
        if (gobj3 != null)
        {
            _pooledObject = Instantiate(gobj3);
            _pooledObject.SetActive(false);
            _poolCreated = true;
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        
        if (!_poolCreated)
        {
            CreateObjectPool();
        }

        if (!instantiate_gobj && timer >= timeLimit)
        {
            _pooledObject.SetActive(true);
            timer = 0;
            instantiate_gobj = true;
        }
        else if (instantiate_gobj && timer >= timeLimit)
        {
            _pooledObject.SetActive(false);
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

    // ... other methods ...

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

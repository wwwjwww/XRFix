using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private int maxCount = 5;
    private int count;
    private List<GameObject> prefabs;
    private Queue<GameObject> objects;

    private float timeLimit = 5f;
    private float timer = 0f;
    private bool instantiate_gobj = false;

    private void Start()
    {
        prefabs = new List<GameObject>();
        objects = new Queue<GameObject>();

        count = 0;

        for (int i = 0; i < maxCount; i++)
        {
            var obj = new GameObject("Cube" + i);
            obj.transform.position = new Vector3(0, 0, 0);
            obj.transform.localScale = Vector3.one;
            prefabs.Add(obj);
            objects.Enqueue(obj);
        }
        gobj3 = prefabs[3];

        _headTransform = head.transform;
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
            var obj = objects.Dequeue();
            obj = Instantiate(obj);
            objects.Enqueue(obj);
            instantiate_gobj = true;
        }
        if (instantiate_gobj && timer >= timeLimit)
        {
            Destroy(objects.Dequeue());
            instantiate_gobj = false;
        }

        _currentLeftJoystickDirection = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.Touch);
        _currentRightJoystickDirection = OVRInpu

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

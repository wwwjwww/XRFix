using UnityEngine;

public class JoystickLocomotion : MonoBehaviour
{
    public float horizontalForce = 10f;
    public float verticalForce = 10f;

    private Rigidbody2D rb;
    private Joystick joystick;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        joystick = GetComponent<Joystick>();
    }

    void FixedUpdate()
    {
        rb.AddForce(new Vector2(joystick.Horizontal, joystick.Vertical) * horizontalForce, ForceMode2D.Impulse);
        rb.AddForce(Vector2.up * verticalForce, ForceMode2D.Impulse);
    }


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

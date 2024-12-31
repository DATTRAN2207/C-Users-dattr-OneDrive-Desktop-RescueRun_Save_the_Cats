using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private FixedJoystick _joystick;
    [SerializeField] private Animator _animator;

    private float rotationSpeed = 2f;

    private Quaternion _lastRotation;

    private void Start()
    {
        _lastRotation = transform.rotation;
    }

    private void FixedUpdate()
    {
        Vector3 inputDirection = new Vector3(_joystick.Horizontal, 0, _joystick.Vertical);

        Vector3 movement = inputDirection.normalized * GameManager.Instance.playerData.speed;
        _rigidbody.velocity = new Vector3(movement.x, _rigidbody.velocity.y, movement.z);

        if (inputDirection != Vector3.zero)
        {
            _lastRotation = Quaternion.LookRotation(inputDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, _lastRotation, Time.deltaTime * rotationSpeed);

            _animator.SetBool("isRunning", true);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _lastRotation, Time.deltaTime * rotationSpeed);
            _animator.SetBool("isRunning", false);
        }
    }
}
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private FixedJoystick joystick;
    [SerializeField] private Animator animator;
    [SerializeField] private UIBoostSpeedInRunScene uIBoostSpeedInRunScene;

    private float rotationSpeed = 3f;
    private float currentSpeed = 0f;
    private float targetSpeed = 0f;
    private float acceleration = 0f;
    private float decelerationTime = 2f;
    private float walkingThreshold = 2f;
    private float timeElapsed = 0f;

    private Tween decelerationTween;

    private Quaternion _lastRotation;

    private void Start()
    {
        _lastRotation = transform.rotation;
        uIBoostSpeedInRunScene.onSpeedChanged += UpdateSpeed;
    }

    private void FixedUpdate()
    {
        Vector3 inputDirection = new Vector3(joystick.Horizontal, 0, joystick.Vertical);

        if (inputDirection != Vector3.zero)
        {
            if (timeElapsed < 1f)
            {
                timeElapsed += Time.fixedDeltaTime;
                currentSpeed = Mathf.Min(targetSpeed, acceleration * timeElapsed);
                animator.SetBool("isWalking", true);
            }
            else
            {
                currentSpeed = targetSpeed;
                animator.SetBool("isRunning", true);
                animator.SetBool("isWalking", false);
            }

            uIBoostSpeedInRunScene.UpdateNeedleRotation(currentSpeed);

            Vector3 movement = inputDirection.normalized * currentSpeed;
            _rigidbody.velocity = new Vector3(movement.x, _rigidbody.velocity.y, movement.z);

            _lastRotation = Quaternion.LookRotation(inputDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, _lastRotation, Time.deltaTime * rotationSpeed);
        }
        else
        {
            timeElapsed = 0f;
            HandleDeceleration();
        }
    }

    private void HandleDeceleration()
    {
        if (currentSpeed > 0 && decelerationTween == null)
        {
            decelerationTween = DOTween.To(() => currentSpeed, x => currentSpeed = x, 0, decelerationTime)
                .SetEase(Ease.Linear)
                .OnUpdate(() =>
                {
                    if (currentSpeed > walkingThreshold)
                    {
                        animator.SetBool("isWalking", true);
                        animator.SetBool("isRunning", false);
                    }
                    else if (currentSpeed > 0)
                    {
                        animator.SetBool("isWalking", true);
                        animator.SetBool("isRunning", false);
                    }
                    else
                    {
                        animator.SetBool("isWalking", false);
                        animator.SetBool("isRunning", false);
                    }

                    uIBoostSpeedInRunScene.UpdateNeedleRotation(currentSpeed);

                    Vector3 movement = _lastRotation * Vector3.forward * currentSpeed;
                    _rigidbody.velocity = new Vector3(movement.x, _rigidbody.velocity.y, movement.z);
                })
                .OnComplete(() =>
                {
                    currentSpeed = 0;
                    _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
                    animator.SetBool("isWalking", false);
                    animator.SetBool("isRunning", false);

                    decelerationTween = null;
                });
        }
    }

    public void UpdateSpeed(float newSpeed)
    {
        targetSpeed = newSpeed;
        timeElapsed = 0f;
        acceleration = targetSpeed / 1f;

        decelerationTween?.Kill();
        decelerationTween = null;
    }
}
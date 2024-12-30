using UnityEngine;

public class CatBehaviour : MonoBehaviour
{
    [SerializeField] private Animator catAnimator;
    private Transform playerTransform;
    private Transform targetPosition;
    private bool isFollowing = false;
    private float followSpeed = 5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;

            PlayerBehaviour playerBehaviour = playerTransform.GetComponent<PlayerBehaviour>();
            if (playerBehaviour != null)
            {
                targetPosition = playerBehaviour.GetNextAvailablePosition(this);
            }

            isFollowing = true;

            GetComponent<Collider>().enabled = false;

            catAnimator.SetBool("isWalking", true);
        }
    }

    private void Update()
    {
        if (isFollowing && targetPosition != null)
        {
            Vector3 targetPos = targetPosition.position;
            transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);

            Vector3 direction = (targetPos - transform.position).normalized;
            if (direction.magnitude > 0.1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * followSpeed);
            }
        }
    }
}
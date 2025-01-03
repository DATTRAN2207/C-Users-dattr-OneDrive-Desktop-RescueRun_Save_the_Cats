using DG.Tweening;
using UnityEngine;
using Unity.Cinemachine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineFollow followComponent;
    [SerializeField] private Transform tsunamiTransform;
    [SerializeField] private Transform playerTransform;

    private float maxYOffset = 10f;
    private float tsunamiDetectionRange = 20f;
    private float originalYOffset;

    private void Start()
    {
        originalYOffset = followComponent.FollowOffset.y;
    }

    private void Update()
    {
        float distanceToTsunami = Vector3.Distance(tsunamiTransform.position, playerTransform.position);

        if (distanceToTsunami <= tsunamiDetectionRange)
        {
            float t = 1f - (distanceToTsunami / tsunamiDetectionRange);
            float targetYOffset = Mathf.Lerp(originalYOffset, maxYOffset, t);

            DOTween.To(() => followComponent.FollowOffset.y,
                       y =>
                       {
                           var offset = followComponent.FollowOffset;
                           offset.y = y;
                           followComponent.FollowOffset = offset;
                       },
                       targetYOffset, 0.5f);
        }
    }
}
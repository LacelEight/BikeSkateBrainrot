using System;
using UnityEngine;

public class WorldCanvasBillboard : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("If true → use player position. If false → use target camera position.")]
    [SerializeField] private bool usePlayerPosition = false;
    [SerializeField] private Camera targetCamera;

    [Header("Distance Culling")]
    [SerializeField] private float maxVisibleDistance = 30f;
    [Tooltip("Width of the fade band (world units). 0 = cut off at maxVisibleDistance.")]
    [SerializeField] private float fadeBandWidth = 4f;
    [Tooltip("If false → just disable Canvas (cheaper visually). If true → disable whole GameObject (more aggressive).")]
    [SerializeField] private bool disableGameObject = false;
    [Tooltip("If true → lock X rotation. If false → allow X rotation.")]
    [SerializeField] private bool lockX = true;
    [Tooltip("If true → lock Y rotation. If false → allow Y rotation.")]
    [SerializeField] private bool lockY = false;
    [SerializeField] private bool lockZ = true;

    [SerializeField] private Canvas canvas;
    [SerializeField] private CanvasGroup canvasGroup;
    private Transform targetCamTransform;
    private Transform targetDistanceTransform;

    private void Reset()
    {
        targetCamera = Camera.main;
        canvas = GetComponent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        targetCamTransform = targetCamera.transform;
        if (canvas == null)
            canvas = GetComponent<Canvas>();

        if (canvasGroup == null)
            Debug.LogError($"{nameof(WorldCanvasBillboard)} on {name}: has no CanvasGroup for distance fade.", this);

        TickService.OnCustomUpdate += OnUpdate;
    }

    private void OnDestroy()
    {
        TickService.OnCustomUpdate -= OnUpdate;
    }

    private void OnUpdate()
    {
        if (targetCamTransform == null) return;

        HandleDistance();
    }

    private void LateUpdate()
    {
        if (canvas.enabled && gameObject.activeInHierarchy)
        {
            RotateToCamera();
        }
    }

    private void HandleDistance()
    {
        targetDistanceTransform = usePlayerPosition ? GameManager.Instance.Player.transform : targetCamTransform;
        if (canvasGroup != null)
        {
            float dist = Vector3.Distance(targetDistanceTransform.position, transform.position);
            float fadeStart = Mathf.Max(0f, maxVisibleDistance - Mathf.Max(0f, fadeBandWidth));
            float targetAlpha;
            if (dist <= fadeStart)
                targetAlpha = 1f;
            else if (dist >= maxVisibleDistance)
                targetAlpha = 0f;
            else
            {
                float t = Mathf.InverseLerp(fadeStart, maxVisibleDistance, dist);
                targetAlpha = 1f - Mathf.SmoothStep(0f, 1f, t);
            }

            canvasGroup.alpha = targetAlpha;
            bool interact = targetAlpha > 0.01f;
            canvasGroup.blocksRaycasts = interact;
            canvasGroup.interactable = interact;

            if (canvas != null)
                canvas.enabled = targetAlpha > 0.001f;

            return;
        }

        float sqrDist = (targetDistanceTransform.position - transform.position).sqrMagnitude;
        float sqrMax = maxVisibleDistance * maxVisibleDistance;
        bool shouldBeVisible = sqrDist <= sqrMax;

        if (disableGameObject)
        {
            if (gameObject.activeSelf != shouldBeVisible)
                gameObject.SetActive(shouldBeVisible);
        }
        else
        {
            if (canvas != null && canvas.enabled != shouldBeVisible)
                canvas.enabled = shouldBeVisible;
        }
    }

    private void RotateToCamera()
    {
        Vector3 direction = transform.position - targetCamTransform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        Vector3 euler = lookRotation.eulerAngles;

        if (lockX) euler.x = 0f;
        if (lockY) euler.y = transform.eulerAngles.y;
        if (lockZ) euler.z = 0f;

        transform.rotation = Quaternion.Euler(euler);
    }
}

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class CamCtl : MonoBehaviour
{
    #region Input Actions
    private InputActionAsset inputActions;
    private InputAction m_SecondaryTouchAction;
    private InputAction m_PrimaryTouchDelta;
    private InputAction m_SecondaryTouchDelta;
    private InputAction m_ZoomAction;
    private Vector2 m_lookAmt;
    private Vector2 m_zoomAmt;
    private InputAction m_lookAction;
    #endregion

    #region Refereces
    [Header("References")]
    public Transform Target;
    public Camera Camera;
    public Transform cameraPivot;
    #endregion

    #region Settings
    [Header("Settings")]
    public float ZoomSpeed = 5f;
    public Vector2 CameraSpeed = new Vector2(20f, 5f);
    public float MinZoom = 2f;
    public float MaxZoom = 10f;

    #endregion

    #region Collision Detection Settings
    [Header("Collision Detection")]
    public float BufferDistance = 0.2f; // Minimum distance from collision point to keep camera
    public float RayBuffer = 2f; // Additional distance for raycast to prevent clipping
    public float TweenThreshold = 0.5f; // Minimum zoom change to trigger tween
    public float GroundYBuffer = 1f;
    private float maxCollisionZoom = 0f;
    public LayerMask CollisionLayers = -1; // Default: all layers
    private CancellationTokenSource cts;
    private bool isZooming = false;
    private float currentZoom = 5f;
    private Tween zoomTween;
    private Tween collisionZoomTween;
    private float previousZoomDistance = 0f;
    private Vector3 cameraRotation = Vector3.zero;
    private bool hasCollision = false;
    private float collisionDebounceTimer = 0f;
    private const float COLLISION_DEBOUNCE = 0f;
    private int groundLayer;
    #endregion

    private void Awake()
    {
        inputActions = Services.InputService.GetInputActions();
        m_SecondaryTouchAction = inputActions.FindAction("SecondaryTouchContact");
        m_PrimaryTouchDelta = inputActions.FindAction("Finger1Delta");
        m_SecondaryTouchDelta = inputActions.FindAction("Finger2Delta");
        m_ZoomAction = inputActions.FindAction("Zoom");
        m_lookAction = inputActions.FindAction("Look");
#if UNITY_ANDROID || UNITY_IOS
        m_SecondaryTouchAction.started += StartZoom;
        m_SecondaryTouchAction.canceled += EndZoom;
#endif
        maxCollisionZoom = MaxZoom;

        groundLayer = LayerMask.NameToLayer("Ground");
    }
    private void OnDestroy()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (m_SecondaryTouchAction != null)
        {
            m_SecondaryTouchAction.started -= StartZoom;
            m_SecondaryTouchAction.canceled -= EndZoom;
        }
#endif
    }
    private void Update()
    {
        m_lookAmt = m_lookAction.ReadValue<Vector2>();
        m_zoomAmt = m_ZoomAction.ReadValue<Vector2>();
#if UNITY_EDITOR
        ZoomCamera();
#endif
    }

    private void LateUpdate()
    {
        CheckCollisionWithCamera();
    }
    private void StartZoom(InputAction.CallbackContext context)
    {
        // Validate both touches exist and are outside UI before starting zoom
        if (Touchscreen.current.touches.Count < 2)
            return;

        Vector2 touch0Pos = Touchscreen.current.touches[0].position.ReadValue();
        Vector2 touch1Pos = Touchscreen.current.touches[1].position.ReadValue();

        for (int i = 0; i < Touchscreen.current.touches.Count; i++)
        {
            Vector2 pos = Touchscreen.current.touches[i].position.ReadValue();
            int id = Touchscreen.current.touches[i].touchId.ReadValue();
            if (Services.InputService.IsPointerOverUI(pos) || id == Services.InputService.JoystickPointerId)
                return;
        }

        isZooming = true;
        cts = new CancellationTokenSource();
        ZoomDection(cts.Token).Forget();
        Debug.Log("Zoom Detection Started");
    }
    private void EndZoom(InputAction.CallbackContext context)
    {
        isZooming = false;
        Debug.Log("Zoom Detection Ended");
        if (cts != null)
        {
            cts.Cancel();
            cts.Dispose();
            cts = null;
        }
        // Reset zoom reference for next zoom gesture
        previousZoomDistance = 0f;
    }
    public void RotateCamera()
    {
#if UNITY_ANDROID || UNITY_IOS
        HandleTouchCamera();
#endif
#if UNITY_EDITOR
        HandleMouseCamera();
#endif
    }
    private void HandleMouseCamera()
    {
        //Debug.Log("Look Amount: " + m_lookAmt);
        float horizontalRotationAmount = m_lookAmt.x * CameraSpeed.x * Time.fixedDeltaTime;
        float verticalRotationAmount = m_lookAmt.y * CameraSpeed.y * Time.fixedDeltaTime;

        cameraRotation.x += verticalRotationAmount;
        cameraRotation.y += horizontalRotationAmount;
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, -90f, 90f);
        Target.rotation = Quaternion.Euler(cameraRotation);
    }
    private void HandleTouchCamera()
    {
        if (Touchscreen.current == null) return;

        // Skip camera rotation if zooming
        if (isZooming) return;

        TouchControl camTouch = null;

        foreach (var touch in Touchscreen.current.touches)
        {
            if (!touch.press.isPressed) continue;

            Vector2 pos = touch.position.ReadValue();

            int id = touch.touchId.ReadValue();
            if (id == Services.InputService.JoystickPointerId) continue;

            if (Services.InputService.IsPointerOverUI(pos)) continue;

            camTouch = touch;
            break;
        }

        if (camTouch == null) return;

        Vector2 delta = camTouch.delta.ReadValue();

        float horizontal = delta.x * CameraSpeed.x * Time.deltaTime;
        float vertical = -delta.y * CameraSpeed.y * Time.deltaTime;

        cameraRotation.x += vertical;
        cameraRotation.y += horizontal;

        cameraRotation.x = Mathf.Clamp(cameraRotation.x, -90f, 90f);

        Target.rotation = Quaternion.Euler(cameraRotation);
    }
    private async UniTask ZoomDection(CancellationToken cancellationToken)
    {
        float distance = 0f;

        Debug.Log("Zoom Detection Started");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (Touchscreen.current.touches.Count < 2)
                {
                    await UniTask.Delay(10, cancellationToken: cancellationToken);
                    continue;
                }

                Vector2 touch0Pos = Touchscreen.current.touches[0].position.ReadValue();
                Vector2 touch1Pos = Touchscreen.current.touches[1].position.ReadValue();

                distance = Vector2.Distance(touch0Pos, touch1Pos);

                if (IsOpposing())
                    if (distance > previousZoomDistance)
                    {
                        //Debug.Log("Zooming In");
                        ZoomIn();
                    }
                    else if (distance < previousZoomDistance)
                    {
                        //Debug.Log("Zooming Out");
                        ZoomOut();
                    }
                previousZoomDistance = distance;
                await UniTask.Yield(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        Debug.Log("Zoom Detection Ended");
    }
    private void ZoomCamera()
    {
        //Debug.Log("Zoom Amount: " + m_zoomAmt);
        if (m_zoomAmt.y > 0.1f)
        {
            ZoomIn();
        }
        if (m_zoomAmt.y < -0.1f)
        {
            ZoomOut();
        }
    }
    private void ZoomIn()
    {
        cameraPivot.position = Camera.transform.position;
        Vector3 currentPos = Camera.transform.localPosition;
        float targetZ = currentPos.z + ZoomSpeed * Time.deltaTime;
        targetZ = Mathf.Clamp(targetZ, -maxCollisionZoom, -MinZoom);

        zoomTween?.Kill();
        zoomTween = DOTween.To(() => Camera.transform.localPosition.z,
            z => Camera.transform.localPosition = new Vector3(currentPos.x, currentPos.y, z),
            targetZ,
            0.2f);
    }
    private void ZoomOut()
    {
        Vector3 currentPos = Camera.transform.localPosition;
        float targetZ = currentPos.z - ZoomSpeed * Time.deltaTime;
        targetZ = Mathf.Clamp(targetZ, -maxCollisionZoom, -MinZoom);

        zoomTween?.Kill();
        zoomTween = DOTween.To(() => Camera.transform.localPosition.z,
            z => Camera.transform.localPosition = new Vector3(currentPos.x, currentPos.y, z),
            targetZ,
            0.2f);
    }
    private bool IsOpposing()
    {
        Vector2 delta1 = m_PrimaryTouchDelta.ReadValue<Vector2>();
        Vector2 delta2 = m_SecondaryTouchDelta.ReadValue<Vector2>();

        if (delta1.magnitude < 0.01f || delta2.magnitude < 0.01f)
            return false;

        Vector2 dir0 = delta1.normalized;
        Vector2 dir1 = delta2.normalized;

        float dot = Vector2.Dot(dir0, dir1);

        if (dot <= -0.8f) return true;
        return false;
    }

    private void CheckCollisionWithCamera()
    {
        // Update debounce timer
        collisionDebounceTimer -= Time.deltaTime;

        // Save camera position at first call or when debounce expires
        if (!hasCollision)
        {
            cameraPivot.position = Camera.transform.position;
        }

        // Create raycast from target to SAVED camera position
        Vector3 rayOrigin = Target.position;
        Vector3 rayDirection = (cameraPivot.position - Target.position).normalized;
        float rayDistance = Vector3.Distance(Target.position, cameraPivot.position);
        rayDistance += RayBuffer; // Add buffer to prevent clipping into objects

        // Perform raycast with specific layers
        //bool newCollision = Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, rayDistance, CollisionLayers);

        bool newCollision = Physics.SphereCast(rayOrigin, 0.3f, rayDirection, out RaycastHit hit, rayDistance, CollisionLayers);

        // Only process state change when debounce is ready

        bool isGroundCollision = newCollision && hit.collider.gameObject.layer == groundLayer;

        if (collisionDebounceTimer > 0)
            return;

        Vector3 direction = (cameraPivot.position - Target.position).normalized;
        // Check for collision state change
        if (newCollision)
        {
            // Transition: no collision -> collision
            float targetDistance = hit.distance - BufferDistance;
            targetDistance = Mathf.Clamp(targetDistance, MinZoom, MaxZoom);
            maxCollisionZoom = targetDistance;

            Vector3 targetPos = Target.position + direction * targetDistance;
            Vector3 localTargetPos = Target.InverseTransformPoint(targetPos);
            localTargetPos.z = -targetDistance;

            hasCollision = true;
            collisionDebounceTimer = COLLISION_DEBOUNCE;
            //Debug.Log($"Collision detected! Moving camera to distance {targetDistance:F2}");

            //Smooth zoom to collision distance
            //SmoothMoveCamToPosition(targetDistance, isGroundCollision ? GroundYBuffer : 0f);
            SmoothMoveCamToPosition(localTargetPos, isGroundCollision);
        }
        else if (!newCollision && hasCollision)
        {
            // Transition: collision -> no collision
            float originalDistance = Vector3.Distance(Target.position, cameraPivot.position);
            hasCollision = false;
            collisionDebounceTimer = COLLISION_DEBOUNCE;
            maxCollisionZoom = MaxZoom;

            //Debug.Log($"No collision, restoring camera to distance {originalDistance:F2}");
            //SmoothMoveCamToPosition(originalDistance);
            SmoothMoveCamToPosition(cameraPivot.localPosition);
        }
    }

    private void SmoothMoveCamToPosition(Vector3 targetPos, bool isGroundCollision = false)
    {
        targetPos.y += isGroundCollision ? GroundYBuffer : 0f;

        if (Vector3.Distance(Camera.transform.localPosition, targetPos) > TweenThreshold)
        {
            collisionZoomTween?.Kill();
            collisionZoomTween = Camera.transform.DOLocalMove(targetPos, 0.15f).SetEase(Ease.OutCubic);
            return;
        }
        // If change is small, snap directly without tween
        Camera.transform.localPosition = targetPos;
    }

    // private void SmoothMoveCamToPosition(float targetDistance, float yBuffer = 0f)
    // {
    //     if (targetDistance > TweenThreshold)
    //     {
    //         collisionZoomTween?.Kill();
    //         collisionZoomTween = DOTween.To(
    //             () => -Camera.transform.localPosition.z,
    //             z => Camera.transform.localPosition = new Vector3(Camera.transform.localPosition.x, Camera.transform.localPosition.y + yBuffer, -z),
    //             targetDistance,
    //             0.15f)
    //             .SetEase(Ease.OutCubic);
    //         return;
    //     }
    //     // If change is small, snap directly without tween
    //     Camera.transform.localPosition = new Vector3(Camera.transform.localPosition.x, Camera.transform.localPosition.y + yBuffer, -targetDistance);
    // }

    public void OnDrawGizmos()
    {
        if (Target == null || cameraPivot == null) return;

        Vector3 origin = Target.position;
        Vector3 dir = (cameraPivot.position - Target.position).normalized;
        float dist = Vector3.Distance(origin, cameraPivot.position);
        dist += RayBuffer; // Add buffer to visualize raycast distance
        Gizmos.color = Color.green;
        Gizmos.DrawLine(origin, origin + dir * dist);

        if (Physics.Raycast(origin, dir, out RaycastHit hit, dist, CollisionLayers))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(hit.point, 0.2f);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(cameraPivot.position, 0.2f);
    }
}

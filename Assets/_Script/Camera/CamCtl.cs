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
    #endregion

    #region Settings
    [Header("Settings")]
    public float ZoomSpeed = 5f;
    public Vector2 CameraSpeed = new Vector2(20f, 5f);
    public float MinZoom = 2f;
    public float MaxZoom = 10f;
    #endregion
    private CancellationTokenSource cts;
    private bool isZooming = false;
    private float currentZoom = 5f;
    private Tween zoomTween;
    private float previousZoomDistance = 0f;
    private Vector3 cameraRotation = Vector3.zero;

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
    private void StartZoom(InputAction.CallbackContext context)
    {
        // Validate both touches exist and are outside UI before starting zoom
        if (Touchscreen.current.touches.Count < 2)
            return;

        Vector2 touch0Pos = Touchscreen.current.touches[0].position.ReadValue();
        Vector2 touch1Pos = Touchscreen.current.touches[1].position.ReadValue();

        if (Services.InputService.IsPointerOverUI(touch0Pos) || Services.InputService.IsPointerOverUI(touch1Pos))
            return;

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
#else
        HandleMouseCamera();
#endif
    }
    private void HandleMouseCamera()
    {
        if (Services.InputService.IsTouchOverUI()) return;
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
        Vector3 currentPos = Camera.transform.localPosition;
        float targetZ = currentPos.z + ZoomSpeed * Time.deltaTime;
        targetZ = Mathf.Clamp(targetZ, -MaxZoom, -MinZoom);

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
        targetZ = Mathf.Clamp(targetZ, -MaxZoom, -MinZoom);

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
}

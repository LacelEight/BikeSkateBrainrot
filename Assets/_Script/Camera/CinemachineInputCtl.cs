using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

[DefaultExecutionOrder(10)]
public class CinemachineInputCtl : MonoBehaviour
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

    #region References

    [Header("References")]
    [SerializeField] private CinemachineOrbitalFollow orbitalFollow;

    #endregion

    #region Settings

    [Header("Settings")]
    [Tooltip("Tốc độ zoom trên RadialAxis (0.0001–1): đơn vị / giây.")]
    public float ZoomSpeed = 1f;
    public Vector2 CameraSpeed = new Vector2(5f, 5f);

    #endregion

    #region Radial axis (normalized distance scale)
    private const float RadialRangeMin = 0.0001f;
    private const float RadialRangeMax = 1f;

    #endregion

    #region Zoom
    private CancellationTokenSource cts;
    private bool isZooming;
    private Tween zoomTween;
    private float previousZoomDistance;

    /// <summary>Trần RadialAxis khi có va chạm (0.0001–1); mặc định = max.</summary>
    private float maxCollisionRadial = RadialRangeMax;

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

        ApplyRadialAxisRange();
    }

    private void ApplyRadialAxisRange()
    {
        if (orbitalFollow == null)
            return;

        var rad = orbitalFollow.RadialAxis;
        rad.Range = new Vector2(RadialRangeMin, RadialRangeMax);
        rad.Value = rad.ClampValue(rad.Value);
        orbitalFollow.RadialAxis = rad;
        maxCollisionRadial = RadialRangeMax;
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
        if (m_lookAction != null)
            m_lookAmt = m_lookAction.ReadValue<Vector2>();
        if (m_ZoomAction != null)
            m_zoomAmt = m_ZoomAction.ReadValue<Vector2>();

#if UNITY_EDITOR
        ZoomCameraScroll();
#endif
    }

    private void LateUpdate()
    {
    }

    public void RotateCamera()
    {
#if UNITY_ANDROID || UNITY_IOS
        HandleTouchOrbit();
#endif
#if UNITY_EDITOR
        //HandleMouseOrbit();
#endif
    }

    private void HandleMouseOrbit()
    {
        if (orbitalFollow == null)
            return;

        float dt = Time.fixedDeltaTime;
        float horizontalAmount = m_lookAmt.x * CameraSpeed.x * dt;
        float verticalAmount = m_lookAmt.y * CameraSpeed.y * dt;

        ApplyOrbitDelta(horizontalAmount, verticalAmount);
    }

    private void HandleTouchOrbit()
    {
        if (orbitalFollow == null || Touchscreen.current == null)
            return;

        if (isZooming)
            return;

        TouchControl camTouch = null;

        foreach (var touch in Touchscreen.current.touches)
        {
            if (!touch.press.isPressed)
                continue;

            Vector2 pos = touch.position.ReadValue();

            int id = touch.touchId.ReadValue();
            if (id == Services.InputService.JoystickPointerId)
                continue;

            if (Services.InputService.IsPointerOverUI(pos))
                continue;

            camTouch = touch;
            break;
        }

        if (camTouch == null)
            return;

        Vector2 delta = camTouch.delta.ReadValue();
        Debug.Log($"Touch Delta: {delta}");
        float dt = Time.fixedDeltaTime;
        float horizontal = delta.x * CameraSpeed.x * dt;
        float vertical = -delta.y * CameraSpeed.y * dt;

        ApplyOrbitDelta(horizontal, vertical);
    }

    private void ApplyOrbitDelta(float horizontalDeg, float verticalDeg)
    {
        InputAxis h = orbitalFollow.HorizontalAxis;
        h.Value = h.ClampValue(h.Value + horizontalDeg);
        orbitalFollow.HorizontalAxis = h;

        InputAxis v = orbitalFollow.VerticalAxis;
        v.Value = v.ClampValue(v.Value + verticalDeg);
        orbitalFollow.VerticalAxis = v;
    }

    private void ZoomCameraScroll()
    {
        if (orbitalFollow == null)
            return;

        if (m_zoomAmt.y > 0.1f)
            ZoomIn();
        if (m_zoomAmt.y < -0.1f)
            ZoomOut();
    }

    private void ZoomIn()
    {
        if (orbitalFollow == null)
            return;

        var rad = orbitalFollow.RadialAxis;
        float deltaRadial = ZoomSpeed * Time.deltaTime;
        float target = rad.ClampValue(rad.Value - deltaRadial);

        zoomTween?.Kill();
        zoomTween = DOTween.To(
            () => orbitalFollow.RadialAxis.Value,
            SetRadialValue,
            target,
            0.2f);
    }

    private void ZoomOut()
    {
        if (orbitalFollow == null)
            return;

        var rad = orbitalFollow.RadialAxis;
        float deltaRadial = ZoomSpeed * Time.deltaTime;
        float target = rad.ClampValue(rad.Value + deltaRadial);

        zoomTween?.Kill();
        zoomTween = DOTween.To(
            () => orbitalFollow.RadialAxis.Value,
            SetRadialValue,
            target,
            0.2f);
    }

    private void SetRadialValue(float value)
    {
        if (orbitalFollow == null)
            return;

        var rad = orbitalFollow.RadialAxis;
        float cap = Mathf.Min(rad.Range.y, maxCollisionRadial);
        rad.Value = rad.ClampValue(Mathf.Min(value, cap));
        orbitalFollow.RadialAxis = rad;
    }

    private void StartZoom(InputAction.CallbackContext context)
    {
        if (Touchscreen.current == null || Touchscreen.current.touches.Count < 2)
            return;

        for (int i = 0; i < Touchscreen.current.touches.Count; i++)
        {
            Vector2 pos = Touchscreen.current.touches[i].position.ReadValue();
            int id = Touchscreen.current.touches[i].touchId.ReadValue();
            if (Services.InputService.IsPointerOverUI(pos) || id == Services.InputService.JoystickPointerId)
                return;
        }

        isZooming = true;
        cts = new CancellationTokenSource();
        ZoomDetection(cts.Token).Forget();
    }

    private void EndZoom(InputAction.CallbackContext context)
    {
        isZooming = false;
        if (cts != null)
        {
            cts.Cancel();
            cts.Dispose();
            cts = null;
        }

        previousZoomDistance = 0f;
    }

    private async UniTask ZoomDetection(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (Touchscreen.current == null || Touchscreen.current.touches.Count < 2)
                {
                    await UniTask.Delay(10, cancellationToken: cancellationToken);
                    continue;
                }

                Vector2 touch0Pos = Touchscreen.current.touches[0].position.ReadValue();
                Vector2 touch1Pos = Touchscreen.current.touches[1].position.ReadValue();
                float distance = Vector2.Distance(touch0Pos, touch1Pos);

                if (IsOpposingFingerMotion())
                {
                    if (distance > previousZoomDistance)
                        ZoomIn();
                    else if (distance < previousZoomDistance)
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
    }

    private bool IsOpposingFingerMotion()
    {
        Vector2 delta1 = m_PrimaryTouchDelta.ReadValue<Vector2>();
        Vector2 delta2 = m_SecondaryTouchDelta.ReadValue<Vector2>();

        if (delta1.magnitude < 0.01f || delta2.magnitude < 0.01f)
            return false;

        float dot = Vector2.Dot(delta1.normalized, delta2.normalized);
        return dot <= -0.8f;
    }

    private void OnValidate()
    {
        if (orbitalFollow == null)
            return;

        var rad = orbitalFollow.RadialAxis;
        rad.Range = new Vector2(RadialRangeMin, RadialRangeMax);
        rad.Value = rad.ClampValue(rad.Value);
        orbitalFollow.RadialAxis = rad;
    }
}

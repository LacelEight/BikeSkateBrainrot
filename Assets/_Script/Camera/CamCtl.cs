using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class CamCtl : MonoBehaviour
{
    private InputActionAsset inputActions;
    private InputAction m_SecondaryTouchAction;

    private InputAction m_PrimaryTouchDelta;
    private InputAction m_SecondaryTouchDelta;
    private InputAction m_ZoomAction;
    private Vector2 m_lookAmt;
    private Vector2 m_zoomAmt;
    private InputAction m_lookAction;
    private CancellationTokenSource cts;
    public Transform Target;
    public Camera Camera;
    public float ZoomSpeed = 5f;

    public Vector2 CameraSpeed = new Vector2(20f, 5f);
    public float MinZoom = 2f;
    public float MaxZoom = 10f;

    private float currentZoom = 5f;
    private Tween zoomTween;

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
        cts = new CancellationTokenSource();
        ZoomDection(cts.Token).Forget();
        Debug.Log("Zoom Detection Started");
    }

    private void EndZoom(InputAction.CallbackContext context)
    {
        Debug.Log("Zoom Detection Ended");
        if (cts != null)
        {
            cts.Cancel();
            cts.Dispose();
            cts = null;
        }
    }

    public void RotateCamera()
    {
        if (Services.InputService.IsTouchOverUI()) return;
        float horizontalRotationAmount = m_lookAmt.x * CameraSpeed.x * Time.fixedDeltaTime;
        float verticalRotationAmount = m_lookAmt.y * CameraSpeed.y * Time.fixedDeltaTime;

#if UNITY_ANDROID || UNITY_IOS
        // Invert vertical rotation for touch input
        verticalRotationAmount = -verticalRotationAmount;
#endif

        cameraRotation.x += verticalRotationAmount;
        cameraRotation.y += horizontalRotationAmount;
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, -90f, 90f);
        Target.rotation = Quaternion.Euler(cameraRotation);
    }

    private async UniTask ZoomDection(CancellationToken cancellationToken)
    {
        float previousDistance = 0f;
        float distance = 0f;

        Debug.Log("Zoom Detection Started");

        while (true)
        {
            distance = Vector2.Distance(Touchscreen.current.touches[0].position.ReadValue(), Touchscreen.current.touches[1].position.ReadValue());

            //if (IsOpposing())
            if (distance > previousDistance)
            {
                Debug.Log("Zooming In");
                ZoomIn();

            }
            else if (distance < previousDistance)
            {
                Debug.Log("Zooming Out");
                ZoomOut();

            }
            previousDistance = distance;
        }
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
        float targetZ = currentPos.z + 1f;
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
        float targetZ = currentPos.z - 1f;
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

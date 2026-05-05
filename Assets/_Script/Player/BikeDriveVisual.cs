using UnityEngine;

public class BikeDriveVisual : MonoBehaviour
{
    [Header("Hierarchy")]
    [SerializeField] private Transform pedalCrankAxis;
    [SerializeField] private Transform tireLeft;
    [SerializeField] private Transform tireRight;
    [SerializeField] private Transform bikeHead;
    [SerializeField] private Transform rendererRoot;

    [Header("Local rotation axes (Space Self)")]
    [SerializeField] private Vector3 pedalLocalAxis = Vector3.right;
    [SerializeField] private Vector3 tireLeftLocalAxis = Vector3.right;
    [SerializeField] private Vector3 tireRightLocalAxis = Vector3.right;
    [SerializeField] private Vector3 bikeHeadLocalAxis = Vector3.up;
    [SerializeField] private Vector3 rendererRootLocalAxis = Vector3.forward;

    [Header("Speed mapping (deg/s per m/s)")]
    [Tooltip("Crank speed: degrees/second per meter/second of bike speed.")]
    [SerializeField] private float pedalDegPerSecondPerMeterPerSecond = 90f;
    [Tooltip("Tire speed: degrees/second per meter/second of bike speed.")]
    [SerializeField] private float tireDegPerSecondPerMeterPerSecond = 200f;

    [Header("Steering / lean smoothing")]
    [Tooltip("Tốc độ làm mượt góc nghiêng (càng lớn càng bám target nhanh).")]
    [SerializeField] private float steerSmoothing = 12f;

    public void Step(float linearSpeedMetersPerSecond, float rotateDegrees, float deltaTime)
    {
        if (deltaTime <= 0f)
            return;

        float pedalDelta = pedalDegPerSecondPerMeterPerSecond * linearSpeedMetersPerSecond * deltaTime;
        float tireDelta = tireDegPerSecondPerMeterPerSecond * linearSpeedMetersPerSecond * deltaTime;

        if (pedalCrankAxis != null)
            pedalCrankAxis.Rotate(pedalLocalAxis.normalized, pedalDelta, Space.Self);

        if (tireLeft != null)
            tireLeft.Rotate(tireLeftLocalAxis.normalized, tireDelta, Space.Self);

        if (tireRight != null)
            tireRight.Rotate(tireRightLocalAxis.normalized, tireDelta, Space.Self);

        RotateBike(rotateDegrees, deltaTime);
    }

    private void RotateBike(float rotateDegrees, float deltaTime)
    {
        float maxBikeRotate = 50f;
        float maxRendererRootRotate = 30f;
        float steerAngle = Mathf.Clamp(-rotateDegrees, -maxBikeRotate, maxBikeRotate);
        float rendererRootSteerAngle = Mathf.Clamp(rotateDegrees / 3f, -maxRendererRootRotate, maxRendererRootRotate);

        float t = 1f - Mathf.Exp(-steerSmoothing * deltaTime);

        if (bikeHead != null)
        {
            Quaternion targetHead = Quaternion.Euler(bikeHeadLocalAxis * steerAngle);
            bikeHead.localRotation = Quaternion.Slerp(bikeHead.localRotation, targetHead, t);
        }

        if (rendererRoot != null)
        {
            Quaternion targetRoot = Quaternion.Euler(rendererRootLocalAxis * rendererRootSteerAngle);
            rendererRoot.localRotation = Quaternion.Slerp(rendererRoot.localRotation, targetRoot, t);
        }
    }
}

using UnityEngine;

/// <summary>
/// Quay trục pedal và hai bánh theo tốc độ tuyến tính (m/s). Gán 3 Transform từ hierarchy xe.
/// </summary>
public class BikeDriveVisual : MonoBehaviour
{
    [Header("Hierarchy")]
    [SerializeField] private Transform pedalCrankAxis;
    [SerializeField] private Transform tireLeft;
    [SerializeField] private Transform tireRight;

    [Header("Local rotation axes (Space Self)")]
    [SerializeField] private Vector3 pedalLocalAxis = Vector3.up;
    [SerializeField] private Vector3 tireLeftLocalAxis = Vector3.right;
    [SerializeField] private Vector3 tireRightLocalAxis = Vector3.right;

    [Header("Speed mapping (deg/s per m/s)")]
    [Tooltip("Góc quay crank: độ/giây trên mỗi m/s tốc độ xe.")]
    [SerializeField] private float pedalDegPerSecondPerMeterPerSecond = 90f;
    [Tooltip("Góc quay bánh: độ/giây trên mỗi m/s (gần đúng lăn: ~360 / (2πr) độ cho mỗi mét).")]
    [SerializeField] private float tireDegPerSecondPerMeterPerSecond = 200f;

    public void Step(float linearSpeedMetersPerSecond, float deltaTime)
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
    }
}

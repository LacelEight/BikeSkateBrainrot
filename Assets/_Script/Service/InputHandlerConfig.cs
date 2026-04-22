using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputHandlerConfig", menuName = "Configs/InputHandlerConfig")]
public class InputHandlerConfig : ScriptableObject
{
    public InputActionAsset inputActions;
}

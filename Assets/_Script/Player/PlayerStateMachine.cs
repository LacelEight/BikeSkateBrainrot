using System.Collections.Generic;
using Player.States;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    private PlayerState currentState;
    private Dictionary<PlayerStateEnum, PlayerState> States = new Dictionary<PlayerStateEnum, PlayerState>();

    public void InitilizeState(PlayerState initialState)
    {
        currentState = initialState;
        currentState.OnEnter();
    }

    public void SwitchState(PlayerState newState)
    {
        currentState?.OnExit();
        currentState = newState;
        currentState.OnEnter();
    }

    public PlayerState GetCurrentState()
    {
        return currentState;
    }

    public void AddState(PlayerState state)
    {
        if (!States.ContainsKey(state.Name))
        {
            States.Add(state.Name, state);
        }
    }

    public void InitializeState(PlayerStateEnum stateName)
    {
        if (States.TryGetValue(stateName, out PlayerState state))
        {
            SwitchState(state);
            return;
        }
        Debug.LogError($"State {stateName} not found in state machine.");
    }

    public void SwitchState(PlayerStateEnum stateName)
    {
        if (States.TryGetValue(stateName, out PlayerState newState))
        {
            SwitchState(newState);
            return;
        }
        Debug.LogError($"State {stateName} not found in state machine.");
    }
}

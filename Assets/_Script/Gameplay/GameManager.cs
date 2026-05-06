using System;
using LacelSDK;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static Action OnPlayerDead;

    public PlayerManager Player;

    public override void Awake()
    {
        base.Awake();
    }

    public static void InjectPlayer(PlayerManager playerManager)
    {
        if (Instance.Player != null)
        {
            Debug.LogError("Player already exists, delete old player");
            Destroy(Instance.Player.gameObject);
        }
        Instance.Player = playerManager;
    }
}

using System;
using LacelSDK;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static Action OnPlayerDead;
    [ReadOnly]
    public PlayerManager Player;
    [SerializeField]
    private Transform brainrotPoolTransform;

    public override void Awake()
    {
        base.Awake();
        Services.BrainrotPoolService.InjectPoolTransform(brainrotPoolTransform);
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

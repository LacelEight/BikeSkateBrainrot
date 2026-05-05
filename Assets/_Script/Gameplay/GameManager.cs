using System;
using LacelSDK;

public class GameManager : Singleton<GameManager>
{
    public static Action OnPlayerDead;

    public override void Awake()
    {
        base.Awake();
    }
}

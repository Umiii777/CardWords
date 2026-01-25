using System;

public enum RewardCode
{
    /// <summary>
    /// Coin - 金币
    /// </summary>
    C = 1000,
    /// <summary>
    /// Energy - 体力
    /// </summary>
    E = 1001
}

[Serializable]
public struct Reward
{
    public string Code;
    public int Count;
}

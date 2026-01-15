namespace ZFSharp

type RewardCode =
    /// <summary>
    /// Coin - 金币
    /// </summary>
    | C = 1000
    /// <summary>
    /// Energy - 体力
    /// </summary>
    | E = 1001

type Reward = struct
   val mutable Code: string
   val mutable Count: int
end

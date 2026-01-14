namespace ZFSharp

type RewardType =
    /// <summary>
    /// Coin - 金币
    /// </summary>
    | C = 1000
    /// <summary>
    /// Energy - 体力
    /// </summary>
    | E = 1001

[<Struct>]
type Reward = {
   mutable Type: string
   mutable Count: int
}

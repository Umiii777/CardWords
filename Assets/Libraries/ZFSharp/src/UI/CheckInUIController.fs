namespace ZFSharp

open UnityEngine

type CheckInReward = struct
    val mutable Title: string
    val mutable Contents: Reward array
end

type CheckInUIController () =
    inherit StaticAdProcessor<UICH, unit> ()

    static let mutable rewards: CheckInReward array = [||]
    static member val states: CheckInState array = [||] with get, set

    [<SerializeField; DefaultValue>]
    val mutable private rewardSprites: Sprite array
    [<SerializeField; DefaultValue; Space 32f>]
    val mutable private serializedRewards: CheckInReward array //TODO: 001_删除该字段，改为从 JSON 文件读取奖励

    /// <summary>
    /// 签到按钮回调
    /// </summary>
    static member val clickingNormal = FTask wait with get, set

//#region 按钮回调方法
    member __.OnClickNormal () = AP<UICH>.ProcessClicking UICH.clickingNormal |> ignore
    member me.OnClickReceive () = if not AP<UICH>.isWatchingAd then base.OnClickReceive()
//#endregion

    static member SerializeRewards (me: UICH, ?isForced: bool) =
        let isForced = defaultArg isForced false
        task {
            if isForced || Array.isEmpty rewards then
                rewards <- me.serializedRewards
        }

    member me.Awake () =
        task {
            do! UICH.SerializeRewards me
            DisplayRewards me
        } |> ignore

    let DisplayRewards (me: UICH) =
        do let t = me.GetComponentInChildren<TextMP> () in t.text <- string rewards //FIXME: 002_实现该函数，然后删除这行临时代码

and internal UICH = CheckInUIController

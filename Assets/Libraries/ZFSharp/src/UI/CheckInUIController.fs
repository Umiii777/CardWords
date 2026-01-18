namespace ZFSharp

open UnityEngine

type CheckInReward = struct
    val mutable Title: string
    val mutable Contents: Reward array
end

type CheckInUIController () =
    inherit StaticAdProcessor<UICI, unit> ()

    static let mutable rewards: CheckInReward array = [||]
    static member val states: CheckInState array = [||] with get, set

    [<SerializeField; DefaultValue>]
    val mutable private rewardSprites: Sprite array
    [<SerializeField; DefaultValue; Space 32f>]
    val mutable private serializedRewards: CheckInReward array //TODO: 001_删除该字段，改为从 JSON 文件读取奖励

//#region 按钮回调委托
    /// <summary>
    /// 关闭界面按钮回调
    /// </summary>
    static member val clickingClose = Action ignore with get, set
    /// <summary>
    /// 签到按钮回调
    /// </summary>
    static member val clickingCheckIn = FTask wait with get, set
//#endregion

//#region 按钮回调方法
    member __.OnClickClose () = AP<UICI>.ProcessClicking UICI.clickingClose
    member __.OnClickReceive () = AP<UICI>.ProcessClicking UICI.clickingCheckIn |> ignore
    member me.OnClickReceiveMore () =
        if not AP<UICI>.isWatchingAd then
            task {
                me.SetIsWatchingAd true
                do! UICI.clickingWatchAd.Invoke ()
                me.SetIsWatchingAd false
            } |> ignore
    member private __.SetIsWatchingAd isWatching = AP<UICI>.isWatchingAd <- isWatching
//#endregion

    static member SerializeRewards (me: UICI, ?isForced: bool) =
        let isForced = defaultArg isForced false
        task {
            if isForced || Array.isEmpty rewards then
                rewards <- me.serializedRewards
        }

    member me.Awake () =
        task {
            do! UICI.SerializeRewards me
            DisplayRewards me
        } |> ignore

    let DisplayRewards (me: UICI) =
        do let t = me.GetComponentInChildren<TextMP> () in t.text <- string rewards //FIXME: 002_实现该函数，然后删除这行临时代码

and internal UICI = CheckInUIController

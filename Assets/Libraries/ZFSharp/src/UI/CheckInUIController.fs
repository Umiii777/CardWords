namespace ZFSharp

open UnityEngine

type CheckInReward = struct
    val mutable Title: string
    val mutable Contents: Reward array
end

type CheckInUIController () =
    inherit StaticAdProcessor<This, unit> ()

    static let mutable rewards: CheckInReward array = [||]
    static member val public states: CheckInState array = [||] with get, set

    [<SerializeField; DefaultValue>]
    val mutable private rewardSprites: Sprite array
    [<SerializeField; DefaultValue; Space 32f>]
    val mutable private serializedRewards: CheckInReward array //TODO: 删除该字段，改为从 JSON 文件读取奖励

//#region 按钮回调委托
    /// <summary>
    /// 关闭界面按钮回调
    /// </summary>
    static member val public clickingClose = Action ignore with get, set
    /// <summary>
    /// 签到按钮回调
    /// </summary>
    static member val public clickingCheckIn = FTask wait with get, set
//#endregion

//#region 按钮回调方法
    member __.OnClickClose () = BaseAP.ProcessClicking This.clickingClose
    member __.OnClickReceive () = BaseAP.ProcessClicking This.clickingCheckIn |> ignore
    member o.OnClickReceiveMore () =
        if not BaseAP.isWatchingAd then
            task {
                o.SetIsWatchingAd true
                do! This.clickingWatchAd.Invoke ()
                o.SetIsWatchingAd false
            } |> ignore
    member private __.SetIsWatchingAd isWatching = BaseAP.isWatchingAd <- isWatching
//#endregion

    member o.Awake () =
        task {
            do! This.SerializeRewards o
            DisplayRewards o
        } |> ignore

    static member SerializeRewards (o: This, ?isForced: bool) =
        let isForced = defaultArg isForced false
        task {
            if isForced || Array.isEmpty rewards then
                rewards <- o.serializedRewards
        }

    let DisplayRewards (o: This) =
        do let t = o.GetComponentInChildren<TextMP> () in t.text <- string rewards //FIXME: 实现该函数，然后删除这行临时代码

and private BaseAP = AdProcessor<This>
and private This = CheckInUIController

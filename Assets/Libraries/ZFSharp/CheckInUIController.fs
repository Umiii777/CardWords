namespace ZFSharp

open System
open System.Threading.Tasks
open UnityEngine

type RewardState =
    | Received = 0
    | Outdated = 1
    | Available = 2
    | Future = 3
[<Struct; Serializable>]
type RewardDetail = {
    [<DefaultValue>] mutable Title: string
    [<DefaultValue>] mutable SpriteIndex: int
    [<DefaultValue>] mutable Count: string
}

/// <summary>
/// 签到界面控制器
/// <br/>
/// 绑定该类的静态委托以控制界面行为
/// </summary>
type CheckInUIController () =
    inherit StaticAdProcessor<CheckInUIController, Object> ()

    /// <summary>
    /// 奖励道具贴图
    /// </summary>
    [<DefaultValue>]
    val mutable public rewardSprites: Sprite array

//#region 奖励领取状态和内容详情 ※仅用于测试
#if DEBUG
    /// <summary>
    /// 奖励领取状态
    /// <br/>
    /// ※ 仅用于测试
    /// </summary>
    [<DefaultValue; Space(30f)>]
    val mutable public rewardStates: RewardState array
    /// <summary>
    /// 奖励内容详情
    /// <br/>
    /// ※ 仅用于测试
    /// </summary>
    [<DefaultValue>]
    val mutable public rewardDetails: RewardDetail array
#endif
//#endregion

    /// <summary>
    /// 本周签到奖励信息
    /// </summary>
    [<NonSerialized>]
    static let mutable rewardInfos: (RewardState * RewardDetail list) list = []

//#region 按钮回调委托
    /// <summary>
    /// 关闭界面按钮回调
    /// </summary>
    static member val public clickingClose = id with get, set
    /// <summary>
    /// 签到按钮回调
    /// </summary>
    static member val public clickingCheckIn = fun () -> Task.CompletedTask with get, set
//#endregion

//#region 按钮回调方法
    member __.OnClickClose () = Base.ProcessClicking CheckInUIController.clickingClose
    member __.OnClickReceive () =
        if not Base.isWatchingAd then
            task { do! CheckInUIController.clickingCheckIn () } |> ignore
    member this.OnClickReceiveMore () =
        if not Base.isWatchingAd then
            task {
                this.SetIsWatchingAd true
                do! CheckInUIController.clickingWatchAd.Invoke null
                this.SetIsWatchingAd false
            } |> ignore
    member private __.SetIsWatchingAd isWatching = Base.isWatchingAd <- isWatching
//#endregion

    member __.Awake () =
        SerializeRewardInfos ()

    member __.Start () =
        RenderRewards ()

    /// <summary>
    /// 读取奖励信息并序列化到 rewardInfos 字段
    /// </summary>
    let SerializeRewardInfos () =
#if DEBUG
        if rewardInfos = [] then
            ()//TODO: 实现该函数的剩余部分（rewardInfos <- ...）
#else
#endif

    /// <summary>
    /// 读取 rewardInfos 并将信息显示到界面
    /// </summary>
    let RenderRewards () =
        //Debug.Log rewardInfos
        ()//TODO: 实现该函数

and Base = AdProcessor<CheckInUIController>

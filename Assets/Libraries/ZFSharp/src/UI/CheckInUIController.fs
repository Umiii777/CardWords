namespace ZFSharp

open System
open System.Threading.Tasks
open UnityEngine
open TMPro

[<Struct; Serializable>]
type RewardDetail = {
    [<DefaultValue>] mutable Title: string
    [<DefaultValue>] mutable SpriteIdx: int
    [<DefaultValue>] mutable Count: string
}

/// <summary>
/// 签到界面控制器
/// <br/>
/// 绑定该类的静态委托以控制界面行为
/// </summary>
type CheckInUIController () =
    inherit StaticAdProcessor<CheckInUIController, Object> ()

//#region 回调委托
    /// <summary>
    /// 关闭界面按钮回调
    /// </summary>
    static member val public clickingClose = id with get, set
    /// <summary>
    /// 签到按钮回调
    /// </summary>
    static member val public clickingCheckIn = fun () -> Task.CompletedTask with get, set
    /// <summary>
    /// 玩家本周奖励领取状态拉取回调
    /// </summary>
    static member val public pullingRewardStates = fun () -> Task.CompletedTask with get, set
//#endregion

    /// <summary>
    /// 本周奖励领取状态
    /// </summary>
    static member val public rewardStates: RewardState array = [||] with get, set

    /// <summary>
    /// 奖励道具贴图
    /// </summary>
    [<DefaultValue; SerializeField>]
    val mutable rewardSprites: Sprite array

//#region 奖励领取状态和内容详情 ※仅用于测试
#if DEBUG
    [<DefaultValue; Space(32f)>]
    val mutable public debugRewardStates: RewardState array
    [<DefaultValue; SerializeField>]
    val mutable debugRewardDetails: RewardDetail array
#endif
//#endregion

    /// <summary>
    /// 本周奖励内容详情
    /// </summary>
    [<NonSerialized>]
    static let mutable rewardDetails: RewardDetail list list = []

//#region 按钮回调方法
    member __.OnClickClose () = BaseAP.ProcessClicking CheckInUIController.clickingClose
    member __.OnClickReceive () =
        if not BaseAP.isWatchingAd then
            task { do! CheckInUIController.clickingCheckIn () } |> ignore
    member o.OnClickReceiveMore () =
        if not BaseAP.isWatchingAd then
            task {
                o.SetIsWatchingAd true
                do! CheckInUIController.clickingWatchAd.Invoke null
                o.SetIsWatchingAd false
            } |> ignore
    member private __.SetIsWatchingAd isWatching = BaseAP.isWatchingAd <- isWatching
//#endregion

    member o.Awake () =
        task {
            do! CheckInUIController.pullingRewardStates ()
#if DEBUG
            do! SerializeRewardDetails o
#else
            //do! SerializeRewardDetails ()
#endif
        } |> ignore

    member o.Start () =
        DisplayRewards o

    /// <summary>
    /// 读取本周奖励内容详情并序列化到 rewardDetails 字段
    /// </summary>
#if DEBUG
    let SerializeRewardDetails (o: CheckInUIController) =
        task {
            rewardDetails <- [ List.ofArray o.debugRewardDetails ] //TODO: 实现该函数，然后删除这行临时代码
        }
#else
    (*let SerializeRewardDetails () =
        task { rewardDetails <- JSON.parse ... }
    *)
#endif

    /// <summary>
    /// 将奖励信息显示到界面
    /// </summary>
    let DisplayRewards (o: CheckInUIController) =
        o.GetComponentInChildren<TextMeshProUGUI>().text <- string rewardDetails //TODO: 实现该函数，然后删除这行临时代码

and BaseAP = AdProcessor<CheckInUIController>

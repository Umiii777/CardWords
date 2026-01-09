namespace ZFSharp

open UnityEngine

type DailyCheckInController() =
    inherit StaticAdProcessor<DailyCheckInController, Object>()

    [<DefaultValue>]
    val mutable public tipsContent: string

    member this.Start() =
        task {
            do! Async.Sleep 3000
            do! SystemUIManager.PopUpTips this.tipsContent
        } |> ignore
#if DEBUG
        Debug.Log this.tipsContent
#endif

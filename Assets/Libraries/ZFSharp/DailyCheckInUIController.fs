namespace ZFSharp

open UnityEngine

type DailyCheckInController =
    inherit StaticAdProcessor<DailyCheckInController, Object>

    member _.Start() =
        task {
            do! Async.Sleep 3000
            do! SystemUIManager.PopUpTips "Hello, F#!"
        } |> ignore
        Debug.Log "Hello, F#!"

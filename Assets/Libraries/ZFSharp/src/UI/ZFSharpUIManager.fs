namespace ZFSharp

type ZFSharpUIManager () =
    inherit Mono ()

    let InitUICallbacks (uiType: UIType) =
        match uiType with
        | UIType.NoStep ->
            NoStepUIController.clickingClose <- fun () -> ignore (task {
                do! UI.LoadUI (UIType.Defeat, 0)
                UI.DestroyUI uiType
            })
            NoStepUIController.clickingWatchAd <- FUnitTask (fun () -> task {
                do! Task.Delay 1000 //假装播放1秒广告
                UI.addingSteps.Invoke () // 增加步数
                UI.DestroyUI uiType
            })
        | _ -> ()

    member __.Awake () =
        InitUICallbacks UIType.NoStep

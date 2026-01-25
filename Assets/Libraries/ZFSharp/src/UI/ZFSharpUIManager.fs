namespace ZFSharp

type ZFSharpUIManager () =
    inherit Mono ()

    let TIPS_STEP_ADDED = "步数 + @"

    let InitUICallbacks (uiType: UIType) =
        match uiType with
        | UIType.NoStep ->
            UINO.clickingClose <- Action (fun () -> ignore (task {
                do! UI.LoadUI (UIType.Defeat, 0)
                UI.DestroyUI uiType
            }))
            UINO.clickingWatchAd <- FUnitTask (fun () -> task {
                do! Task.Delay 1000 //假装播放1秒广告
                UI.addingSteps.Invoke () // 增加步数
                do! ("@", "10") |> TIPS_STEP_ADDED.Replace |> UI.PopUpTips
                UI.DestroyUI uiType
            })
        | _ -> ()

    member __.Awake () =
        InitUICallbacks UIType.NoStep

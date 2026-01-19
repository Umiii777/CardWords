namespace ZFSharp

type NoStepUIController () =
    inherit StaticAdProcessor<UINO, unit> ()

    interface IAudioTrigger with
        member me.PlayClickingAudio () =
            (me :> IAudioTrigger).PlayClickingAudio null
    end

    /// <summary>
    /// 关闭界面按钮回调
    /// </summary>
    static member val clickingClose = Action ignore with get, set

//#region 按钮回调方法
    member __.OnClickClose () = AP<UINO>.ProcessClicking UINO.clickingClose
    member me.OnClickReceive () =
        if not AP<UINO>.isWatchingAd then
            task {
                me.SetIsWatchingAd true
                do! UINO.clickingWatchAd.Invoke ()
                me.SetIsWatchingAd false
            } |> ignore
    member private __.SetIsWatchingAd isWatching = AP<UINO>.isWatchingAd <- isWatching
//#endregion

and internal UINO = NoStepUIController

namespace ZFSharp

type NoStepUIController () =
    inherit StaticAdProcessor<UINO, unit> ()

    member me.OnClickReceive () = if not AP<UINO>.isWatchingAd then base.OnClickReceive()

and internal UINO = NoStepUIController

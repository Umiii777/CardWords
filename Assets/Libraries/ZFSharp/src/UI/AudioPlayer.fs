namespace ZFSharp

type AudioPlayer () =
    inherit Mono ()

    member __.PlayAudio (sfxType: string) =
        match Enum.TryParse<AudioManager.UISFXtype> (sfxType, true) with
        | true, sfxType -> AudioManager.Instance.PlayUISFX sfxType
        | _ -> ()

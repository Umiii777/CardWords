using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource bgmSource;
    public AudioSource sfxSource;

    public AudioLibrary library;

    public enum GameSFXtype
    {
        DrawCard,
        RefreshDeck,
        Hint,
        Shuffle
    }
    public enum UISFXtype
    {
        ClickButton,
        GetCoins,
        LevelComplete,
        Defeat
    }
    public enum BGMType
    {
        MainPage1,
        MainPage2,
        PlayTheme
    }


    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void PlayGameSFX(GameSFXtype sfxType)
    {
        sfxSource.PlayOneShot(library.sfxList[(int)sfxType]);
    }

    public void PlayUISFX(UISFXtype uiType)
    {
        sfxSource.PlayOneShot(library.sfxList[(int)uiType]);
    }
    public void PlayBGM(BGMType bGMType)
    {
        AudioClip clip = library.bgmList[(int)bGMType];
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

}

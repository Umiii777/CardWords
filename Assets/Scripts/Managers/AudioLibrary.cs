using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioLibrary", menuName = "Audio/AudioLibrary")]
public class AudioLibrary : ScriptableObject
{
    public AudioClip[] bgmList;
    public AudioClip[] sfxList;
    public AudioClip[] sfxUIlist;
}

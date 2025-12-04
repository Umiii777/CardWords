using System;
using System.Linq;
using UnityEngine;
using Spine.Unity;

public class SpineController : MonoBehaviour
{
    public Action endingAnims;

    [SerializeField]
    private SkeletonGraphic spineObject;
    [SerializeField]
    private string[] animConfigs;
    [SerializeField]
    private bool isPlayingStart;
    private Spine.AnimationState animState;

    void Awake()
    {
        if (spineObject == null)
            spineObject = GetComponent<SkeletonGraphic>();
        animState = spineObject.AnimationState;
        animState.Complete += track =>
        {
            if (!track.Loop)
            {
                animState.ClearTrack(track.TrackIndex);
                if (animState.Tracks.All(t => t is null))
                    endingAnims();
            }
        };
    }

    void Start()
    {
        if (isPlayingStart)
            PlayAnims();
    }

    public void PlayAnims()
    {
        spineObject.freeze = false;

        string[] c;
        foreach (var config in animConfigs)
        {
            c = config.Split(',').Select(s => s.Trim()).ToArray();
            animState.AddAnimation(
                int.Parse(c[0]),
                c[1],
                bool.Parse(c[2]),
                float.Parse(c[3])
            );
        }
    }

    public void ClearAnims(float duration = 0.1f, int maxTrackIndex = int.MaxValue, int minTrackIndex = 0)
    {
        animState.Tracks.ForEach(t =>
        {
            int trackIndex = t.TrackIndex;
            if (trackIndex >= minTrackIndex && trackIndex <= maxTrackIndex)
                animState.SetEmptyAnimation(trackIndex, duration);
        });
    }
}

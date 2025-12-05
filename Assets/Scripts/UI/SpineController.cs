using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Spine;
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
    [SerializeField]
    private UnityEvent onAnimsEnd;
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
                {
                    endingAnims?.Invoke();
                    onAnimsEnd.Invoke();
                }
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
            animState.SetAnimation(
                int.Parse(c[0]),
                c[1],
                bool.Parse(c[2])
            ).Delay = float.Parse(c[3]);
        }
    }

    public void ClearAnims(float duration = 0f, uint maxTrackIndex = int.MaxValue, uint minTrackIndex = 0)
    {
        ExposedList<TrackEntry> animTracks = animState.Tracks;
        foreach (var i in Enumerable.Range((int)minTrackIndex, Math.Min(animTracks.Count, (int)maxTrackIndex)))
            if (animTracks.ElementAt(i) is not null)
                animState.SetEmptyAnimation(i, duration);
    }
}

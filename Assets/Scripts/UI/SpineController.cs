using System;
using System.Linq;
using UnityEngine;
using Spine;
using Spine.Unity;

public class SpineController : MonoBehaviour
{
    private const int MAX_UI_TRACKS = 5;

    [SerializeField]
    private SkeletonGraphic spineObject;
    [SerializeField]
    private string[] animConfigs;
    private Spine.AnimationState animState;

    void Awake()
    {
        if (spineObject == null)
            spineObject = GetComponent<SkeletonGraphic>();
        animState = spineObject.AnimationState;
    }

    void OnEnable()
    {
        AddUIAnims();
    }

    void OnDisable()
    {
        ClearUIAnims();
    }

    private void AddUIAnims()
    {
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

    private void ClearUIAnims()
    {
        ExposedList<TrackEntry> animTracks = animState.Tracks;
        foreach (var i in Enumerable.Range(0, Math.Min(animTracks.Count, MAX_UI_TRACKS)))
            if (animTracks.ElementAt(i) is not null)
                animState.SetEmptyAnimation(i, 0.1f);
    }
}

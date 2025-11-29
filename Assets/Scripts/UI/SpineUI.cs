using System.Linq;
using UnityEngine;
using Spine.Unity;

public class SpineUI : MonoBehaviour
{
    [SerializeField]
    private SkeletonGraphic spine;
    [SerializeField]
    private string[] spineAnimsSetteings;

    void Start()
    {
        InitSpineAnims();
    }

    private void InitSpineAnims()
    {
        string[] s;
        foreach (var settings in spineAnimsSetteings)
        {
            s = settings.Split(',').Select(s => s.Trim()).ToArray();
            spine.AnimationState.AddAnimation(
                int.Parse(s[0]),
                s[1],
                bool.Parse(s[2]),
                float.Parse(s[3])
            );
        }
    }
}

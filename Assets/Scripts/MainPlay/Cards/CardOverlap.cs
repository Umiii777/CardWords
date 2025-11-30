using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardOverlap : MonoBehaviour
{
    public static float GetOverlapArea(RectTransform a, RectTransform b)
    {
        Vector3[] ac = new Vector3[4];
        Vector3[] bc = new Vector3[4];
        a.GetWorldCorners(ac);
        b.GetWorldCorners(bc);

        float xMin = Mathf.Max(ac[0].x, bc[0].x);
        float yMin = Mathf.Max(ac[0].y, bc[0].y);
        float xMax = Mathf.Min(ac[2].x, bc[2].x);
        float yMax = Mathf.Min(ac[2].y, bc[2].y);

        if (xMax <= xMin || yMax <= yMin)
            return 0;

        return (xMax - xMin) * (yMax - yMin);
    }
}

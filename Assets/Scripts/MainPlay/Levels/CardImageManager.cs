using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardImageManager : MonoBehaviour
{
    public static CardImageManager Instance;

    private Dictionary<string, Sprite> imageCache = new Dictionary<string, Sprite>();

    private void Awake()
    {
        Instance = this;
    }

    // 获取任意图片 sprite，根据 key（例如 "img_fire"）
    public Sprite GetSprite(string key)
    {
        if (imageCache.ContainsKey(key))
            return imageCache[key];

        Sprite spr = Resources.Load<Sprite>("Icons/" + key);

        if (spr == null)
        {
            Debug.LogWarning($"CardImageManager: sprite not found for key: {key}");
            return null;
        }

        imageCache[key] = spr;
        return spr;
    }
}

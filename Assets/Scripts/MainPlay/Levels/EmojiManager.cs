using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class EmojiManager : MonoBehaviour
{
    public static EmojiManager Instance;

    private Dictionary<string, Sprite> emojiCache = new Dictionary<string, Sprite>();

    private void Awake()
    {
        Instance = this;
    }

    public Sprite GetEmojiSprite(string emoji)
    {
        if (emojiCache.TryGetValue(emoji, out Sprite cached))
            return cached;

        string fileName = ConvertEmojiToFileName(emoji);

        Sprite sprite = Resources.Load<Sprite>("emojis/" + fileName);

        // 尝试去掉 FE0F（很多资源不包含 FE0F）
        if (sprite == null && fileName.Contains("_fe0f"))
        {
            string noFe0f = fileName.Replace("_fe0f", "");
            sprite = Resources.Load<Sprite>("emojis/" + noFe0f);
        }

        if (sprite == null)
        {
            Debug.LogWarning("Emoji sprite not found: " + fileName);
            return null;
        }

        emojiCache[emoji] = sprite;
        return sprite;
    }


    // 👩‍⚖️ → emoji_u1f469_200d_2696_fe0f
    private string ConvertEmojiToFileName(string emoji)
    {
        List<string> hexList = new List<string>();

        StringInfo si = new StringInfo(emoji);
        int len = si.LengthInTextElements;

        for (int i = 0; i < len; i++)
        {
            string element = si.SubstringByTextElements(i, 1);
            int code = char.ConvertToUtf32(element, 0);
            hexList.Add(code.ToString("x"));
        }

        return "emoji_u" + string.Join("_", hexList);
    }
}

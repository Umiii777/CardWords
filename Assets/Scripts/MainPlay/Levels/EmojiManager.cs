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

        // 尝试加载没有 _fe0f 后缀的 emoji 图片
        Sprite sprite = Resources.Load<Sprite>("emojis/" + fileName);

        // 如果没有找到，尝试加载没有变体选择符的版本
        if (sprite == null)
        {
            Debug.LogWarning("Emoji sprite not found without '_fe0f': " + fileName);
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

        // 如果是 ZWJ 或变体选择符，跳过并继续处理
        if (code == 0x200D || code == 0xFE0F)
        {
            continue; // 直接跳过 ZWJ 和变体选择符
        }

        hexList.Add(code.ToString("x"));
    }

    return "emoji_u" + string.Join("_", hexList);
}
}

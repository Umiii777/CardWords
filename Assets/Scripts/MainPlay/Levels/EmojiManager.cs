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

        // 1️⃣ 原样加载（带 fe0f / zwj）
        Sprite sprite = Resources.Load<Sprite>("emojis/" + fileName);
        if (sprite != null)
        {
            emojiCache[emoji] = sprite;
            return sprite;
        }

        // 2️⃣ 去掉 fe0f 再试（❤️ 的关键）
        if (fileName.Contains("_fe0f"))
        {
            string noFe0f = fileName.Replace("_fe0f", "");
            sprite = Resources.Load<Sprite>("emojis/" + noFe0f);
            if (sprite != null)
            {
                emojiCache[emoji] = sprite;
                return sprite;
            }
        }

        // 3️⃣ 最终兜底：只用第一个 code point（防止再炸）
        int firstCodePoint = char.ConvertToUtf32(emoji, 0);
        string fallback = "emoji_u" + firstCodePoint.ToString("x");
        sprite = Resources.Load<Sprite>("emojis/" + fallback);
        if (sprite != null)
        {
            emojiCache[emoji] = sprite;
            return sprite;
        }

        Debug.LogError("❌ Emoji sprite not found: " + fileName);
        return null;
    }


    // 👩‍⚖️ → emoji_u1f469_200d_2696_fe0f
    private string ConvertEmojiToFileName(string emoji)
    {
        List<string> hexList = new List<string>();
        emoji = NormalizeDanglingZWJ(emoji);
        for (int i = 0; i < emoji.Length; i++)
        {
            int codePoint;

            // 处理代理对（高低位）
            if (char.IsHighSurrogate(emoji[i]) &&
                i + 1 < emoji.Length &&
                char.IsLowSurrogate(emoji[i + 1]))
            {
                codePoint = char.ConvertToUtf32(emoji[i], emoji[i + 1]);
                i++; // 跳过低位代理
            }
            else
            {
                codePoint = emoji[i];
            }

            // 全部保留：包括 200D / FE0F
            hexList.Add(codePoint.ToString("x"));
        }

        return "emoji_u" + string.Join("_", hexList);
    }
    private string NormalizeDanglingZWJ(string emoji)
    {
        // 如果最后一个 code point 是 ZWJ（200D），直接删掉
        if (emoji.Length == 0) return emoji;

        int lastIndex = emoji.Length - 1;

        // 处理代理对
        if (lastIndex > 0 &&
            char.IsLowSurrogate(emoji[lastIndex]) &&
            char.IsHighSurrogate(emoji[lastIndex - 1]))
        {
            int cp = char.ConvertToUtf32(emoji[lastIndex - 1], emoji[lastIndex]);
            if (cp == 0x200D)
                return emoji.Substring(0, lastIndex - 1);
        }
        else
        {
            if (emoji[lastIndex] == '\u200D')
                return emoji.Substring(0, lastIndex);
        }

        return emoji;
    }
}

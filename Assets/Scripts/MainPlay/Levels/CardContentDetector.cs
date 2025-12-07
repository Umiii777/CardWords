using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

public enum CardContentType
{
    Text,
    Image,
    Emoji
}

public static class CardContentDetector
{
    public static CardContentType GetContentType(string content)
    {
        // 1. 判断是否Emoji
        if (IsEmoji(content))
            return CardContentType.Emoji;

        // 2. 判断是否是图片关键字（你自己定义的格式）
        if (IsImageKey(content))
            return CardContentType.Image;

        // 3. 默认是文字
        return CardContentType.Text;
    }

    // 判断是否emoji
    public static bool IsEmoji(string input)
    {
        if (string.IsNullOrEmpty(input))
            return false;

        StringInfo si = new StringInfo(input);

        for (int i = 0; i < si.LengthInTextElements; i++)
        {
            string element = si.SubstringByTextElements(i, 1);
            int code = char.ConvertToUtf32(element, 0);

            // emoji 区间判断（与你原逻辑一致）
            if ((code >= 0x1F000 && code <= 0x1FAFF) ||   // 表情/物体/人物
                (code >= 0x1F300 && code <= 0x1F5FF) ||   // 杂项符号
                (code >= 0x1F600 && code <= 0x1F64F) ||   // 表情符
                (code >= 0x1F680 && code <= 0x1F6FF) ||   // 交通符号
                (code >= 0x1F900 && code <= 0x1F9FF))     // 补充符号
            {
                return true;
            }
        }

        return false;
    }

    // 判断是否你的图片key
    public static bool IsImageKey(string input)
    {
        // 示例：你可以自定义规则
        return input.StartsWith("icon_");
    }
}

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

        for (int i = 0; i < input.Length; i++)
        {
            int codePoint;

            // 处理代理对（emoji 基本都在这里）
            if (char.IsHighSurrogate(input[i]) && i + 1 < input.Length && char.IsLowSurrogate(input[i + 1]))
            {
                codePoint = char.ConvertToUtf32(input[i], input[i + 1]);
                i++; // 跳过低位代理
            }
            else
            {
                codePoint = input[i];
            }

            // === Emoji 主区间 ===
            if (
                (codePoint >= 0x1F000 && codePoint <= 0x1FAFF) || // 表情、人物、动物、物体
                (codePoint >= 0x2600 && codePoint <= 0x26FF) || // ☀ ☎ ⚛ ☪
                (codePoint >= 0x2700 && codePoint <= 0x27BF) || // ✔ ✡
                (codePoint >= 0x2300 && codePoint <= 0x23FF)     // ⌨ ⏰
            )
            {
                return true;
            }

            // === 关键补充 ===
            if (
                codePoint == 0xFE0F || // 变体选择符 ❤️ ☎️
                codePoint == 0x200D    // ZWJ 👨‍👩‍👧‍👦 🧛‍♀️
            )
            {
                // 如果是ZWJ字符，继续处理下一个字符
                if (i + 1 < input.Length)
                {
                    i++; // 跳过 ZWJ 字符后的连接部分
                }
                return true; // 处理 ZWJ 的复合字符
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

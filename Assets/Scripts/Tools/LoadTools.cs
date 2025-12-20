using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadTools : MonoBehaviour
{
    public static string EmojiToResourceKey(string emoji)
    {
        if (string.IsNullOrEmpty(emoji)) return "";

        var si = new System.Globalization.StringInfo(emoji);
        string te = si.SubstringByTextElements(0, 1); // 取整个 emoji (1个 text element)

        List<string> hexList = new List<string>();
        for (int i = 0; i < te.Length;)
        {
            int cp = char.ConvertToUtf32(te, i);
            hexList.Add(cp.ToString("x").ToLowerInvariant());
            i += char.IsSurrogatePair(te, i) ? 2 : 1;
        }

        // 拼成 emoji_uXXXX_XXXX 的格式
        return "emoji_u" + string.Join("_", hexList);
    }
}

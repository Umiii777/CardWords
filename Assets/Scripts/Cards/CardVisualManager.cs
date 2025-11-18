using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardVisualManager : MonoBehaviour
{
    public static CardVisualManager Instance;

    public Sprite spriteBack;
    public Sprite spriteFrontNormal;
    public Sprite spriteFrontMain;

    private void Awake()
    {
        Instance = this;
    }
}

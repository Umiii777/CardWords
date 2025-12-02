using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardVisualManager : MonoBehaviour
{
    public static CardVisualManager Instance;

    public Sprite spriteBack;
    public Sprite spriteFrontNormal;
    public Sprite spriteFrontMain;
    public Sprite refreshDeck;
    public Sprite normalDeck;

    private void Awake()
    {
        Instance = this;
    }
}

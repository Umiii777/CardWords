using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager :MonoBehaviour
{
    public static UIManager Instance;
    public Transform dragLayer;
    public Transform rowLayer;
    public void Awake()
    {
        Instance = this;
    }
}

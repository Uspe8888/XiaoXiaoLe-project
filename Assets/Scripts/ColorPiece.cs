using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class ColorPiece : MonoBehaviour
{
    // 定义颜色类型枚举
    public enum ColorType
    {
        YELLOW,
        PURPLE,
        RED,
        GREEN,
        BLUE,
        PINK,
        ANY,
        COUNT
    }

    // 定义颜色和精灵的结构体
    [System.Serializable]
    public struct ColorSprite
    {
        public ColorType color;
        public Sprite sprite;
    }
    public ColorSprite[] colorSprites;

    private ColorType color;

    // 颜色属性的getter和setter
    public ColorType Color
    {
        get => color;
        set { SetColor(value); }
    }

    // 颜色数量的getter
    public int NumColors
    {
        get {return colorSprites.Length;}
    }

    private SpriteRenderer sprite;

    private Dictionary<ColorType, Sprite> colorSpriteDict;

    // 初始化方法
    private void Awake()
    {
        sprite = transform.Find("piece").GetComponent<SpriteRenderer>();

        colorSpriteDict = new Dictionary<ColorType, Sprite>();

        for (int i = 0; i < colorSprites.Length; i++)
        {
            if (!colorSpriteDict.ContainsKey(colorSprites[i].color))
            {
                colorSpriteDict.Add(colorSprites[i].color, colorSprites[i].sprite);
            }
        }
    }

    // 设置颜色的方法
    public void SetColor(ColorType newColor)
    {
        color = newColor;
        if (colorSpriteDict.ContainsKey(newColor))
        {
            sprite.sprite = colorSpriteDict[newColor];
        }
    }
}

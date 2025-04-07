using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 颜色块类，用于管理游戏对象的颜色及其对应的精灵
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
    private ColorSprite[] colorSprites;

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
        // 获取waitingArea对象并初始化颜色和精灵的映射
        GameObject waitingArea = GameObject.FindGameObjectWithTag("select");
        List<Image> childImages = new List<Image>();
        foreach (Transform child in waitingArea.transform)
        {
            Image img = child.GetComponent<Image>();
            if (img != null && img.sprite != null)
            {
                childImages.Add(img);
            }
        }
        colorSprites = new ColorSprite[childImages.Count];
        for (int i = 0; i < childImages.Count; i++)
        {
            colorSprites[i] = new ColorSprite
            {
                color = (ColorType)i,
                sprite = childImages[i].sprite
            };
        }

        // 获取SpriteRenderer组件并创建颜色到精灵的字典
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
        // 设置颜色并更新精灵
        color = newColor;
        if (colorSpriteDict.ContainsKey(newColor))
        {
            sprite.sprite = colorSpriteDict[newColor];
        }
    }
}

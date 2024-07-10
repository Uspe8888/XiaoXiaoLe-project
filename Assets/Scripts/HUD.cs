using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


/// <summary>
/// 
/// </summary>
public class HUD : MonoBehaviour
{
    public Level level;

    public TextMeshProUGUI remainingText;
    public TextMeshProUGUI remainingSubText;
    public TextMeshProUGUI targetText;
    public TextMeshProUGUI targetSubText;
    public TextMeshProUGUI scoreText;
    public UnityEngine.UI.Image[] stars;

   

    private int starIdx = 0;
    private bool isGameOver = false;


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 
/// </summary>
public class Health : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;


    private void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            StartFade();
        }
        
    }

    [SerializeField] float fadeDuration = 1f;
    
    public void StartFade()
    {
        StartCoroutine(FadeCoroutine());
    }
    

    IEnumerator FadeCoroutine()
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        float timer = 0;
        
        while(timer < fadeDuration)
        {
            float alpha = Mathf.Lerp(1, 0, timer/fadeDuration);
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
            
            // 同时缩小尺寸
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, timer/fadeDuration);
            
            timer += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 
/// </summary>
public class monster : MonoBehaviour
{
    public Transform target;
    public float moveSpeed=1;

    private SpriteRenderer spr;

    private void Start()
    {
        spr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {

        spr.flipX = (transform.position.x > target.position.x);
      

        if(target)
        {
            transform.position += (target.position - transform.position).normalized*Time.deltaTime;
           
        }
    }
}

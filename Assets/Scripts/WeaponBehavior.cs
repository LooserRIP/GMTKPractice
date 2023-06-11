using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WeaponBehavior : MonoBehaviour
{
    public bool melee;
    public float damage;
    public float knockback;
    public float dIndex;

    Collider2D collision;
    float attackDur;


    // Start is called before the first frame update
    void Start()
    {
        if (melee)
            collision = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        attackDur -= Time.deltaTime;
        if (melee && attackDur < 0)
        {
            collision.enabled = false;
        }
    }

    public void Attack()
    {
        if (melee)
        {
            collision.enabled = true;
            attackDur = 0.5f;
        }
    }

    public float GetMeleeDamage()
    {
        return dIndex * damage;
    }

    public float GetKnockBack()
    {
        return dIndex * knockback;
    }
}

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

    public gameManager gm;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (melee && collision == null)
            collision = GetComponent<Collider2D>();

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.tag.Equals("Crate"))
        {
            gm.DropItem(Random.Range(0, gm.gameItems.Length), collision.transform);
            Destroy(collision.gameObject);
        }
    }
}

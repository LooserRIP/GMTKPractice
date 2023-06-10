using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    public EnemyType enemyType;
    public float trackingRange;
    public float maxHealth;
    public float health;
    public GameObject player;
    public GameObject bullet;

    float iFrames;
    float attackCooldown;

    public enum EnemyType
    {
        Crawler,
        Pillar,
        Scrambler
    }
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(enemyType == EnemyType.Crawler)
        {
            if ((player.transform.position - transform.position).magnitude < trackingRange)
                GetComponent<NavMeshAgent>().SetDestination(player.transform.position);
            else
                GetComponent<NavMeshAgent>().SetDestination(transform.position);
        }
        if(enemyType == EnemyType.Scrambler)
        {
            if (player.GetComponent<PlayerBehavior>().weakness < 40)
            {
                if ((player.transform.position - transform.position).magnitude < trackingRange)
                    GetComponent<NavMeshAgent>().SetDestination(transform.position * 2 - player.transform.position);
                else
                    GetComponent<NavMeshAgent>().SetDestination(transform.position);
            }
            else
            {
                if ((player.transform.position - transform.position).magnitude < trackingRange)
                    GetComponent<NavMeshAgent>().SetDestination(player.transform.position);
                else
                    GetComponent<NavMeshAgent>().SetDestination(transform.position);
            }
        }
        if(enemyType == EnemyType.Pillar)
        {
            if ((player.transform.position - transform.position).magnitude < trackingRange && attackCooldown < 0)
            {
                attackCooldown = 2;
                GameObject nBullet = Instantiate(bullet, transform.position, Quaternion.identity, transform);
                nBullet.GetComponent<BulletBehavior>().velocity = (player.transform.position - transform.position).normalized * 3;
            }
        }

        transform.rotation = Quaternion.identity;
        attackCooldown -= Time.deltaTime;
        iFrames -= Time.deltaTime;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag.Equals("Weapon") && iFrames < 0)
        {
            health -= col.gameObject.GetComponent<WeaponBehavior>().GetMeleeDamage();
            iFrames = 0.2f;
        }
    }
}

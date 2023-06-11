using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class EnemyBehavior : MonoBehaviour
{
    public EnemyType enemyType;
    public float trackingRange;
    public float maxHealth;
    public float health;
    public GameObject player;
    public GameObject bullet;

    public Sprite sprite;
    public Sprite flashSprite;

    public List<GameObject> constraints;
    List<Vector3> cPos;

    float iFrames;
    float attackCooldown;

    float flash;

    public enum EnemyType
    {
        Crawler,
        Pillar,
        Scrambler
    }
    // Start is called before the first frame update
    void Start()
    {
        cPos = new List<Vector3>();
        foreach(GameObject constraint in constraints)
        {
            cPos.Add(constraint.transform.position);
        }
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

            List<Vector3> offsets = new List<Vector3>()
            {
                new Vector3(-0.4f, -0.2f),
                new Vector3(0.4f, -0.2f),
                new Vector3(-0.2f, -0.1f),
                new Vector3(0.2f, -0.1f)
            };
            int i = 0;
            foreach(GameObject constraint in constraints)
            {
                if((constraint.transform.position - (transform.position + offsets[i])).magnitude > offsets[i].magnitude)
                {
                    constraint.transform.position = transform.position + offsets[i];
                    cPos[i] = constraint.transform.position;
                }
                else
                {
                    constraint.transform.position = cPos[i];
                }
                i++;
            }
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

            List<Vector3> offsets = new List<Vector3>()
            {
                new Vector3(-0.2f, -0.1f),
                new Vector3(0.2f, -0.1f),
                new Vector3(-0.1f, 0),
                new Vector3(0.1f, 0)
            };
            int i = 0;
            foreach (GameObject constraint in constraints)
            {
                if ((constraint.transform.position - (transform.position + offsets[i])).magnitude > offsets[i].magnitude)
                {
                    constraint.transform.position = transform.position + offsets[i];
                    cPos[i] = constraint.transform.position;
                }
                else
                {
                    constraint.transform.position = cPos[i];
                }
                i++;
            }
        }
        if(enemyType == EnemyType.Pillar)
        {
            if (player.transform.position.x > transform.position.x)
            {
                GetComponent<SpriteRenderer>().flipX = false;
            }
            else
            {
                GetComponent<SpriteRenderer>().flipX = true;
            }
            //transform.GetChild(0).transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2((player.transform.position - transform.position - new Vector3(0, 0.75f)).normalized.y, (player.transform.position - transform.position - new Vector3(0, 0.75f)).normalized.x) * Mathf.Rad2Deg);
            if ((player.transform.position - transform.position).magnitude < trackingRange && attackCooldown < 0)
            {
                attackCooldown = 2;
                GameObject nBullet = Instantiate(bullet, transform.position + new Vector3(0, 0.75f), Quaternion.identity);
                nBullet.GetComponent<BulletBehavior>().velocity = (player.transform.position - transform.position - new Vector3(0, 0.75f)).normalized * 3;
            }
        }

        transform.rotation = Quaternion.identity;
        attackCooldown -= Time.deltaTime;
        iFrames -= Time.deltaTime;
        if (health <= 0)
        {
            Destroy(gameObject);
        }

        if(flash < 0)
        {
            GetComponent<SpriteRenderer>().sprite = sprite;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = flashSprite;
        }

        flash -= Time.deltaTime;
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag.Equals("Weapon") && iFrames < 0)
        {
            health -= col.gameObject.GetComponent<WeaponBehavior>().GetMeleeDamage();
            transform.position += (transform.position - col.gameObject.transform.position).normalized * col.gameObject.GetComponent<WeaponBehavior>().GetKnockBack() / 25;
            iFrames = 0.2f;
            flash = 0.2f;
        }
    }
}

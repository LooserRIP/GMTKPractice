using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    public EnemyType enemyType;
    public float trackingRange;
    public float speed;
    public float health;
    public GameObject player;

    public enum EnemyType
    {
        Crawler,
        Pillar,
        Scrambler
    }
    // Start is called before the first frame update
    void Start()
    {
        
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
            transform.rotation = Quaternion.identity;
        }
    }
}

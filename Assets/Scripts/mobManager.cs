using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mobManager : MonoBehaviour
{
    public Transform playerTrans;
    public GameObject[] mobs;

    public void spawnJews(int id, float x, float y) {
        GameObject instantMob = Instantiate(mobs[id], new Vector3(x, y), Quaternion.identity, transform);
        EnemyBehavior oheverybodywantstobemyenemy = instantMob.GetComponent<EnemyBehavior>();
        oheverybodywantstobemyenemy.player = playerTrans.gameObject;
    }

}

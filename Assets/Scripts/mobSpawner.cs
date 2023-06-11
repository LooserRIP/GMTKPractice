using UnityEngine;

public class mobSpawner : MonoBehaviour
{
    public int minimumMobs = 1;
    public int maximumMobs = 3;
    public Vector2 spread;
    public int[] mobTypes = new int[]{0,1,2};
    public float weight = 0.9f;
    public float delayMinimum;
    public float delayMaximum;
    public float range = 999f;
    public bool waitForSameRoom;
    public Sprite activatedSprite;
    public SpriteRenderer sr;
    public mobManager mm;
    public Transform playerTrans;
    public bool activated = false;
    public bool spawnShown = false;
    public bool spawned = false;
    public float delay = 0f;

    public void activate() {
        activated = true;
    }
    public void spawn() {
        spawned = true;
        if (weight >= Random.value) {
            int mobAmt = Random.Range(minimumMobs, maximumMobs);
            for (int i = 0; i < mobAmt; i++) {
                Vector2 spawnMobs = transform.position;
                spawnMobs += new Vector2(Random.Range(-0.5f*spread.x, 0.5f*spread.x), Random.Range(-0.5f*spread.y, 0.5f*spread.y));
                mm.spawnJews(mobTypes[Random.Range(0, mobTypes.Length)], spawnMobs.x, spawnMobs.y);
            }
        }

    }
    public void showSpawn() {
        Debug.Log(delay);
        spawnShown = true;
        sr.sprite = activatedSprite;
        if (delay == 0f) {
            spawn();
        }
    }
    public void Update() {
        if (spawnShown && !spawned) {
            delay -= Time.deltaTime;
            Debug.Log(delay);
            if (delay <= 0f) {
                spawn();
            }
        }
        if (activated && !spawned && !spawnShown) {
            float distPlayer = Vector2.Distance(playerTrans.position, transform.position);
            Vector2Int posPlayer = new Vector2Int(Mathf.FloorToInt((playerTrans.position.x) / 5), Mathf.FloorToInt((playerTrans.position.y) / 4));
            Vector2Int pos = new Vector2Int(Mathf.FloorToInt((transform.position.x) / 5), Mathf.FloorToInt((transform.position.y) / 4));
            if (posPlayer.x == pos.x && posPlayer.y == pos.y) {
                if (distPlayer <= range) {
                    delay = Random.Range(delayMinimum, delayMaximum);
                    Debug.Log(delayMinimum);
                    Debug.Log(delayMaximum);
                    Debug.Log(delay);
                    showSpawn();
                }
            }
        }
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraManager : MonoBehaviour
{
    public Transform playerTrans;
    public void Update() {
        Vector2Int pos = new Vector2Int(Mathf.FloorToInt((playerTrans.position.x) / 9), Mathf.FloorToInt((playerTrans.position.y) / 7));
        transform.position = Vector3.Lerp(transform.position, new Vector3(pos.x * 9 + 4.5f, pos.y * 7 + 3.75f, -10), Time.deltaTime * 4);
    }
}

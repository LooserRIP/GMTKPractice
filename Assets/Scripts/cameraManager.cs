using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraManager : MonoBehaviour
{
    public Transform playerTrans;
    public void Update() {
        Vector2Int pos = new Vector2Int(Mathf.FloorToInt((playerTrans.position.x) / 5), Mathf.FloorToInt((playerTrans.position.y) / 4));
        transform.position = Vector3.Lerp(transform.position, new Vector3(pos.x * 5 + 2.5f, pos.y * 4 + 2f, -10), Time.deltaTime * 4);
    }
}

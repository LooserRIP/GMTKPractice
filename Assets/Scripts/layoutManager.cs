using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class layoutManager : MonoBehaviour
{
    public NavMeshPlus.Components.NavMeshSurface navSurface;
    public Tilemap globalwalls;
    public Tilemap globalceils;
    public TileBase wallTile;
    public TileBase ceilingTile;
    public Transform playerTrans;
    public GameObject[] layoutPrefabs;
    public List<Vector2Int> layouts;
    public Dictionary<Vector2Int, bool[]> layoutToWalls;
    public bool[] startingWalls;
    public mobManager mm;


    public void Start() {
        layouts.Clear();
        layoutToWalls = new Dictionary<Vector2Int, bool[]>(){};
        generateLayout(0,0);
    }
    public void Update() {
        Vector2Int pos = new Vector2Int(Mathf.FloorToInt(playerTrans.position.x / 5), Mathf.FloorToInt(playerTrans.position.y / 4));
        //Debug.Log(playerTrans.position.x + " - " +  playerTrans.position.y);
       // Debug.Log(pos.x + ", " + pos.y);
        if (layouts.Contains(pos)) {
            if (layoutToWalls[pos][0] && !layouts.Contains(new Vector2Int(pos.x, pos.y - 1))) {
                generateLayout(pos.x, pos.y - 1);
            }
            if (layoutToWalls[pos][1] && !layouts.Contains(new Vector2Int(pos.x + 1, pos.y))) {
                generateLayout(pos.x + 1, pos.y);
            }
            if (layoutToWalls[pos][2] && !layouts.Contains(new Vector2Int(pos.x, pos.y + 1))) {
                generateLayout(pos.x, pos.y + 1);
            }
            if (layoutToWalls[pos][3] && !layouts.Contains(new Vector2Int(pos.x - 1, pos.y))) {
                generateLayout(pos.x - 1, pos.y);
            }
        }
    }
    public void generateLayout(int x, int y) {
        layouts.Add(new Vector2Int(x, y));
        bool[] boolWalls = new bool[]{false, false, false, false};
        bool[] boolWallEnters = new bool[]{false, false, false, false};
        List<int> optionsWalls = new List<int>(){0,1,2,3};
        if (layouts.Contains(new Vector2Int(x, y - 1))) {
            optionsWalls.Remove(0);
            boolWallEnters[0] = true;
        }
        if (layouts.Contains(new Vector2Int(x + 1, y))) {
            optionsWalls.Remove(1);
            boolWallEnters[1] = true;
        }
        if (layouts.Contains(new Vector2Int(x, y + 1))) {
            optionsWalls.Remove(2);
            boolWallEnters[2] = true;
        }
        if (layouts.Contains(new Vector2Int(x - 1, y))) {
            optionsWalls.Remove(3);
            boolWallEnters[3] = true;
        }
        Debug.Log(string.Join(", ", optionsWalls));
        if (optionsWalls.Count > 0) {
            int amtOfWalls = Mathf.Clamp(Random.Range(0,3), 1, 2);
            for (int i = 0; i < amtOfWalls; i++) {
                if (optionsWalls.Count == 0) break;
                int randomIndGetOptionWalls = Random.Range(0, optionsWalls.Count);
                Debug.Log(optionsWalls[randomIndGetOptionWalls]);
                boolWalls[optionsWalls[randomIndGetOptionWalls]] = true;
                optionsWalls.Remove(randomIndGetOptionWalls);
            }
        }
        layoutToWalls.Add(new Vector2Int(x, y), boolWalls);
        Debug.Log(boolWallEnters[0] + " , " + boolWallEnters[1] + ", " + boolWallEnters[2] + ", " + boolWallEnters[3]);
        placeWalls(x * 10,y * 8, boolWalls, boolWallEnters);
        GameObject layoutInstant = Instantiate(layoutPrefabs[Random.Range(0, layoutPrefabs.Length - 1)], new Vector3(x * 5, y * 4, 0), Quaternion.identity, transform);
        navSurface.BuildNavMesh();
        Transform layoutObjects = layoutInstant.transform.GetChild(2);
        for (int ic = 0; ic < layoutObjects.childCount; ic++) {
            Transform layoutObject = layoutObjects.GetChild(ic);
            mobSpawner mb = layoutObject.GetComponent<mobSpawner>();
            if (mb != null) {
                mb.mm = mm;
                mb.playerTrans = playerTrans;
                mb.activate();
            }
        }
    }
    public void placeWalls(int x, int y, bool[] walls, bool[] wallenters) {
        for (int ix = 0; ix < 9; ix++) {
            if (!wallenters[0] || ix == 9 || ix == 0)
            {
                globalwalls.SetTile(new Vector3Int(x + ix, y), wallTile);
                globalceils.SetTile(new Vector3Int(x + ix, y + 1), ceilingTile);
            }
            if (!wallenters[2] || ix == 9 || ix == 0)
            {
                globalwalls.SetTile(new Vector3Int(x + ix, y + 7), wallTile);
                globalceils.SetTile(new Vector3Int(x + ix, y + 8), ceilingTile);
            }
        }
        for (int iy = 0; iy < 8; iy++) {
            if (!wallenters[3] || iy == 7 || iy == 0)
            {
                globalwalls.SetTile(new Vector3Int(x, y + iy), wallTile);
                globalceils.SetTile(new Vector3Int(x, y + iy + 1), ceilingTile);
            }
            if (!wallenters[1] || iy == 7 || iy == 0)
            {
                globalwalls.SetTile(new Vector3Int(x + 9, y + iy), wallTile);
                globalceils.SetTile(new Vector3Int(x + 9, y + iy + 1), ceilingTile);
            }
        }
        if (walls[0]) {
            globalwalls.SetTile(new Vector3Int(x + 4, y), null);
            globalwalls.SetTile(new Vector3Int(x + 5, y), null);
            globalceils.SetTile(new Vector3Int(x + 4, y + 1), null);
            globalceils.SetTile(new Vector3Int(x + 5, y + 1), null);
        }
        if (walls[1]) {
            globalwalls.SetTile(new Vector3Int(x + 9, y + 3), null);
            globalwalls.SetTile(new Vector3Int(x + 9, y + 4), null);
            globalceils.SetTile(new Vector3Int(x + 9, y + 4), null);
            globalceils.SetTile(new Vector3Int(x + 9, y + 5), null);
        }
        if (walls[2]) {
            globalwalls.SetTile(new Vector3Int(x + 4, y + 7), null);
            globalwalls.SetTile(new Vector3Int(x + 5, y + 7), null);
            globalceils.SetTile(new Vector3Int(x + 4, y + 8), null);
            globalceils.SetTile(new Vector3Int(x + 5, y + 8), null);
        }
        if (walls[3]) {
            globalwalls.SetTile(new Vector3Int(x, y + 3), null);
            globalwalls.SetTile(new Vector3Int(x, y + 4), null);
            globalceils.SetTile(new Vector3Int(x, y + 4), null);
            globalceils.SetTile(new Vector3Int(x, y + 5), null);
        }
    }

}

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
    public Dictionary<Vector2Int, GameObject> layoutToGameObject;
    public int enemiesLeft;
    public TileBase[] forcefieldTiles;
    private Vector2Int forcefieldLocation;
    public bool[] startingWalls;
    public mobManager mm;
    public int layoutCounter = 0;


    public void Start() {
        enemiesLeft = 0;
        layoutCounter = 0;
        layouts.Clear();
        layoutToWalls = new Dictionary<Vector2Int, bool[]>(){};
        layoutToGameObject = new Dictionary<Vector2Int, GameObject>(){};
        generateLayout(0,0);
    }
    public void Update() {
        Vector2Int pos = new Vector2Int(Mathf.FloorToInt(playerTrans.position.x / 9), Mathf.FloorToInt(playerTrans.position.y / 7));
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
            if (Random.value >= ((layoutCounter <= 2) ? 0.75f : 0.9f)) amtOfWalls = 4;
            if (Random.value >= 0.8f && layoutCounter > 5) amtOfWalls = 0;
            for (int i = 0; i < amtOfWalls; i++) {
                if (optionsWalls.Count == 0) break;
                int randomIndGetOptionWalls = Random.Range(0, optionsWalls.Count);
                Debug.Log(optionsWalls[randomIndGetOptionWalls]);
                boolWalls[optionsWalls[randomIndGetOptionWalls]] = true;
                optionsWalls.Remove(randomIndGetOptionWalls);
            }
        }
        layoutToWalls.Add(new Vector2Int(x, y), boolWalls);
       // Debug.Log(boolWallEnters[0] + " , " + boolWallEnters[1] + ", " + boolWallEnters[2] + ", " + boolWallEnters[3]);
        GameObject layoutprefab = layoutPrefabs[Random.Range(0, layoutPrefabs.Length)];
        GameObject layoutInstant = Instantiate(layoutprefab, new Vector3(x * 9, y * 7, 0), Quaternion.identity, transform);
        layoutToGameObject.Add(new Vector2Int(x, y), layoutInstant);
        navSurface.BuildNavMesh();
        enemiesLeft = 0;
        Transform layoutObjects = layoutInstant.transform.GetChild(3);
        for (int ic = 0; ic < layoutObjects.childCount; ic++) {
            Transform layoutObject = layoutObjects.GetChild(ic);
            mobSpawner mb = layoutObject.GetComponent<mobSpawner>();
            if (mb != null) {
                mb.mm = mm;
                mb.playerTrans = playerTrans;
                mb.lm = this;
                mb.activate();
                if (mb.countEnemiesForForcefield) {
                    enemiesLeft += mb.mobsSpawning;
                }
            }
        }
        placeWalls(x * 18,y * 14, boolWalls, boolWallEnters);
        layoutCounter++;
    }
    public void placeWalls(int x, int y, bool[] walls, bool[] wallenters) {
        Debug.Log(x + ", " + y);
        for (int ix = 0; ix < 17; ix++) {
            if (!wallenters[0] || ix == 17 || ix == 0)
            {
                globalwalls.SetTile(new Vector3Int(x + ix, y), wallTile);
                globalceils.SetTile(new Vector3Int(x + ix, y + 1), ceilingTile);
            }
            if (!wallenters[2] || ix == 17 || ix == 0)
            {
                globalwalls.SetTile(new Vector3Int(x + ix, y + 13), wallTile);
                globalceils.SetTile(new Vector3Int(x + ix, y + 14), ceilingTile);
            }
        }
        for (int iy = 0; iy < 14; iy++) {
            if (!wallenters[3] || iy == 13 || iy == 0)
            {
                globalwalls.SetTile(new Vector3Int(x, y + iy), wallTile);
                globalceils.SetTile(new Vector3Int(x, y + iy + 1), ceilingTile);
            }
            if (!wallenters[1] || iy == 13 || iy == 0)
            {
                globalwalls.SetTile(new Vector3Int(x + 17, y + iy), wallTile);
                globalceils.SetTile(new Vector3Int(x + 17, y + iy + 1), ceilingTile);
            }
        }
        if (walls[0]) {
            globalwalls.SetTile(new Vector3Int(x + 8, y), null);
            globalwalls.SetTile(new Vector3Int(x + 9, y), null);
            globalceils.SetTile(new Vector3Int(x + 8, y + 1), null);
            globalceils.SetTile(new Vector3Int(x + 9, y + 1), null);
        }
        if (walls[1]) {
            globalwalls.SetTile(new Vector3Int(x + 17, y + 6), null);
            globalwalls.SetTile(new Vector3Int(x + 17, y + 7), null);
            globalceils.SetTile(new Vector3Int(x + 17, y + 7), null);
            globalceils.SetTile(new Vector3Int(x + 17, y + 8), null);
        }
        if (walls[2]) {
            globalwalls.SetTile(new Vector3Int(x + 9, y + 13), null);
            globalwalls.SetTile(new Vector3Int(x + 8, y + 13), null);
            globalceils.SetTile(new Vector3Int(x + 9, y + 14), null);
            globalceils.SetTile(new Vector3Int(x + 8, y + 14), null);
        }
        if (walls[3]) {
            globalwalls.SetTile(new Vector3Int(x, y + 6), null);
            globalwalls.SetTile(new Vector3Int(x, y + 7), null);
            globalceils.SetTile(new Vector3Int(x, y + 7), null);
            globalceils.SetTile(new Vector3Int(x, y + 8), null);
        }
    }
    public void placeForcefield(int xr, int yr, int enemiesAdd) {
        forcefieldLocation = new Vector2Int(xr, yr);
        enemiesLeft += enemiesAdd;
        int x = xr * 18;
        int y = yr * 14;
        Vector3Int[] forcefieldLocs = new Vector3Int[]{new Vector3Int(x + 8, y), new Vector3Int(x + 9, y), new Vector3Int(x + 9, y + 13), new Vector3Int(x + 8, y + 13), 
                                                    new Vector3Int(x, y + 6), new Vector3Int(x, y + 7), new Vector3Int(x + 17, y + 6), new Vector3Int(x + 17, y + 7)};
        for (int ffi = 0; ffi < 8; ffi++) {
            if (globalwalls.GetTile(forcefieldLocs[ffi]) == null) {
                if (ffi <= 3) {
                    if (globalwalls.GetTile(new Vector3Int(forcefieldLocs[ffi].x - 2, forcefieldLocs[ffi].y)) == null) {
                        forcefieldLocs[ffi].y += ((ffi <= 1) ? -1 : 1);
                    }
                }
                /*
                if (ffi >= 4) {
                    if (globalwalls.GetTile(new Vector3Int(forcefieldLocs[ffi].x, forcefieldLocs[ffi].y - 2)) == null) {
                        forcefieldLocs[ffi].x -= ((ffi <= 5) ? -1 : 1);
                    }
                }*/
                globalwalls.SetTile(forcefieldLocs[ffi], forcefieldTiles[((ffi >= 4) ? 1 : 0)]);
            }
        }
        navSurface.BuildNavMesh();
    }
    public void enemyDeath() {
        if (enemiesLeft > 0) {
            enemiesLeft -= 1;
            if (enemiesLeft == 0) {
                Debug.Log("removing forcefield");
                int x = forcefieldLocation.x * 18;
                int y = forcefieldLocation.y * 14;
                Debug.Log(x + " , " + y);
                int xe = x + 19;
                int ye = y + 15;
                Tilemap layouttiles = layoutToGameObject[new Vector2Int(forcefieldLocation.x, forcefieldLocation.y)].transform.GetChild(1).GetComponent<Tilemap>();

                for (int xi = x; xi < xe; xi++) {
                    for (int yi = y; yi < ye; yi++) {
                        Vector3Int locCheck = new Vector3Int(xi, yi);
                        TileBase gt = globalwalls.GetTile(locCheck);
                        if (gt == null)  {
                            locCheck = new Vector3Int(xi - x, yi - y);
                            gt = layouttiles.GetTile(locCheck);
                            if (gt == forcefieldTiles[0] || gt == forcefieldTiles[1]) {
                                layouttiles.SetTile(locCheck,null);
                            }
                        } else {
                            if (gt == forcefieldTiles[0] || gt == forcefieldTiles[1]) {
                                globalwalls.SetTile(locCheck,null);
                            }
                        }
                    }
                }
                navSurface.BuildNavMesh();

            }
        }
    }

}

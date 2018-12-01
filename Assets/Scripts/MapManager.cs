using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

    public int mapSize = 5;
    public float tileHeight = 4;
    public float tileWidth = 5;
    public GameObject tilePrefab;
    public GameObject selectorPrefab;

    private GameObject[][] map;
    public GameObject[][] Map { get { return map; } }
    private GameObject selector;

    void Start() {

        GenerateMap();
        selector = Instantiate<GameObject>(selectorPrefab);
        selector.SetActive(false);
    }

    private void GenerateMap() {

        map = new GameObject[mapSize][];

        float initPosX = 0;
        float initPosY = tileHeight * (mapSize - 1) / 2;
        for (int i = 0; i < mapSize; i++) {
            map[i] = new GameObject[mapSize];
            for (int j = 0; j < mapSize; j++) {
                GameObject tileClone = Instantiate<GameObject>(tilePrefab);
                tileClone.transform.position = new Vector3(initPosX - i * tileWidth / 2 + j * tileWidth / 2, initPosY - i * tileHeight / 2 - j * tileHeight / 2, 0);
                tileClone.transform.parent = gameObject.transform;
                tileClone.name = tilePrefab.name + "" + i + "" + j;
                map[i][j] = tileClone;
            }
        }

        InitTileData();
    }

    private void InitTileData() {

        List<int> rainValues = new List<int>();
        for (int i = 0; i < mapSize * mapSize; i++) {
            if (i == 0) {
                rainValues.Add(130);
            } else {
                rainValues.Add(rainValues[i - 1] + 20);
            }
        }

        for (int i = 0; i < mapSize; i++) {
            for (int j = 0; j < mapSize; j++) {
                int pickedIndex = Random.Range(0, rainValues.Count);
                map[i][j].GetComponent<TileData>().initRainValue = rainValues[pickedIndex];
                rainValues.RemoveAt(pickedIndex);
            }
        }

    }

    void Update() {

        int i = 0;
        bool mouseDetected = false;
        while (i < mapSize && !mouseDetected) {
            int j = 0;
            while (j < mapSize && !mouseDetected) {
                TileMouseDetector mouseDetector = map[i][j].GetComponent<TileMouseDetector>();
                if (mouseDetector.MouseIsOver) {
                    selector.transform.position = map[i][j].transform.position;
                    selector.SetActive(true);
                    mouseDetected = true;
                }
                j++;
            }
            i++;
        }
        if (!mouseDetected)
            selector.SetActive(false);
    }
}

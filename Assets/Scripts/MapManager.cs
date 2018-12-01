using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

    public int mapSize = 5;
    public float tileHeight = 4;
    public float tileWidth = 5;
    public GameObject tilePrefab;
    public GameObject selectorPrefab;
    public GameObject rainEffectPrefab;

    private GameObject[][] map;
    public GameObject[][] Map { get { return map; } }
    private GameObject selector;
    private GameObject rainEffect;

    void Start() {

        GenerateMap();
        selector = Instantiate<GameObject>(selectorPrefab);
        selector.SetActive(false);
        rainEffect = Instantiate<GameObject>(rainEffectPrefab);
        rainEffect.GetComponentInChildren<ParticleSystem>().Stop();
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
                //map[i][j].GetComponent<TileData>().initRainValue = rainValues[pickedIndex];
                map[i][j].GetComponent<TileData>().initRainValue = 0;
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
                    rainEffect.transform.position = map[i][j].transform.position;
                    mouseDetected = true;
                }
                j++;
            }
            i++;
        }
        if (!mouseDetected) {
            selector.SetActive(false);
        }

        if (mouseDetected && Input.GetMouseButton(0)) {
            if (!rainEffect.GetComponentInChildren<ParticleSystem>().isPlaying)
                rainEffect.GetComponentInChildren<ParticleSystem>().Play();
        } else {
            rainEffect.GetComponentInChildren<ParticleSystem>().Stop();
        }
    }

    public void HideAllTiles() {
        for (int i = 0; i < mapSize; i++) {
            for (int j = 0; j < mapSize; j++) {
                map[i][j].SetActive(false);
            }
        }
    }

    public void ShowOneTile() {

        map[mapSize / 2][mapSize / 2].SetActive(true);
    }

    public void ShowCrossedTiles() {
        map[mapSize / 2][mapSize / 2 + 1].SetActive(true);
        map[mapSize / 2][mapSize / 2 - 1].SetActive(true);
        map[mapSize / 2 + 1][mapSize / 2].SetActive(true);
        map[mapSize / 2 - 1][mapSize / 2].SetActive(true);
    }

    public void SetCrossedTilesTransparency(float alpha) {
        Color yPlus1Color = map[mapSize / 2][mapSize / 2 + 1].GetComponent<SpriteRenderer>().color;
        map[mapSize / 2][mapSize / 2 + 1].GetComponent<SpriteRenderer>().color = new Color(yPlus1Color.r, yPlus1Color.g, yPlus1Color.b, alpha);
        Color yMinus1Color = map[mapSize / 2][mapSize / 2 - 1].GetComponent<SpriteRenderer>().color;
        map[mapSize / 2][mapSize / 2 - 1].GetComponent<SpriteRenderer>().color = new Color(yMinus1Color.r, yMinus1Color.g, yMinus1Color.b, alpha);
        Color xPlus1Color = map[mapSize / 2 + 1][mapSize / 2].GetComponent<SpriteRenderer>().color;
        map[mapSize / 2 + 1][mapSize / 2].GetComponent<SpriteRenderer>().color = new Color(xPlus1Color.r, xPlus1Color.g, xPlus1Color.b, alpha);
        Color xMinus1Color = map[mapSize / 2 - 1][mapSize / 2].GetComponent<SpriteRenderer>().color;
        map[mapSize / 2 - 1][mapSize / 2].GetComponent<SpriteRenderer>().color = new Color(xMinus1Color.r, xMinus1Color.g, xMinus1Color.b, alpha);
    }
}

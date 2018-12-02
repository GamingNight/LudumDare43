using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

    public int mapSize = 5;
    public float tileHeight = 4;
    public float tileWidth = 5;
    public GameObject tilePrefab;
    public GameObject selectorPrefab;
    public GameObject[] particleEffectPrefabs;

    private GameObject[][] map;
    public GameObject[][] Map { get { return map; } }
    private GameObject selector;
    private Dictionary<TileData.StepName, GameObject> particleEffects;
    private TileData.StepName activeParticleType;

    void Start() {

        GenerateMap();
        selector = Instantiate<GameObject>(selectorPrefab);
        selector.SetActive(false);
        particleEffects = new Dictionary<TileData.StepName, GameObject>();
        for (int i = 0; i < particleEffectPrefabs.Length; i++) {
            if (i == 0) {
                particleEffects[TileData.StepName.RAIN] = Instantiate<GameObject>(particleEffectPrefabs[i]);
                particleEffects[TileData.StepName.RAIN].GetComponentInChildren<ParticleSystem>().Stop();
            } else if (i == 1) {
                particleEffects[TileData.StepName.WIND] = Instantiate<GameObject>(particleEffectPrefabs[i]);
                particleEffects[TileData.StepName.WIND].GetComponentInChildren<ParticleSystem>().Stop();
            } else if (i == 2) {
                particleEffects[TileData.StepName.SUN] = Instantiate<GameObject>(particleEffectPrefabs[i]);
                particleEffects[TileData.StepName.SUN].GetComponentInChildren<ParticleSystem>().Stop();
            }
        }
        activeParticleType = TileData.StepName.RAIN;
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

        //List<int> rainValues = new List<int>();
        //for (int i = 0; i < mapSize * mapSize; i++) {
        //    if (i == 0) {
        //        rainValues.Add(130);
        //    } else {
        //        rainValues.Add(rainValues[i - 1] + 20);
        //    }
        //}

        for (int i = 0; i < mapSize; i++) {
            for (int j = 0; j < mapSize; j++) {
                //int pickedIndex = Random.Range(0, rainValues.Count);
                //map[i][j].GetComponent<TileData>().initRainValue = rainValues[pickedIndex];
                map[i][j].GetComponent<TileData>().initValue = 0;
                //rainValues.RemoveAt(pickedIndex);
            }
        }

    }

    void Update() {

        //Change selected particle effect as soon as player press Space
        if (Input.GetMouseButton(1)) {
            particleEffects[activeParticleType].GetComponentInChildren<ParticleSystem>().Stop();
            if (activeParticleType == TileData.StepName.RAIN) {
                activeParticleType = TileData.StepName.WIND;
            } else if (activeParticleType == TileData.StepName.WIND) {
                activeParticleType = TileData.StepName.SUN;
            } else if (activeParticleType == TileData.StepName.SUN) {
                activeParticleType = TileData.StepName.RAIN;
            }
        }

        //Update selector and particle position depending on mouse position
        int i = 0;
        bool mouseDetected = false;
        int mouseTileI = -1;
        int mouseTileJ = -1;
        while (i < mapSize && !mouseDetected) {
            int j = 0;
            while (j < mapSize && !mouseDetected) {
                TileMouseDetector mouseDetector = map[i][j].GetComponent<TileMouseDetector>();
                if (mouseDetector.MouseIsOver) {
                    selector.transform.position = map[i][j].transform.position;
                    selector.SetActive(true);
                    particleEffects[activeParticleType].transform.position = map[i][j].transform.position;
                    mouseDetected = true;
                    mouseTileI = i;
                    mouseTileJ = j;
                }
                j++;
            }
            i++;
        }

        //Disable selector if mouse is away
        if (!mouseDetected) {
            selector.SetActive(false);
        }

        //Activate particle effect if player presses mouse left button
        if (mouseDetected && Input.GetMouseButton(0)) {
            if (!particleEffects[activeParticleType].GetComponentInChildren<ParticleSystem>().isPlaying)
                particleEffects[activeParticleType].GetComponentInChildren<ParticleSystem>().Play();
        } else {
            particleEffects[activeParticleType].GetComponentInChildren<ParticleSystem>().Stop();
        }

        //Update all tile values depending on particle effect type
        for (i = 0; i < mapSize; i++) {
            for (int j = 0; j < mapSize; j++) {
                bool increaseValue = false;
                if (Input.GetMouseButton(0) && i == mouseTileI && j == mouseTileJ && activeParticleType == map[i][j].GetComponent<TileData>().LastStep.name) {
                    increaseValue = true;
                }
                map[i][j].GetComponent<TileData>().UpdateValue(increaseValue);
            }
        }
    }

    public void HideAllTiles() {
        for (int i = 0; i < mapSize; i++) {
            for (int j = 0; j < mapSize; j++) {
                map[i][j].SetActive(false);
            }
        }
    }

    public void ShowTile(int i, int j) {

        map[i][j].SetActive(true);
    }

    public void SetTileTransparency(int i, int j, float alpha) {
        Color tileColor = map[i][j].GetComponent<SpriteRenderer>().color;
        map[i][j].GetComponent<SpriteRenderer>().color = new Color(tileColor.r, tileColor.g, tileColor.b, alpha);
    }

    public float ComputeScore() {

        float score = 0;
        for (int i = 0; i < mapSize; i++) {
            for (int j = 0; j < mapSize; j++) {
                score += map[i][j].GetComponent<TileData>().Value;
            }
        }
        return score;
    }

    public float ComputeMeanScore() {

        float mean = 0;
        for (int i = 0; i < mapSize; i++) {
            for (int j = 0; j < mapSize; j++) {
                mean += map[i][j].GetComponent<TileData>().Value;
            }
        }
        if (mapSize != 0) {
            mean /= (mapSize * mapSize);
        }
        return mean;
    }

    public float ComputeStandardDeviationScore() {

        float sd = 0;
        float mean = ComputeMeanScore();
        for (int i = 0; i < mapSize; i++) {
            for (int j = 0; j < mapSize; j++) {
                float value = map[i][j].GetComponent<TileData>().Value;
                sd += Mathf.Pow(mean - value, 2);
            }
        }
        if (mapSize != 0) {
            sd = Mathf.Sqrt(sd / (mapSize * mapSize));
        }
        return sd;
    }

    public void PrintScores() {

        for (int i = 0; i < mapSize; i++) {
            for (int j = 0; j < mapSize; j++) {
                Debug.Log(i + "_" + j + " = " + map[i][j].GetComponent<TileData>().Value);
            }
        }
    }
}

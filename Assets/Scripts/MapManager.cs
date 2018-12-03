using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour {

    public int mapSize = 5;
    public float tileHeight = 4;
    public float tileWidth = 5;
    public GameObject tilePrefab;
    public GameObject selectorPrefab;
    public GameObject[] particleEffectPrefabs;
    public SelectIcon[] particleEffectIcons;
    public AudioClip[] particleEffectClips;

    private GameObject[][] map;
    public GameObject[][] Map { get { return map; } }
    private GameObject selector;
    private Dictionary<TileData.StepName, GameObject> particleEffects;
    private TileData.StepName activeEffect;
    private bool windEffectIsAvailable;
    private bool sunEffectIsAvailable;
    private AudioSource audioSource;

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
        activeEffect = TileData.StepName.RAIN;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = particleEffectClips[0];
        windEffectIsAvailable = false;
        sunEffectIsAvailable = false;
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
                tileClone.GetComponent<SpriteRenderer>().sortingOrder = 2 + (i * mapSize + j);
                map[i][j] = tileClone;
            }
        }

        InitTileData();
    }

    private void InitTileData() {

        for (int i = 0; i < mapSize; i++) {
            for (int j = 0; j < mapSize; j++) {
                map[i][j].GetComponent<TileData>().initValue = 0;
            }
        }

    }

    void Update() {

        //Change selected particle effect as soon as player press Space
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space)) {
            if (windEffectIsAvailable) {
                particleEffects[activeEffect].GetComponentInChildren<ParticleSystem>().Stop();
                audioSource.Stop();
            }
            if (activeEffect == TileData.StepName.RAIN) {
                if (windEffectIsAvailable) {
                    activeEffect = TileData.StepName.WIND;
                    particleEffectIcons[0].Unselect();
                    particleEffectIcons[1].Select();
                    audioSource.clip = particleEffectClips[1];
                }
            } else if (activeEffect == TileData.StepName.WIND) {
                if (sunEffectIsAvailable) {
                    activeEffect = TileData.StepName.SUN;
                    particleEffectIcons[1].Unselect();
                    particleEffectIcons[2].Select();
                    audioSource.clip = particleEffectClips[2];
                } else {
                    activeEffect = TileData.StepName.RAIN;
                    particleEffectIcons[1].Unselect();
                    particleEffectIcons[0].Select();
                    audioSource.clip = particleEffectClips[0];
                }
            } else if (activeEffect == TileData.StepName.SUN) {
                activeEffect = TileData.StepName.RAIN;
                particleEffectIcons[2].Unselect();
                particleEffectIcons[0].Select();
                audioSource.clip = particleEffectClips[0];
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
                    particleEffects[activeEffect].transform.position = map[i][j].transform.position;
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
            if (!particleEffects[activeEffect].GetComponentInChildren<ParticleSystem>().isPlaying)
                particleEffects[activeEffect].GetComponentInChildren<ParticleSystem>().Play();
            if (!audioSource.isPlaying)
                audioSource.Play();
        } else {
            particleEffects[activeEffect].GetComponentInChildren<ParticleSystem>().Stop();
            audioSource.Stop();
        }

        //Update all tile values depending on particle effect type
        for (i = 0; i < mapSize; i++) {
            for (int j = 0; j < mapSize; j++) {
                bool increaseValue = false;
                bool effectTypeIsCompatible = false;
                if (map[i][j].GetComponent<TileData>().LastStep != null)
                    effectTypeIsCompatible = map[i][j].GetComponent<TileData>().LastStep.name.ToString().Contains(activeEffect.ToString());
                //bool effectTypeIsCompatible = activeParticleType == map[i][j].GetComponent<TileData>().LastStep.name;
                if (Input.GetMouseButton(0) && i == mouseTileI && j == mouseTileJ && effectTypeIsCompatible) {
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

    public float TerritoryEqualityBonus() {
        // Fixme 12 is the number of step
        // please update if you update scoreSteps and scoreSteps_ref
        int nbSteps = 11;
        int[] scoreSteps = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        int[] scoreSteps_ref = new int[] { 0, 50, 100, 200, 300, 320, 400, 550, 700, 720, 850, 1000 };

        for (int i = 0; i < mapSize; i++) {
            for (int j = 0; j < mapSize; j++) {
                float score = map[i][j].GetComponent<TileData>().Value;
                int steps = 0;
                for (int k = 0; k < nbSteps; k++) {
                    if (score > scoreSteps_ref[k])
                        steps = k;
                }
                scoreSteps[steps] = scoreSteps[steps] + 1;
            }
        }
        int bonus = 1;

        // Do not count empty tile -> start at 1
        for (int i = 1; i < nbSteps; i++) {
            if (bonus < scoreSteps[i]) {
                bonus = scoreSteps[i];
            }
        }
        return (float)bonus;

    }


    public void PrintScores() {

        for (int i = 0; i < mapSize; i++) {
            for (int j = 0; j < mapSize; j++) {
                Debug.Log(i + "_" + j + " = " + map[i][j].GetComponent<TileData>().Value);
            }
        }
    }

    public void ReleaseWindEffect() {
        windEffectIsAvailable = true;
    }

    public void ReleaseSunEffect() {
        sunEffectIsAvailable = true;
    }
}

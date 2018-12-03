﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameScenario : MonoBehaviour {

    public enum ScenarioState {
        Enter, OneTile, CrossTiles, AllTiles
    }

    public MapManager mapManager;
    public GameObject[] elementIcons;
    public GameObject elementControlIcon;
    private ScenarioState state;
    private GameTimer timer;
    private GameScore currentScore;

    void Start() {
        state = ScenarioState.Enter;
        timer = GetComponent<GameTimer>();
        currentScore = GetComponent<GameScore>();
    }

    void Update() {

        if (state == ScenarioState.Enter) {
            InitOneTileState();
            state = ScenarioState.OneTile;
        } else if (state == ScenarioState.OneTile) {
            TileData middleTileData = mapManager.Map[mapManager.mapSize / 2][mapManager.mapSize / 2].GetComponent<TileData>();
            if (middleTileData.Value >= 320) {
                InitCrossTilesState();
                ReleaseWindEffect();
                state = ScenarioState.CrossTiles;
            }
        } else if (state == ScenarioState.CrossTiles) {
            TileData middleTileData = mapManager.Map[mapManager.mapSize / 2][mapManager.mapSize / 2].GetComponent<TileData>();
            TileData upTileData = mapManager.Map[mapManager.mapSize / 2][mapManager.mapSize / 2 + 1].GetComponent<TileData>();
            TileData downTileData = mapManager.Map[mapManager.mapSize / 2][mapManager.mapSize / 2 - 1].GetComponent<TileData>();
            TileData leftTileData = mapManager.Map[mapManager.mapSize / 2 - 1][mapManager.mapSize / 2].GetComponent<TileData>();
            TileData rightTileData = mapManager.Map[mapManager.mapSize / 2 + 1][mapManager.mapSize / 2].GetComponent<TileData>();
            if (middleTileData.Value >= 300 && upTileData.Value >= 300 && downTileData.Value >= 300 && leftTileData.Value >= 300 && rightTileData.Value >= 300) {
                InitAllTilesState();
                state = ScenarioState.AllTiles;
            }
        }

        //Release sun effect as soon as a tile has reached the level
        bool sunLevelReached = false;
        int i = 0;
        while (i < mapManager.mapSize && !sunLevelReached) {
            int j = 0;
            while (j < mapManager.mapSize && !sunLevelReached) {
                sunLevelReached = mapManager.Map[i][j].GetComponent<TileData>().Value >= 720;
                j++;
            }
            i++;
        }
        if (sunLevelReached) {
            ReleaseSunEffect();
        }

        //Time over = Game over 
        if (timer.TimeOver) {
            EndGameStats.RAW_SCORE = (float)currentScore.totalScore;
            EndGameStats.HOMOGENEITY_MULTIPLIER = currentScore.homogeneityCoef;
            SceneManager.LoadScene("gameover");
        }
    }

    private void InitOneTileState() {
        mapManager.HideAllTiles();
        mapManager.ShowTile(mapManager.mapSize / 2, mapManager.mapSize / 2);
    }

    private void InitCrossTilesState() {
        StartCoroutine(CameraUnzoomCoroutine(5f, 2f));
        StartCoroutine(CrossTilesActivationCoroutine(2f));
    }

    private void InitAllTilesState() {
        StartCoroutine(CameraUnzoomCoroutine(7f, 2f));
        StartCoroutine(AllTilesActivationCoroutine(2f));
    }

    private void ReleaseWindEffect() {
        StartCoroutine(DisplayWindIconCoroutine(2f));
        mapManager.ReleaseWindEffect();
        StartCoroutine(DisplayControlIconCoroutine(5f));
    }

    private void ReleaseSunEffect() {
        StartCoroutine(DisplaySunIconCoroutine(2f));
        mapManager.ReleaseSunEffect();
    }

    private IEnumerator CameraUnzoomCoroutine(float targetSize, float zoomDuration) {

        float initSize = Camera.main.orthographicSize;
        float interpolation = 0;
        while (Camera.main.orthographicSize < targetSize) {
            Camera.main.orthographicSize = Mathf.Lerp(initSize, targetSize, interpolation / zoomDuration);
            interpolation += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator CrossTilesActivationCoroutine(float lerpDuration) {

        int middleMap = mapManager.mapSize / 2;
        mapManager.SetTileTransparency(middleMap + 1, middleMap, 0);
        mapManager.SetTileTransparency(middleMap - 1, middleMap, 0);
        mapManager.SetTileTransparency(middleMap, middleMap + 1, 0);
        mapManager.SetTileTransparency(middleMap, middleMap - 1, 0);
        mapManager.ShowTile(middleMap + 1, middleMap);
        mapManager.ShowTile(middleMap - 1, middleMap);
        mapManager.ShowTile(middleMap, middleMap + 1);
        mapManager.ShowTile(middleMap, middleMap - 1);
        float interpolation = 0;
        while ((interpolation / lerpDuration) < 1) {
            float lerpValue = Mathf.Lerp(0, 1, interpolation / lerpDuration);
            mapManager.SetTileTransparency(middleMap + 1, middleMap, lerpValue);
            mapManager.SetTileTransparency(middleMap - 1, middleMap, lerpValue);
            mapManager.SetTileTransparency(middleMap, middleMap + 1, lerpValue);
            mapManager.SetTileTransparency(middleMap, middleMap - 1, lerpValue);
            interpolation += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator AllTilesActivationCoroutine(float lerpDuration) {
        int middleMap = mapManager.mapSize / 2;
        mapManager.SetTileTransparency(middleMap + 1, middleMap + 1, 0);
        mapManager.SetTileTransparency(middleMap - 1, middleMap + 1, 0);
        mapManager.SetTileTransparency(middleMap + 1, middleMap - 1, 0);
        mapManager.SetTileTransparency(middleMap - 1, middleMap - 1, 0);
        mapManager.ShowTile(middleMap + 1, middleMap + 1);
        mapManager.ShowTile(middleMap - 1, middleMap + 1);
        mapManager.ShowTile(middleMap + 1, middleMap - 1);
        mapManager.ShowTile(middleMap - 1, middleMap - 1);
        float interpolation = 0;
        while ((interpolation / lerpDuration) < 1) {
            float lerpValue = Mathf.Lerp(0, 1, interpolation / lerpDuration);
            mapManager.SetTileTransparency(middleMap + 1, middleMap + 1, lerpValue);
            mapManager.SetTileTransparency(middleMap - 1, middleMap + 1, lerpValue);
            mapManager.SetTileTransparency(middleMap + 1, middleMap - 1, lerpValue);
            mapManager.SetTileTransparency(middleMap - 1, middleMap - 1, lerpValue);
            interpolation += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator DisplayWindIconCoroutine(float lerpDuration) {

        Image windImage = elementIcons[1].GetComponent<Image>();
        Color windColor = windImage.color;
        windImage.color = new Color(windColor.r, windColor.g, windColor.b, 0);
        elementIcons[1].SetActive(true);
        float interpolation = 0;
        while (interpolation < lerpDuration) {
            float lerpValue = Mathf.Lerp(0, 1, interpolation / lerpDuration);
            windImage.color = new Color(windColor.r, windColor.g, windColor.b, lerpValue);
            interpolation += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator DisplaySunIconCoroutine(float lerpDuration) {

        Image sunImage = elementIcons[2].GetComponent<Image>();
        Color windColor = sunImage.color;
        sunImage.color = new Color(windColor.r, windColor.g, windColor.b, 0);
        elementIcons[2].SetActive(true);
        float interpolation = 0;
        while (interpolation < lerpDuration) {
            float lerpValue = Mathf.Lerp(0, 1, interpolation / lerpDuration);
            sunImage.color = new Color(windColor.r, windColor.g, windColor.b, lerpValue);
            interpolation += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator DisplayControlIconCoroutine(float lifeDuration) {

        yield return new WaitForSeconds(2f);
        Image controlInfoImage = elementControlIcon.GetComponent<Image>();
        Color infoColor = controlInfoImage.color;
        controlInfoImage.color = new Color(controlInfoImage.color.r, controlInfoImage.color.g, controlInfoImage.color.b, 0);
        elementControlIcon.SetActive(true);
        float interpolation = 0;
        float lerpDuration = 1;
        while (interpolation < lerpDuration) {
            float lerpValue = Mathf.Lerp(0, 1, interpolation / lerpDuration);
            controlInfoImage.color = new Color(controlInfoImage.color.r, controlInfoImage.color.g, controlInfoImage.color.b, lerpValue);
            interpolation += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(lifeDuration);
        interpolation = 0;
        while (interpolation < lerpDuration) {
            float lerpValue = Mathf.Lerp(1, 0, interpolation / lerpDuration);
            controlInfoImage.color = new Color(controlInfoImage.color.r, controlInfoImage.color.g, controlInfoImage.color.b, lerpValue);
            interpolation += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        elementControlIcon.SetActive(false);
    }
}

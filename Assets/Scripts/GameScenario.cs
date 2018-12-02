using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScenario : MonoBehaviour {

    public enum ScenarioState {
        Enter, OneTile, CrossTiles, AllTiles
    }

    public MapManager mapManager;
    private ScenarioState state;
    private GameTimer timer;


    void Start() {
        state = ScenarioState.Enter;
        timer = GetComponent<GameTimer>();
    }

    void Update() {

        if (state == ScenarioState.Enter) {
            InitOneTileState();
            state = ScenarioState.OneTile;
        } else if (state == ScenarioState.OneTile) {
            TileData middleTileData = mapManager.Map[mapManager.mapSize / 2][mapManager.mapSize / 2].GetComponent<TileData>();
            if (middleTileData.Value >= 220) {
                InitCrossTilesState();
                state = ScenarioState.CrossTiles;
            }
        } else if (state == ScenarioState.CrossTiles) {
            TileData middleTileData = mapManager.Map[mapManager.mapSize / 2][mapManager.mapSize / 2].GetComponent<TileData>();
            TileData upTileData = mapManager.Map[mapManager.mapSize / 2][mapManager.mapSize / 2 + 1].GetComponent<TileData>();
            TileData downTileData = mapManager.Map[mapManager.mapSize / 2][mapManager.mapSize / 2 - 1].GetComponent<TileData>();
            TileData leftTileData = mapManager.Map[mapManager.mapSize / 2 - 1][mapManager.mapSize / 2].GetComponent<TileData>();
            TileData rightTileData = mapManager.Map[mapManager.mapSize / 2 + 1][mapManager.mapSize / 2].GetComponent<TileData>();
            if (middleTileData.Value >= 220 && upTileData.Value >= 220 && downTileData.Value >= 220 && leftTileData.Value >= 220 && rightTileData.Value >= 220) {
                InitAllTilesState();
                state = ScenarioState.AllTiles;
            }
        }

        //Timer over = Game over 
        if (timer.TimeOver) {
            EndGameStats.FINAL_SCORE = mapManager.ComputeScore();
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

    private IEnumerator CameraUnzoomCoroutine(float targetSize, float zoomDuration) {

        float initSize = Camera.main.orthographicSize;
        float interpolation = 0;
        while (Camera.main.orthographicSize != targetSize) {
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
        while ((interpolation / lerpDuration) != 1) {
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
        while ((interpolation / lerpDuration) != 1) {
            float lerpValue = Mathf.Lerp(0, 1, interpolation / lerpDuration);
            mapManager.SetTileTransparency(middleMap + 1, middleMap + 1, lerpValue);
            mapManager.SetTileTransparency(middleMap - 1, middleMap + 1, lerpValue);
            mapManager.SetTileTransparency(middleMap + 1, middleMap - 1, lerpValue);
            mapManager.SetTileTransparency(middleMap - 1, middleMap - 1, lerpValue);
            interpolation += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
    }
}

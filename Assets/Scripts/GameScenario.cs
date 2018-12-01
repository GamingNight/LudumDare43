using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScenario : MonoBehaviour {

    public enum State {
        Enter, OneTile, CrossedTiles, AllTiles
    }

    public MapManager mapManager;
    private State state;


    void Start() {
        state = State.Enter;
    }

    void Update() {

        if (state == State.Enter) {
            InitOneTileState();
            state = State.OneTile;
        } else if (state == State.OneTile) {
            TileData middleTileData = mapManager.Map[mapManager.mapSize / 2][mapManager.mapSize / 2].GetComponent<TileData>();
            if (middleTileData.RainValue >= 220) {
                InitCrossedTilesState();
                state = State.CrossedTiles;
            }
        }
    }

    private void InitOneTileState() {
        mapManager.HideAllTiles();
        mapManager.ShowOneTile();
    }

    private void InitCrossedTilesState() {
        StartCoroutine(CameraUnzoomCoroutine(5f, 2f));
        StartCoroutine(TilesActivationCoroutine(2f));
    }

    private IEnumerator CameraUnzoomCoroutine(float targetSize, float zoomDuration) {

        float initSize = Camera.main.orthographicSize;
        float interpolation = 0;
        while (Camera.main.orthographicSize != targetSize) {
            Camera.main.orthographicSize = Mathf.Lerp(initSize, targetSize, interpolation / zoomDuration);
            interpolation += 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator TilesActivationCoroutine(float lerpDuration) {

        mapManager.SetCrossedTilesTransparency(0);
        mapManager.ShowCrossedTiles();
        float interpolation = 0;
        while ((interpolation / lerpDuration) != 1) {
            mapManager.SetCrossedTilesTransparency(Mathf.Lerp(0, 1, interpolation / lerpDuration));
            interpolation += 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScenario : MonoBehaviour {

    public enum GameOverState {
        DEZOOM_CAMERA, DISPLAY_STATS, NONE
    }

    private GameOverState state;

    void Start() {

        state = GameOverState.DEZOOM_CAMERA;
    }

    void Update() {

        if(state == GameOverState.DEZOOM_CAMERA) {
            StartCoroutine(AdjustCameraCoroutine(2f, 8.5f, 5.5f));
        }
    }

    private IEnumerator AdjustCameraCoroutine(float lerpDuration, float targetCameraZoom, float targetXPosition) {

        float initZoom = Camera.main.orthographicSize;
        Vector3 initPos = Camera.main.transform.position;
        float step = 0;
        while(step < lerpDuration) {
            Camera.main.orthographicSize = Mathf.Lerp(initZoom, targetCameraZoom, step / lerpDuration);
            Camera.main.transform.position = new Vector3(Mathf.Lerp(initPos.x, targetXPosition, step / lerpDuration), initPos.y, initPos.z);
            step += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        state = GameOverState.DISPLAY_STATS;
    }
}

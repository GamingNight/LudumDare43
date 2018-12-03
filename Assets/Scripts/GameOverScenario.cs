using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScenario : MonoBehaviour {

    public enum GameOverState {
        DEZOOM_CAMERA, DISPLAY_STATS, NONE
    }

    public SpriteRenderer gameOverSprite;
    public SpriteRenderer scoreSprite;
    public SpriteRenderer bonusSprite;
    public SpriteRenderer retrySprite;
    public SpriteRenderer quitSprite;

    private GameOverState state;

    void Start() {

        state = GameOverState.DEZOOM_CAMERA;
    }

    void Update() {

        if (state == GameOverState.DEZOOM_CAMERA) {
            StartCoroutine(AdjustCameraCoroutine(2f, 8.5f, 5.5f));
            state = GameOverState.NONE;
        } else if (state == GameOverState.DISPLAY_STATS) {
            //StartCoroutine(DisplayHUD(2f));
            state = GameOverState.NONE;
        }
    }

    private IEnumerator AdjustCameraCoroutine(float lerpDuration, float targetCameraZoom, float targetXPosition) {

        float initZoom = Camera.main.orthographicSize;
        Vector3 initPos = Camera.main.transform.position;
        float step = 0;
        while (step < lerpDuration) {
            Camera.main.orthographicSize = Mathf.Lerp(initZoom, targetCameraZoom, step / lerpDuration);
            Camera.main.transform.position = new Vector3(Mathf.Lerp(initPos.x, targetXPosition, step / lerpDuration), initPos.y, initPos.z);
            step += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        state = GameOverState.DISPLAY_STATS;
    }

    private IEnumerator DisplayHUD(float lerpDuration) {

        float step = 0;
        while (step < lerpDuration) {
            float lerp = Mathf.Lerp(0, 1, step / lerpDuration);
            gameOverSprite.color = new Color(1, 1, 1, lerp);
            scoreSprite.color = new Color(1, 1, 1, lerp);
            bonusSprite.color = new Color(1, 1, 1, lerp);
            retrySprite.color = new Color(1, 1, 1, lerp);
            quitSprite.color = new Color(1, 1, 1, lerp);
            step += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData : MonoBehaviour {

    public enum StepName {
        RAIN, WIND, SUN
    }

    [System.Serializable]
    public struct TileStep {
        public StepName name;
        public Sprite sprite;
        public int value;
    }

    public TileStep[] steps;
    public int initValue = 100;

    public float dataDecreaseTimeMuliplier = 5f;
    public float dataIncreaseTimeMuliplier = 40f;

    public SpriteRenderer transitionSpriteRenderer;

    private SpriteRenderer spriteRenderer;
    private TileMouseDetector mouseDetector;
    private float value;
    public float Value { get { return value; } }

    private Sprite previousSprite;
    private Coroutine runningTransitionCoroutine;

    void Start() {
        value = initValue;
        spriteRenderer = GetComponent<SpriteRenderer>();
        mouseDetector = GetComponent<TileMouseDetector>();
        previousSprite = null;
        runningTransitionCoroutine = null;
    }

    void Update() {

        //Update Values
        if (mouseDetector.MouseIsOver && Input.GetMouseButton(0)) {
            value = value + (Time.deltaTime * dataIncreaseTimeMuliplier);
        } else {
            value = Mathf.Max(0, value - (Time.deltaTime * dataDecreaseTimeMuliplier));
        }

        //Update Sprites
        int i = 0;
        while (i < steps.Length && value > steps[i].value) {
            i++;
        }
        if (i == 0)
            i++;

        Sprite nextSprite = steps[i - 1].sprite;
        if (previousSprite == null)
            spriteRenderer.sprite = nextSprite;
        else if (previousSprite != nextSprite) {
            ProcessSpriteTransition(previousSprite, nextSprite);
        }
        previousSprite = nextSprite;
    }

    private void ProcessSpriteTransition(Sprite prevSprite, Sprite nextSprite) {
        if (runningTransitionCoroutine != null)
            StopCoroutine(runningTransitionCoroutine);
        runningTransitionCoroutine = StartCoroutine(SpriteTransitionCoroutine(0.5f, prevSprite, nextSprite));
    }
    private int callIndex = 0;
    private IEnumerator SpriteTransitionCoroutine(float lerpDuration, Sprite prevSprite, Sprite nextSprite) {
        Color tsrCol = transitionSpriteRenderer.color;
        Color srCol = spriteRenderer.color;

        transitionSpriteRenderer.color = new Color(tsrCol.r, tsrCol.g, tsrCol.b, srCol.a);
        transitionSpriteRenderer.sprite = prevSprite;

        spriteRenderer.color = new Color(srCol.r, srCol.g, srCol.b, tsrCol.a);
        spriteRenderer.sprite = nextSprite;

        float interpolation = 0;
        while (interpolation < lerpDuration) {
            float alpha = Mathf.Lerp(tsrCol.a, 1, interpolation / lerpDuration);
            spriteRenderer.color = new Color(srCol.r, srCol.g, srCol.b, alpha);
            float talpha = Mathf.Lerp(srCol.a, 0, interpolation / lerpDuration);
            transitionSpriteRenderer.color = new Color(srCol.r, srCol.g, srCol.b, talpha);
            interpolation += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        transitionSpriteRenderer.sprite = null;
        callIndex++;
    }
}

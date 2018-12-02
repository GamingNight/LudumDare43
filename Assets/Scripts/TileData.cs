using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData : MonoBehaviour {

    public enum StepName {
        RAIN, WIND, SUN
    }

    [System.Serializable]
    public class TileStep {
        public StepName name;
        public Sprite sprite;
        public int upValue;
        public int downValue;
    }

    public TileStep[] steps;
    public int initValue = 100;

    public float dataDecreaseTimeMuliplier = 5f;
    public float dataIncreaseTimeMuliplier = 40f;

    public SpriteRenderer transitionSpriteRenderer;

    private SpriteRenderer spriteRenderer;
    private float value;
    public float Value { get { return value; } }

    private TileStep lastStep;
    public TileStep LastStep { get { return lastStep; } }
    private Coroutine runningTransitionCoroutine;

    void Start() {
        value = initValue;
        spriteRenderer = GetComponent<SpriteRenderer>();
        lastStep = null;
        runningTransitionCoroutine = null;
    }

    //Called by the map manager
    public void UpdateValue(bool increase) {

        //Update Values
        if (increase) {
            value = value + (Time.deltaTime * dataIncreaseTimeMuliplier);
        } else {
            value = Mathf.Max(0, value - (Time.deltaTime * dataDecreaseTimeMuliplier));
        }

        //Check for step changing
        int i = 0;
        bool stopCondition = increase ? value > steps[i].upValue : value > steps[i].downValue;
        while (i < steps.Length && stopCondition) {
            i++;
            if (i < steps.Length)
                stopCondition = increase ? value > steps[i].upValue : value > steps[i].downValue;
        }
        if (i == 0)
            i++;

        TileStep nextStep = steps[i - 1];
        if (lastStep == null)
            spriteRenderer.sprite = nextStep.sprite;
        else if (lastStep != nextStep) {
            ProcessSpriteTransition(lastStep.sprite, nextStep.sprite);
        }
        lastStep = nextStep;
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

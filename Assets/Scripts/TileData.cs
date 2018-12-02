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

    private SpriteRenderer spriteRenderer;
    private TileMouseDetector mouseDetector;
    private float value;
    public float Value { get { return value; } }

    void Start() {
        value = initValue;
        spriteRenderer = GetComponent<SpriteRenderer>();
        mouseDetector = GetComponent<TileMouseDetector>();
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
        spriteRenderer.sprite = steps[i - 1].sprite;
    }
}

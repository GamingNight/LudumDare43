using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData : MonoBehaviour {

    public Sprite[] stepSprites;
    public int[] stepValues = { 100 };
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
        while (i < stepValues.Length && value > stepValues[i]) {
            i++;
        }
        spriteRenderer.sprite = stepSprites[i];
    }
}

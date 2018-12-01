using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData : MonoBehaviour {

    public Sprite[] rainSprites;
    public int[] rainSteps = { 100 };
    public int initRainValue = 100;

    public float dataDecreaseTimeMuliplier = 5f;
    public float dataIncreaseTimeMuliplier = 40f;

    private SpriteRenderer spriteRenderer;
    private TileMouseDetector mouseDetector;
    private float rainValue;
    public float RainValue { get { return rainValue; } }

    void Start() {
        rainValue = initRainValue;
        spriteRenderer = GetComponent<SpriteRenderer>();
        mouseDetector = GetComponent<TileMouseDetector>();
    }

    void Update() {

        //Update Values
        if (mouseDetector.MouseIsOver && Input.GetMouseButton(0)) {
            rainValue = rainValue + (Time.deltaTime * dataIncreaseTimeMuliplier);
        } else {
            rainValue = Mathf.Max(0, rainValue - (Time.deltaTime * dataDecreaseTimeMuliplier));
        }

        //Update Sprites
        int i = 0;
        while (i < rainSteps.Length && rainValue > rainSteps[i]) {
            i++;
        }
        spriteRenderer.sprite = rainSprites[i];
    }
}

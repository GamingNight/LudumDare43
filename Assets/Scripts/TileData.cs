using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData : MonoBehaviour {

    public Sprite[] rainSprites;
    public int initRainValue = 100;
    public int[] rainStages = { 100 };

    public float dataTimeMuliplier = 10f;

    private SpriteRenderer spriteRenderer;
    private TileMouseDetector mouseDetector;
    private float rainValue;

    void Start() {
        rainValue = initRainValue;
        spriteRenderer = GetComponent<SpriteRenderer>();
        mouseDetector = GetComponent<TileMouseDetector>();
    }

    void Update() {

        //Update Values
        if (mouseDetector.MouseIsOver && Input.GetMouseButton(0)) {
            rainValue = rainValue + (Time.deltaTime * dataTimeMuliplier * 15);
        } else {
            rainValue = Mathf.Max(0, rainValue - (Time.deltaTime * dataTimeMuliplier));
        }

        //Update Sprites
        int i = 0;
        while (i < rainStages.Length && rainValue > rainStages[i]) {
            i++;
        }
        spriteRenderer.sprite = rainSprites[i];
    }
}

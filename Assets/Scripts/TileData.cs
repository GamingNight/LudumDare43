using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData : MonoBehaviour {

    public Sprite[] rainSprites;
    public int initRainValue = 100;
    public int[] rainStages = { 100 };
    /// <summary>
    /// Data drops at rate *this per second*
    /// </summary>
    public float dataDropRate = 0.1f;

    private SpriteRenderer spriteRenderer;
    private float rainValue;

    void Start() {
        rainValue = initRainValue;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update() {

        rainValue = Mathf.Max(0, rainValue - (Time.deltaTime * dataDropRate));
        int i = 0;
        while (i < rainStages.Length && rainValue > rainStages[i]) {
            i++;
        }
        spriteRenderer.sprite = rainSprites[i];
    }
}

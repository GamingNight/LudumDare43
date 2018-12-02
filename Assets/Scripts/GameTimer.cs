using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour {

    public int totalTime;
    public Text timerText;

    private float currentTime;
    private bool timeOver;
    public bool TimeOver { get { return timeOver; } }

    void Start() {

        currentTime = 0;
        timeOver = false;
    }

    void Update() {

        currentTime = Mathf.Min(totalTime, currentTime + Time.deltaTime);

        float timerTextValue = ((int)(100 * (totalTime - currentTime))) / 100f;
        timerText.text = timerTextValue.ToString();
        if (currentTime == totalTime) {
            timeOver = true;
        }

    }
}

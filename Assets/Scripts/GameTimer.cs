using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour {

    public int totalTime;
    public Text timerText;

    private float currentTime;

	void Start () {

        currentTime = 0;
	}
	
	void Update () {

        currentTime = Mathf.Min(totalTime, currentTime + Time.deltaTime);

        float timerTextValue = ((int)(100 * (totalTime - currentTime)))/100f;
        timerText.text = timerTextValue.ToString();
        if(currentTime == totalTime) {
            Debug.Log("Game Over");
        }
        
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour {

    public Text scoreText;

    void Start() {

        scoreText.text = "Votre score est de " + (int) EndGameStats.FINAL_SCORE;
    }

    public void LoadMainScene() {
        SceneManager.LoadScene("main");
    }
}

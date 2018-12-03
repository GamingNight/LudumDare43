using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour {

    public Text rawScoreText;
    public Text bonusText;
    public Text totalScoreText;

    void Start() {

        int intRawScore = (int)EndGameStats.RAW_SCORE;
        rawScoreText.text = "Score: " + intRawScore;
        float roundedMultiplier = ((int)(10 * EndGameStats.HOMOGENEITY_MULTIPLIER)) / 10f;
		bonusText.text = "Territory Equality Bonus: x" + roundedMultiplier;
        totalScoreText.text = "Total Score: " + (intRawScore * roundedMultiplier);
    }

    public void LoadMainScene() {
        SceneManager.LoadScene("main");
    }
}

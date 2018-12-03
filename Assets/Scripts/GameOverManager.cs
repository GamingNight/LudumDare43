using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour {

    public Text rawScoreText;
    public Text bonusText;
    public Text totalScoreText;
    public GameObject gameOverTilePrefab;
    public float tileHeight = 4;
    public float tileWidth = 5;

    void Start() {

        int intRawScore = (int)EndGameStats.RAW_SCORE;
        rawScoreText.text = intRawScore.ToString();
        float roundedMultiplier = ((int)(10 * EndGameStats.HOMOGENEITY_MULTIPLIER)) / 10f;
        bonusText.text = "x" + roundedMultiplier;
        totalScoreText.text = (intRawScore * roundedMultiplier).ToString();

        GenerateSpriteMap();
    }

    public void LoadMainScene() {
        SceneManager.LoadScene("main");
    }

    public void Quit() {
        Application.Quit();
    }

    private void GenerateSpriteMap() {

        if (EndGameStats.SPRITES == null)
            return;

        float initPosX = 0;
        float initPosY = tileHeight * (EndGameStats.SPRITES.Length - 1) / 2;
        for (int i = 0; i < EndGameStats.SPRITES.Length; i++) {
            for (int j = 0; j < EndGameStats.SPRITES[i].Length; j++) {
                GameObject tileClone = Instantiate<GameObject>(gameOverTilePrefab);
                tileClone.gameObject.name = "Tile" + i.ToString() + j.ToString();
                tileClone.transform.position = new Vector3(initPosX - i * tileWidth / 2 + j * tileWidth / 2, initPosY - i * tileHeight / 2 - j * tileHeight / 2, 0);
                tileClone.GetComponent<SpriteRenderer>().sprite = EndGameStats.SPRITES[i][j];
                tileClone.GetComponent<SpriteRenderer>().sortingOrder = 2 + (i * EndGameStats.SPRITES.Length + j);
            }
        }
    }
}

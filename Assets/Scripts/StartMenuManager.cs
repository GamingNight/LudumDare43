using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour {

    public HighlightTextMenu startMenuHighlight;
    public HighlightTextMenu quitMenuHighlight;
    public Texture2D cursor;

    private void Start() {

        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
    }

    void Update() {

        if (Input.GetMouseButtonDown(0)) {
            if (startMenuHighlight.Selected)
                SceneManager.LoadScene("main");
            else if (quitMenuHighlight.Selected) {
                Application.Quit();
            }
        }
    }
}

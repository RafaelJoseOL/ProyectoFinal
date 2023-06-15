using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapController : MonoBehaviour {
    public GameObject[] levelButtons;
    DBController dbController;
    GameObject black;

    private void Start () {
        //black = GameObject.FindGameObjectWithTag("BlackScreen");
        //black.GetComponent<Animation>().Play();
        dbController = GameObject.FindGameObjectWithTag("DBController").GetComponent<DBController>();
        dbController.logged = false;
        dbController.stop = false;
        levelButtons[0].GetComponent<Button>().interactable = (bool)dbController.Level1;
        levelButtons[1].GetComponent<Button>().interactable = (bool)dbController.Level2;
        levelButtons[2].GetComponent<Button>().interactable = (bool)dbController.Level3;
    }

    public void LoadLevel (int level) {
        switch (level) {
            case 1:
                SceneManager.LoadScene("Level1");
                break;
            case 2:
                SceneManager.LoadScene("Level2");
                break;
            case 3:
                SceneManager.LoadScene("Level3");
                break;
            default:
                break;
        }
    }
}

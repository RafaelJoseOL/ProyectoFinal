using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PathCreation;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    public GameObject selectedTurret;
    public GameObject startButton;
    public GameObject currentTurret;
    public GameObject pauseMenu;

    public List<GameObject> turrets;
    public List<GameObject> turretInfo;

    public int startingGold;
    public int currentGold;
    public TextMeshProUGUI goldText;
    public GameObject healthBar;

    public List<GameObject> startingBlocks;
    public Lister lister = new Lister();
    public List<int> startingTimer;
    public List<int> cooldown;
    public List<PathCreator> paths;
    public List<bool> spawnerEmpty;
    public bool spawnFinished;
    public int enemyCounter;
    public List<GameObject> turretsPlaced;

    public GameObject statsPanel;
    public TextMeshProUGUI[] statsText;
    public Button upgradeButton;

    string currentScene;
    public GameObject winPanel;
    public GameObject losePanel;

    DBController dbController;
    public GameObject firstSpawn;

    private void Awake () {
        IncreaseGold(startingGold);
        SetTurretValues();
        statsPanel = GameObject.FindGameObjectWithTag("Upgrade");
        statsText = new TextMeshProUGUI[4];
        for (int i = 0; i < 4; i++) {
            statsText[i] = statsPanel.transform.GetChild(2).transform.GetChild(i).GetComponent<TextMeshProUGUI>();
        }
        upgradeButton = GameObject.FindGameObjectWithTag("UpgradeButton").GetComponent<Button>();
        statsPanel.SetActive(false);
        dbController = GameObject.FindGameObjectWithTag("DBController").GetComponent<DBController>();
        currentScene = SceneManager.GetActiveScene().name;
        healthBar = GameObject.FindGameObjectWithTag("HealthBar");
        //firstSpawn.GetComponent<Animation>().Play();
    }

    public void SetTurretValues () {
        for (int i = 0; i < turretInfo.Count; i++) {
            turretInfo[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text += " " + turrets[i].GetComponent<Turret>().cost[0].ToString();
            turretInfo[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text += " " + turrets[i].GetComponent<Turret>().damage[0].ToString();
            turretInfo[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text += " " + turrets[i].GetComponent<Turret>().cooldown.ToString();
            turretInfo[i].transform.GetChild(3).GetComponent<TextMeshProUGUI>().text += " " + turrets[i].GetComponent<Turret>().range.ToString();
        }
    }

    public void SelectTurret (GameObject turret) {
        selectedTurret = turret;
        goldText = GameObject.FindGameObjectWithTag("GoldText").GetComponent<TextMeshProUGUI>();
        goldText.text = currentGold.ToString();
    }

    public void HideRange () {
        for (int i = 0; i < turretsPlaced.Count; i++) {
            turretsPlaced[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        statsPanel.SetActive(false);
    }

    public void ShowRange (GameObject turret) {
        turret.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void ShowUpgradePanel (bool show) {
        if (show) {
            statsPanel.SetActive(true);
            statsPanel.transform.GetChild(0).GetComponent<Image>().sprite = currentTurret.GetComponent<Turret>().turretImage;
            int currentLevel = currentTurret.GetComponent<Turret>().currentLevel;
            if (currentLevel == 2) {
                statsText[0].text = " - ";
                statsText[1].text = currentTurret.GetComponent<Turret>().damage[currentLevel].ToString();
                upgradeButton.interactable = false;
            } else {
                statsText[0].text = currentTurret.GetComponent<Turret>().cost[currentLevel + 1].ToString();
                statsText[1].text = "+" + (currentTurret.GetComponent<Turret>().damage[currentLevel + 1] -
                    currentTurret.GetComponent<Turret>().damage[currentLevel + 0]).ToString() + " ("
                    + currentTurret.GetComponent<Turret>().damage[currentLevel + 1] + ")";
                upgradeButton.interactable = true;
            }
            //statsText[2].text = currentTurret.GetComponent<Turret>().cooldown + " --> " + currentTurret.GetComponent<Turret>().cooldown;
            //statsText[3].text = currentTurret.GetComponent<Turret>().range + " --> " + currentTurret.GetComponent<Turret>().range;
        } else {
            statsPanel.SetActive(false);
        }
    }

    public void UpgradeTurret () {
        int cost = currentTurret.GetComponent<Turret>().cost[currentTurret.GetComponent<Turret>().currentLevel + 1];
        if (currentGold >= cost) {
            currentTurret.GetComponent<Turret>().currentLevel++;
            int currLevel = currentTurret.GetComponent<Turret>().currentLevel;
            currentTurret.transform.GetChild(currLevel).gameObject.SetActive(false);
            currentTurret.transform.GetChild(currLevel + 1).gameObject.SetActive(true);
            currentTurret.GetComponent<Turret>().shootingSpot = currentTurret.transform.GetChild(currLevel + 1).transform.GetChild(0).transform;
            if (!currentTurret.GetComponent<Turret>().turret1) {
                currentTurret.GetComponent<Turret>().head = currentTurret.transform.GetChild(currLevel + 1).transform.GetChild(1).transform;
            }
            IncreaseGold(-cost);
            ShowUpgradePanel(false);
            ShowUpgradePanel(true);
        } else {
            LowGold();
        }
    }

    public void IncreaseGold (int gold) {
        currentGold += gold;
        goldText.text = currentGold.ToString();
    }

    public void StartLevel () {
        for (int i = 0; i < startingBlocks.Count; i++) {
            EnemySpawner enemySpawner = startingBlocks[i].GetComponent<EnemySpawner>();
            List<GameObject> enemies = lister.enemiesList[i].list;
            enemySpawner.enemiesList = enemies;
            enemySpawner.startingTimer = startingTimer[i];
            enemySpawner.cooldown = cooldown[i];
            enemySpawner.path = paths[i];
            enemySpawner.Begin();
            spawnerEmpty.Add(false);
        }
        startButton.SetActive(false);
        ChangeSpeed(1);
    }

    public void CheckSpawners () {
        if (spawnerEmpty.All(b => b)) {
            spawnFinished = true;
        }
    }

    public void CheckEnemies () {
        if (spawnFinished) {
            if (enemyCounter == 0) {
                if (healthBar.GetComponent<Slider>().value <= 0) {
                    OpenLosePanel();
                } else {
                    OpenWinPanel();
                }
            }
        }
    }

    public void DamagePlayer (int damage) {
        healthBar.GetComponent<Slider>().value -= damage;
        if (healthBar.GetComponent<Slider>().value <= 0) {
            OpenLosePanel();
        }
    }

    public void LowGold () {
        goldText.GetComponent<Animation>().Play();
    }

    public void Pause (bool pause) {
        pauseMenu.SetActive(pause);
        if (pause) {
            Time.timeScale = 0;
        } else {
            Time.timeScale = 1;
        }
    }

    public void ChangeSpeed (int speed) {
        Time.timeScale = speed;
    }

    public void OpenWinPanel () {
        if (currentScene == "Level1") {
            dbController.SaveData(2);
            dbController.Level2 = true;
        } else if (currentScene == "Level2") {
            dbController.SaveData(3);
            dbController.Level3 = true;
        }
        Time.timeScale = 0;
        winPanel.SetActive(true);
        if (currentScene == "Level3") {
            GameObject.FindGameObjectWithTag("WinText").GetComponent<TextMeshProUGUI>().text = "¡Enhorabuena por completar el juego, "
                + dbController.username + "!";
        }
    }

    public void OpenLosePanel () {
        Time.timeScale = 0;
        losePanel.SetActive(true);
    }

    public void NextLevel (string scene) {
        SceneManager.LoadScene(scene);
    }

    public void ResetLevel () {
        SceneManager.LoadScene(currentScene);
    }

    public void ExitMap () {
        SceneManager.LoadScene("Map");
    }

    public void ExitMenu () {
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitDesktop () {
        Application.Quit();
    }
}

[System.Serializable]
public class List {
    public List<GameObject> list;
}
[System.Serializable]
public class Lister {
    public List<List> enemiesList;
}

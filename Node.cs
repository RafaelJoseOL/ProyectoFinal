using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour {

    public Color hoverColor;
    private Color initColor;

    private Renderer rend;
    public GameObject turret;
    private GameController gameController;
    public Transform turretPosition;

    //private bool isNodeClicked = false;
    private GameObject blueprintInstance;

    private void Awake () {
        rend = gameObject.GetComponent<Renderer>();
        initColor = rend.material.color;
        turretPosition = gameObject.transform.parent.GetChild(1);
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    private void OnMouseEnter () {
        if (!EventSystem.current.IsPointerOverGameObject() && turret == null) {
            rend.material.color = hoverColor;
            //isNodeClicked = true;
            blueprintInstance = Instantiate(gameController.selectedTurret, turretPosition);
            gameController.ShowRange(blueprintInstance);
            blueprintInstance.GetComponent<Turret>().blueprint = true;
        }
    }

    private void OnMouseExit () {
        rend.material.color = initColor;
        //isNodeClicked = false;
        Destroy(blueprintInstance);
    }

    private void OnMouseUpAsButton () {
        if (!EventSystem.current.IsPointerOverGameObject()) {
            gameController.HideRange();
            if (turret != null) {
                gameController.ShowRange(turret);
                gameController.currentTurret = turret;
                gameController.ShowUpgradePanel(true);
            } else {
                gameController.ShowUpgradePanel(false);
                if (gameController.currentGold >= gameController.selectedTurret.GetComponent<Turret>().
                        cost[gameController.selectedTurret.GetComponent<Turret>().currentLevel]) {
                    turret = Instantiate(gameController.selectedTurret, turretPosition);
                    gameController.IncreaseGold(-gameController.selectedTurret.GetComponent<Turret>().
                        cost[gameController.selectedTurret.GetComponent<Turret>().currentLevel]);
                    gameController.currentTurret = turret;
                    gameController.ShowRange(turret);
                    gameController.turretsPlaced.Add(turret);
                    gameController.ShowUpgradePanel(true);
                } else {
                    gameController.LowGold();
                }
            }
        }
    }
}

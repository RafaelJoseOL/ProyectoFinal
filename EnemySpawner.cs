using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;

public class EnemySpawner : MonoBehaviour {
    public List<GameObject> enemiesList;
    public int startingTimer;
    public int cooldown;
    public PathCreator path;
    GameController gameController;

    public void Begin () {
        InvokeRepeating("Spawn", startingTimer, cooldown);
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public void Spawn () {
        int num = enemiesList.Count;
        GameObject enemy = Instantiate(enemiesList[num - 1], transform.position, transform.rotation) as GameObject;
        enemy.GetComponent<PathFollower>().pathCreator = path;
        enemiesList.RemoveAt(num - 1);
        gameController.enemyCounter++;
        if (num == 1) {
            CancelInvoke("Spawn");            
            for (int i = 0; i < gameController.spawnerEmpty.Count; i++) {
                if (gameController.spawnerEmpty[i] == false) {
                    gameController.spawnerEmpty[i] = true;
                    gameController.CheckSpawners();
                    return;
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using PathCreation.Examples;

public class Enemy : MonoBehaviour {
    public float maxHealth;
    public float currentHealth;
    public int damage;
    public float speed;
    public int gold;
    GameController gameController;
    PathFollower path;
    Animator anim;

    void Awake () {
        currentHealth = maxHealth;
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        path = this.GetComponent<PathFollower>();
        path.speed = speed;
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter (Collider other) {
        if (other.gameObject.tag == "EndBlock") {
            gameController.enemyCounter--;
            gameController.CheckEnemies();
            gameController.DamagePlayer(damage);
            Destroy(this.gameObject);
        }
    }

    public void TakeDamage (float damage) {
        currentHealth -= damage;
        if (currentHealth <= 0) {
            gameController.IncreaseGold(gold);
            gameController.enemyCounter--;
            gameController.CheckEnemies();
            Destroy(this.gameObject);
        }
    }
}

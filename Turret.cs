using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour {
    public Transform target;
    public float[] damage;
    public float range;
    public float cooldown;
    public int[] cost;
    public GameObject ammo;
    public int currentLevel;
    public Transform shootingSpot;
    public Transform head;
    public bool turret1;
    public Sprite turretImage;
    public AudioSource shootSound;
    public bool blueprint;

    private void Awake () {
        InvokeRepeating("UpdateTarget", 1f, 2f);
        InvokeRepeating("Shoot", 1f, cooldown);
        //transform.GetChild(currentLevel + 1).gameObject.SetActive(true);
        //transform.GetChild(currentLevel + 2).gameObject.SetActive(false);
        //transform.GetChild(currentLevel + 3).gameObject.SetActive(false);
        shootingSpot = transform.GetChild(currentLevel + 1).transform.GetChild(0).transform;
        shootSound = GetComponent<AudioSource>();
        if (!turret1) {
            head = transform.GetChild(currentLevel + 1).transform.GetChild(1).transform;
        }
    }

    private void Update () {
        if (!turret1 && target != null) {
            Vector3 direction = target.position - head.position;
            direction.x = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            head.rotation = Quaternion.Lerp(head.rotation, targetRotation, 5f * Time.deltaTime);
        }        
    }

    private void OnDrawGizmosSelected () {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    private void UpdateTarget () {
        if (!blueprint) {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            float shortestDistance = Mathf.Infinity;
            GameObject nearestEnemy = null;
            foreach (GameObject enemy in enemies) {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy < shortestDistance) {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemy;
                }
            }

            if (nearestEnemy != null && shortestDistance <= range) {
                target = nearestEnemy.transform;
            } else {
                target = null;
            }
        }       
    }

    private void Shoot () {
        if (target != null && !blueprint) {
            GameObject bullet = Instantiate(ammo, shootingSpot.position, Quaternion.identity);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.SetTarget(target, damage[currentLevel]);
            shootSound.Play();
        }
    }
}

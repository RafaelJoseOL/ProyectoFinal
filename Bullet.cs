using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target; 
    public float speed = 10f;
    float damage;

    public void SetTarget (Transform newTarget, float dmg) {
        target = newTarget;
        damage = dmg;
    }

    private void Update () {
        if (target == null) {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.1f) {
            target.GetComponent<Enemy>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}

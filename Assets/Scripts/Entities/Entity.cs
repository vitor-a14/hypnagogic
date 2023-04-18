using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int life;
    public bool alive = true;

    public void TakeHit(int damage)
    {
        life -= damage;

        if(life <= 0)
            Die();
    }

    public virtual void Die()
    {
        Destroy(GetComponent<Rigidbody>());
        Destroy(GetComponent<Animator>(), 5);
        Destroy(this, 1);
        Destroy(gameObject, 10);
        alive = false;

        foreach(Collider col in GetComponentsInChildren<Collider>())
            Destroy(col);
    }
}

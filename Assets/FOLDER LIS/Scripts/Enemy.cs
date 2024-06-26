using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void EnemyDeath();
public class Enemy : Unit
{
    private float _health = 3;

    public event EnemyDeath OnEnemyDeath;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Attacked()
    {
        _health = _health - 1;
        if(_health <= 0)
        {
            Die();
        }
    }
  
    public void Die()
    {
        Debug.Log("Enemy died. Invoking event..");
        OnEnemyDeath?.Invoke();
        IsDead = true;
    }
    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    public override void Behave()
    {
        
    }
}

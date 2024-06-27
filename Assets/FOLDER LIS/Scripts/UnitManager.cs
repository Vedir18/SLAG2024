using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public int MaxAttackers = 2;
    private List<EnemyData> _enemies = new List<EnemyData>();

    private List<AllyData> _allies = new List<AllyData>();

    private class EnemyData
    {
        public Enemy Enemy;
        public int CurrentAttackers;
        public EnemyData(Enemy enemy)
        {
            Enemy = enemy;
            CurrentAttackers = 0;
        }

        public void AddAttacker()
        {
            CurrentAttackers = CurrentAttackers + 1;
        }
    }
    private class AllyData
    {
        public Ally Ally;
        public int CurrentAttackers;
        public AllyData(Ally ally)
        {
            Ally = ally;
            CurrentAttackers = 0;
        }

        public void AddAttacker()
        {
            CurrentAttackers = CurrentAttackers + 1;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Object[] objectsEnemies = FindObjectsOfType<Enemy>();
        foreach (Object obj in objectsEnemies)
        {
            _enemies.Add(new EnemyData((Enemy)obj));
        }
        for(int i=0; i< _enemies.Count; i++)
        {
            _enemies[i].Enemy.OnEnemyDeath += RemoveDeadEnemy;
    }

        Object[] objectsAllies = FindObjectsOfType<Ally>();
        foreach (Object obj in objectsAllies)
        {
            _allies.Add(new AllyData((Ally)obj));
        }
        for (int i = 0; i < _allies.Count; i++)
        {
            _allies[i].Ally.OnAllyDeath += RemoveDeadAlly;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Enemy GetNearestEnemy(Vector3 Location)
    {
        // TODO: check for errors (out of array bounds, no enemies left, etc)
        float minDistance = 999;
        int saved = -1;
        int secondChoice = -1;
        for(int i=0; i<_enemies.Count; i++)
        {
            float currentDistance = Vector3.Distance(Location, _enemies[i].Enemy.transform.position);
            if (currentDistance < minDistance)
            {
                if(_enemies[i].CurrentAttackers < 2)
            {
                    saved = i;
                minDistance = currentDistance;
                }
                secondChoice = i;
            }
            }
        
        if (saved == -1)
        {
            saved = secondChoice;
        }
        _enemies[saved].AddAttacker();

        return _enemies[saved].Enemy;
    }
    
    public void RemoveDeadEnemy()
    {
        for (int i = 0; i < _enemies.Count; i++)
        {
            Debug.Log("enemy[" + i + "]" + _enemies[i]);
        }
        Debug.Log("and after");
        for (int i = 0; i < _enemies.Count; i++)
        {
            if (_enemies[i].Enemy.IsDead)
            {
                Enemy enemyToDestroy = _enemies[i].Enemy;
                _enemies.RemoveAt(i);
                enemyToDestroy.DestroyEnemy();
            }
        }
        for (int i = 0; i < _enemies.Count; i++)
        {
            Debug.Log("enemy[" + i + "]" + _enemies[i]);
        }
    }
    public void RemoveDeadAlly()
    {
        for (int i = 0; i < _allies.Count; i++)
        {
            if (_allies[i].Ally.IsDead)
            {
                Ally allyToDestroy = _allies[i].Ally;
                _allies.Remove(_allies[i]);
            }
        }
    }

}

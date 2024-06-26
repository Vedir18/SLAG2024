using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public int MaxAttackers = 2;
    private List<EnemyData> _enemies = new List<EnemyData>();
    private struct EnemyData
    {
        public EnemyData(Enemy enemy)
        {
            Enemy = enemy;
            CurrentAttackers = 0;
        }

        public Enemy Enemy;
        public int CurrentAttackers;

        public void AddAttacker()
        {
            CurrentAttackers++;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Object[] objects = FindObjectsOfType<Enemy>();
        foreach (Object obj in objects)
        {
            _enemies.Add(new EnemyData((Enemy)obj));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Transform GetNearestEnemy(Vector3 Location)
    {
        float minDistance = 999;
        int saved = 0;
        for(int i=0; i<_enemies.Count; i++)
        {
            float currentDistance = Vector3.Distance(Location, _enemies[i].Enemy.transform.position);
            if (currentDistance < minDistance && _enemies[i].CurrentAttackers < 2)
            {
                minDistance = currentDistance;
                saved = i;
                Debug.Log("i: " + i);
            }
        }
        _enemies[saved].AddAttacker();
        return _enemies[saved].Enemy.transform;
    }

}

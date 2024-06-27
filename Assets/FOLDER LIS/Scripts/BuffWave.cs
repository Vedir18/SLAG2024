using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffWave : EnemyWave
{
    protected override void DoContact(Unit unit)
    {
        if (unit.gameObject.TryGetComponent<Enemy>(out Enemy enemy))
        {
            if (changeBrave) { enemy.ModifyBrave(5); }
            if (changeDedicated) { enemy.ModifyDedicated(5); }
        }
    }
}

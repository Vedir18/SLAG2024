using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffWave : EnemyWave
{


    protected override void DoContact(Unit unit)
    {
        if (unit.gameObject.TryGetComponent<Ally>(out Ally ally))
        {
            if(changeBrave) { ally.ModifyBrave(-20); }
            else { ally.ModifyDedicated(-20);}
        }
    }
}

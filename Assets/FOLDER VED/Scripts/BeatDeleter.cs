using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatDeleter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        SingleBeat sb= other.GetComponent<SingleBeat>();
        if (sb != null) sb.Fail();
    }
}

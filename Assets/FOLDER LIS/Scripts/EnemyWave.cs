using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWave : MonoBehaviour
{
    // Start is called before the first frame update
    private float _age = 0;
    private float _lifetime;
    private float _maxRadius;

    private void Start()
    {
        Initialize(Vector3.zero, 5, 10);
    }

    public void Initialize(Vector3 position, float lifetime, float maxRadius)
    {
        transform.position = position;
        _lifetime = lifetime;
        _maxRadius = maxRadius;
    }

    // Update is called once per frame
    void Update()
    {
        _age += Time.deltaTime;
        float i = Mathf.InverseLerp(0, _lifetime, _age);
        transform.localScale = Vector3.one * i * _maxRadius;
        if(i >= 1)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
}

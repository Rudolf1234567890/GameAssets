using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifetimeDestroy : MonoBehaviour
{
    public float lifetime = 3f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Global : MonoSingleton<Global>
{
    public PlayerController PlayerController;

    void Awake()
    {
        PlayerController = gameObject.GetComponent<PlayerController>();
    }
}

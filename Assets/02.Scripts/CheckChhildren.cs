using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckChhildren : MonoBehaviour
{
    public GameManager gameManager; 

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>(); 
        }
    }

    private void Update()
    {
        if (transform.childCount == 0)
        {
            gameManager.GameClear(); 
        }
    }
}

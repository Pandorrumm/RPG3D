using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillCounter : MonoBehaviour
{
    private int killCount = 0; //счётчик убийств
    private Text countText;

    private void Awake()
    {
        countText = GetComponent<Text>();

        countText.text = killCount.ToString(); // в тексте в Unity что будет
    }

    private void OnEnemyDeath()  
    {
        killCount++;
        countText.text = killCount.ToString();
    }

    private void OnEnable()  //подписываемся
    {
         Enemy.OnDeath += OnEnemyDeath;
    }

    private void OnDisable() //отписываемся
    {
         Enemy.OnDeath += OnEnemyDeath;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstAid : MonoBehaviour  //первая помощь (аптечка)
{
    private readonly string PLAYER_TAG = "Player";
    [SerializeField]
    private float aidValue = 100f; // сколько добавляется жизней
    public static System.Action<FirstAid> Consume; // типа когда использована уже

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Spawn(Vector3 pos)
    {
        transform.position = pos + transform.up*3 + transform.forward;
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == PLAYER_TAG) //если в триггерную коллизию вошёл герой
        {
            gameObject.SetActive(false);

            other.gameObject.GetComponent<PlayerController>().Heal(aidValue); //запускаем метод Heal

            if(Consume != null) //если кто то подписан 
            {
                Consume(this); //this - типа сама аптечка
            }               
        }
    }

}

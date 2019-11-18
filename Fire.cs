using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fire : MonoBehaviour
{
    public Slider slider;
    [SerializeField]
    private int fireHealth = 100; //здоровье костра
    private PlayerController player;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        InvokeRepeating("MinusFireHealth", 2f, 2f);
    }

   
    void Update()
    {
        //if(Input.GetMouseButtonUp(1))
        //{
        //    fireHealth -= 10;
        //    slider.value = fireHealth;

        //    if(fireHealth <= 0)
        //    {

        //    }
        //}
    }

    void MinusFireHealth()
    {
        fireHealth -= 1;
        slider.value = fireHealth;
        if (fireHealth <= 0)
        {
            player.Die();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Item")
        {
            Debug.Log("Прибавили +50 огню и удалили доски");
            fireHealth += 50;
            Destroy(other.gameObject);
        }
    }
}

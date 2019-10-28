using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    
    public string sprite; // адрес картинки в Resources/Sprite
   public string prefab; // адрес оружия в Resources/Prefab

    //List<Item> list;
    //private readonly string PLAYER = "Player"; //тег героя нашего
    //[SerializeField]
    //PlayerController player; //игрок, за которым охотятся враги

    //void Start()
    //{
    //    list = new List<Item>();
    //}

    //public void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == PLAYER && !player.IsDead)
    //    {
    //        //Destroy(this.gameObject);
    //        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        Ray ray = new Ray(transform.position, gameObject.transform.position - transform.position);
    //        RaycastHit hit;
    //        if (Physics.Raycast(ray, out hit))
    //        {
    //            Item item = hit.collider.GetComponent<Item>();
    //            if (item != null) //если на объекте есть item
    //            {
    //                list.Add(item);  // то добавляем его в список
    //                Destroy(hit.collider.gameObject); // и удаляем со сцены
    //            }
    //        }
    //        Debug.Log("Предмет взят");
    //    }
    //}
}

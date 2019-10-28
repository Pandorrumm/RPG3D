using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Inventory : MonoBehaviour
{

    List<Item> list;  //список для хранения предметов
    [SerializeField]
    private GameObject inventory;
    private readonly string ITEM = "Item"; //тег героя нашего
    [SerializeField]
    PlayerController player; //игрок, за которым охотятся враги
    public GameObject container; // Image клетки в инвентаре

    void Start()
    {
        list = new List<Item>();
    }

    void Update()
    {
        //сбор предметов по клику мышки
        //if (Input.GetMouseButtonUp(1))
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;
        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        Item item = hit.collider.GetComponent<Item>();
        //        if (item != null) //если на объекте есть item
        //        {
        //            list.Add(item);  // то добавляем его в список
        //            Destroy(hit.collider.gameObject); // и удаляем со сцены
        //        }
        //    }
        //}
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == ITEM)
        {
           
                Item item = other.gameObject.GetComponent<Item>();
                if (item != null) //если на объекте есть item
                {
                    list.Add(item);  // то добавляем его в список
                    Destroy(other.gameObject); // и удаляем со сцены
                }
            
            Debug.Log("Предмет взят");
        }
    }

    public void InventoryButton()
    {
        if (inventory.activeSelf) //если инвентарь сейчас активен
        {
            inventory.SetActive(false);

            for (int i = 0; i < inventory.transform.childCount; i++)
            {
                if(inventory.transform.GetChild(i).transform.childCount > 0) //если что то есть в ячейке - удаляем
                {
                    Destroy(inventory.transform.GetChild(i).transform.GetChild(0).gameObject);
                }
            }
        }
        else
        {
            inventory.SetActive(true);
            int count = list.Count; // длина нашего списка
            for(int i = 0; i< count; i++)
            {
                Item it = list[i];

                if(inventory.transform.childCount >= i) //хватает ли ячеек
                {
                    GameObject img = Instantiate(container); //создаём изображение 
                    img.transform.SetParent(inventory.transform.GetChild(i).transform); // делаем изображение дочерним к ячейке
                    img.GetComponent<Image>().sprite = Resources.Load<Sprite>(it.sprite);
                }
                else break;   
                   
            }
        }
    }
}

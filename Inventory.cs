using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Inventory : MonoBehaviour
{

    List<Item> list;  //список для хранения предметов
    [SerializeField]
    private GameObject inventory;
    private readonly string ITEM = "Item"; //тег предмета нашего
    [SerializeField]
    PlayerController player; //игрок, за которым охотятся враги
    public GameObject container; // Image клетки в инвентаре

    void Start()
    {
        list = new List<Item>();
        player = GetComponent<PlayerController>();
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
        if (other.gameObject.tag == ITEM /*&& inventory.transform.childCount <= 4*/)
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
            Debug.Log("длина инвенторя" + count);
            for (int i = 0; i < count; i++)
            {
                Item it = list[i];

                if(inventory.transform.childCount >= i) //хватает ли ячеек
                {
                    Debug.Log("Сколько ячеек" + inventory.transform.childCount);

                    GameObject img = Instantiate<GameObject>(container); //создаём изображение 
                    img.transform.SetParent(inventory.transform.GetChild(i).transform); // делаем изображение дочерним к ячейке
                    img.GetComponent<Image>().sprite = Resources.Load<Sprite>(it.sprite);

                    //добавляем на ячейки компонент Button, что по нажатию можно было бы вернуть инвентарь обратно
                    //img.AddComponent<Button>().onClick.AddListener(() => Remove(it, img));

                    img.GetComponent<DragInventory>().item = it;
                }
                //else
                //{
                //    var obj = GameObject.FindGameObjectsWithTag("Item"); //находим все объекты по тегу
                //    Debug.Log("Кол-во объектов "+ obj.Length);

                //    for (int y = 0; y < obj.Length; y ++)
                //    {
                //        obj[i].gameObject.tag = "NoItem";
                //    }                        
                //}
                else break;   
                   
            }
        }
    }

    void Use(DragInventory drag) //использование предмета
    {
        Item it = drag.item;
        if(drag.item.type == "FirstAid")
        {
            //прибавляем здоровье
            player.Heal(100);
            Debug.Log("использовалди аптечку");
            // Remove(drag);
            //Destroy(drag.gameObject);
        }
        else if(drag.item.type == "Hand")
        {
            HandItem myItem = Instantiate<GameObject>(Resources.Load<GameObject>(drag.item.prefab)).GetComponent<HandItem>();
            player.AddHand(myItem);
        }
        else if(drag.item.type == "Board")
        {
            Debug.Log("выкидываем дрова");
            Remove(drag);
        }
        list.Remove(drag.item);//удаляем из рюкзака
        Destroy(drag.gameObject); 
    }

    void Remove(DragInventory drag) //удаляем из инвенторя
    {
        Item it = drag.item;

        //добавляем объект в мир
        GameObject newO = Instantiate<GameObject>(Resources.Load<GameObject>(it.prefab));

        //куда создаём                                 (вперёд на метр)      (вверх на метр)
        newO.transform.position = transform.position + transform.forward * 2 + transform.up/2;

        //удаляем объект из инвенторя
        Destroy(drag.gameObject);

        //удаляем объект из списка
        list.Remove(it);
    }
}

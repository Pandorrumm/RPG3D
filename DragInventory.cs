using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

 //Что бы В инвенторе менять местами предметы и доставать их

public class DragInventory : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public Transform canvas;
    public Transform old;

    private GameObject player;

    public Item item;

    void Start()
    {
        canvas = GameObject.Find("Canvas").transform;
        player = GameObject.FindGameObjectWithTag("Player");
    }   
    

    public void OnBeginDrag(PointerEventData eventData) //начало перетаскивания
    {
        //запоминаем старого родителя и перемещаем его в канвас, меняем родителя
        old = transform.parent;
        transform.SetParent(canvas);

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) // процесс перетаскивания
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) //завершение перетаскивания
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        //если наш родитель canvas
        if (transform.parent == canvas)
        {
            //возвращаем назад transform
            transform.SetParent(old);
        }
    }

    public void OnPointerClick(PointerEventData eventData) //когда кликнули на него
    {
        //если нажимаем на левою кнопку мыши, то Используем предмет
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            player.BroadcastMessage("Use", this);
        }
        else
        {
            //рассылаем событие, хотим передать Remove
            //если на игроке есть что то с методом Remove, он будет вызван
            player.BroadcastMessage("Remove", this);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropInventory : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        //меняем нашему Drag родителя
        DragInventory drag = eventData.pointerDrag.GetComponent<DragInventory>();
        if(drag != null)
        {
            //если есть ребёнок у нашего transform, то будем их менять местами(что бы друг на друга не налепливались иконки в инвентаре при перетаскивании в одну клетку)
            if(transform.childCount > 0)
            {
                transform.GetChild(0).SetParent(drag.old);
            }

            //меняем transform на нам нужный
            drag.transform.SetParent(transform);
        }
    }
}

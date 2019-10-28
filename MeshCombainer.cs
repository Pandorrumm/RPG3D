using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class MeshCombainer : MonoBehaviour  //склеиваем все Элементы -горы, землю, деревья
{                                           //вешаем на папку прям с деревьями и горами (Location)
                                            // убрать у всех Static
    private MeshRenderer renderer;
    private Mesh mesh;

    [SerializeField]
    private Material material;

    [SerializeField]
    private string meshName;  // называем папку просто, куда сохранится

    [ContextMenu("Combine")]
    void Combine()
    {
        renderer = GetComponent<MeshRenderer>();
        mesh = transform.GetComponent<MeshFilter>().sharedMesh;

        //собираем все элементы окружения(земля, горы, деревья) из папок соответстующих
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();

        //создаём массив, размер = кол-во найденных элементов
        CombineInstance[] combines = new CombineInstance[meshFilters.Length];

        int i = 0;
        while(i < meshFilters.Length)
        {
            combines[i].mesh = meshFilters[i].sharedMesh;
            combines[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.active = false;

            i++;
        }
        //делаем комбайн
        mesh = new Mesh();
        mesh.CombineMeshes(combines);

        //отключаем объект родителя после успешной операции
        transform.gameObject.active = true;

        gameObject.AddComponent<MeshCollider>();

        renderer.material = Material.Instantiate(material);

        //сохраняем через скрипт MeshSaveEditor
        MeshSaveEditor.SaveMesh(mesh, meshName, false, false);
    }
  
}

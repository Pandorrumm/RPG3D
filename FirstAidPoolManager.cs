using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstAidPoolManager : MonoBehaviour
{
    private static FirstAidPoolManager instance;  //делаем приватный доступ к PoolManager

    [SerializeField]
    private GameObject firstAidPrefab; //префаб аптечки

    private List<FirstAid> avaliable; //cписок свободных аптечек (типа пустой карман)

    private List<FirstAid> busy; //список занятых аптечек (типа полный карман)

    private const int MIN_POOL_SIZE = 3; //минимальный размер кармана

    public static FirstAidPoolManager Instance //св-во публичного доступа к ресурсам компонента
                                               //типа instance у нас приватный же, а мы к нему через Instance подойдём
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
        avaliable = new List<FirstAid>();
        busy = new List<FirstAid>();

        for(int i = 0; i < MIN_POOL_SIZE; i++)
        {
            AddAidToPool();
        }
    }

    private void AddAidToPool()
    {
        avaliable.Add(Instantiate(firstAidPrefab).GetComponent<FirstAid>());
    }

    private void OnEnable()
    {
        FirstAid.Consume += OnConsume;
    }

    private void OnDisable()
    {
        FirstAid.Consume -= OnConsume;
    }

    private void OnConsume(FirstAid obj)
    {
        //перемещаем аптечку из одного кармана в другой
        busy.Remove(obj);
        avaliable.Add(obj);
    }

    public void SpawnFirstAid(Vector3 pos)
    {
        if(avaliable.Count == 0) //если нет свободных аптечек, создаём новую
        {
            AddAidToPool();
        }
        FirstAid item = avaliable[avaliable.Count - 1]; //последняя свободная аптечка

        item.Spawn(pos);
        busy.Add(item); //последняя свободная аптечка добавлена в busy
        avaliable.Remove(item); //последняя свободная аптечка удалена

    }
}

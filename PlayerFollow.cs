using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    [SerializeField]
    private Transform playerTransform;
    private Vector3 cameraOffset;
    [Range(0.01f, 1.0f)]
    public float smoothFactor = 0.5f; //Коэффициент сглаживания, что бы камера нежно изменяла позицию

    public bool LookAtplayer = true;

    void Start()
    {
        //замеряем расстояние между камерой и игроком
        cameraOffset = transform.position - playerTransform.position;
    }

   
    void LateUpdate() //срабатывает после  Update()   Здесь будем вращать камеру
    {         
        // Создаём новую позицию камеры с учётом следования игрока
        Vector3 newPos = playerTransform.position + cameraOffset;

        // смещаем камеру на новую позицию, Slerp - облегчает плавное смещение
        transform.position = Vector3.Slerp(transform.position, newPos, smoothFactor);

        if(LookAtplayer)
        {
            transform.LookAt(playerTransform);
        }

    }
}

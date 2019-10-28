using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;

    [SerializeField]
    PlayerController player; //игрок, за которым охотятся враги

    [SerializeField]
    private float distanceToPlayer = 10f; // дистанция, с которой начнут враги бежать на героя

    private const float EPSILON = 0.1f; //для проверки, движется враг или стоит на месте

    private float health;

    [SerializeField]
    private float maxHealth = 50f; //здоровье врага, сколько всего

    private BoxBar healthBar; // здоровье врага

    private readonly string PLAYER_WEAPON = "PlayerWeapon"; //тег оружия игрока

    private readonly string PLAYER = "Player"; //тег героя нашего

    private bool isVulnerable = false; // уязвимость

    private bool isDead = false;

    private bool isAttacking = false;

    private float attackSpeed = 0.5f;

    private Weapon weapon;//оружие и есть враг у нас

    private Tween attackTween;

    public static System.Action OnDeath; //событие

    //кровь
    [SerializeField]
    private ParticleSystem bloodParticle;

    [SerializeField]
    private Transform respawnTransform; //позиция RespawnEnemy
    private float activeMoveRadius = 30f; //на сколько далеко будут забегать враги
    private float pasiveMoveRadius = 10f; //радиус спокойствия врага

    private const float RESPAWN_TIME = 20f;

    private Tween respawnTween; //твин для респавна
    private Vector3 lastMovePosition;//последняя точка куда хотел идти враг

    private bool isAngry;

    private float MOVE_EPSILON = 2f; //для проверки - дошёл ли враг до ключевой точки куда шёл

    private float lootProbability = 0.5f; //вероятность выпадания аптечки

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        health = maxHealth;
        healthBar = GetComponentInChildren<BoxBar>();
        weapon = GetComponent<Weapon>();
        bloodParticle = GetComponentInChildren<ParticleSystem>();
        isAngry = false;
        respawnTransform = transform.parent; //задаём точку респавна
        lastMovePosition = respawnTransform.position; //указываем, что последняя точка в его пути будет точка респавна(что бы вернулся)

        agent.speed = 2f; // делаем скорость врагов, можно изменять в mesh agent

    }

    //срабатывает при запуске игрового объекта
    private void OnEnable()
    {
        //делаем подписку на событие - на смерть
        PlayerController.OnDeath += OnPlayerDead;       
    }

    private void OnDisable() // отписка от события
    {
        PlayerController.OnDeath -= OnPlayerDead;
    }

    void OnPlayerDead()
    {
        isAttacking = false;

        //останавливаем Твин
        attackTween.Kill();
    }

    private bool IsNavMashMoving
    {
        get
        {
            return agent.velocity.magnitude > EPSILON;  // двигается ли враг
        }
    }

    void Update()
    {
        if (isDead) // если враг не умер
        {
            return;
        }

        float playerDistance = Vector3.Distance(player.transform.position, transform.position); //дистанция до игрока от врага
                                                                                                //(позиция игрока, поизиция врага)
        float respawnDistance = Vector3.Distance(respawnTransform.position, transform.position); //дистанция до респавна от врага

        if (playerDistance < distanceToPlayer && respawnDistance < activeMoveRadius)
        { 
            isAngry = true;
            Vector3 playerPos = player.transform.position; //координаты героя
            agent.SetDestination(playerPos);
            //agent.Warp(playerPos);
        }
        else
        {
            if(isAngry)
            {
                agent.SetDestination(lastMovePosition); //возвращаем врага на точку до того, как разозлили его
            }
            isAngry = false;

            MoveRandomly();
        }
    }

    private void MoveRandomly() //свободное движение врага по территории
    {
        if(Vector3.Distance(lastMovePosition, transform.position) < MOVE_EPSILON)
        {
            //дошли до цели и задаём новую точку
            lastMovePosition = GetRandomPassivePoint();
            agent.SetDestination(lastMovePosition);
        }
    }

    private Vector3 GetRandomPassivePoint()
    {
        return new Vector3(
            respawnTransform.position.x + Random.Range(0, pasiveMoveRadius),
            respawnTransform.position.y,
            respawnTransform.position.z + Random.Range(0, pasiveMoveRadius)
            );
    }

    //появление врага после того, как герой ушёл
    private void RespawnDelay() // задержка возрождения, типа когда герой уйдёт из зоны врагов, тогда возродятся
    {
        if(respawnTween != null)
        {
            respawnTween.Kill();
        }
        respawnTween = DOVirtual.DelayedCall(RESPAWN_TIME, ()=> {
            if (Vector3.Distance(player.transform.position, respawnTransform.position) > pasiveMoveRadius) //если вышел герой из зоны врага
            {
                Respawn();
            }
            else
            {
                RespawnDelay();
            }

        });
    }

    private void Respawn()
    {
        transform.position = respawnTransform.position; //помещаем персонаж(Enemy) в точку respawn

        gameObject.SetActive(true);  // включаем персонаж (Enemy)
        isDead = false; 
        health = maxHealth;  //восстановили жизни
        healthBar.SetValue(health, maxHealth);  //восстановили жизни в healthBar
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == PLAYER_WEAPON)
        {
            GetDamage(other.gameObject.GetComponent<Weapon>().GetDamage); // наносим урон оружием игрока
        }

       
        if(other.gameObject.tag == PLAYER && !player.IsDead)
        {
            Debug.Log("Враг наносит удар - умри");
            isAttacking = true;
            Attack();
        }
    }

    private void OnTriggerExit(Collider other) // прекращаем атаковать героя, если он вышел из коллайдера злодея
    {
        if(other.gameObject.tag == PLAYER)
        {
            isAttacking = false;
            attackTween.Kill();
        }
    }

    private void Attack()
    {
        if(isAttacking)
        {
            player.GetDamage(weapon.GetDamage);

            if(attackTween != null && attackTween.IsActive()) //убиваем предыдущий твин, предыдущ атаку
            {
                attackTween.Kill();
            }
            // повтор атаки по событию OnComplete
            attackTween = DOVirtual.DelayedCall(attackSpeed, () => { }).OnComplete(delegate
            {  
                // здесь РЕКУРСИЯ АТАКИ ! атака зацикливается внутри атаки
                Attack();
                Debug.Log("АТАКА ВРАГА - получи злодей!!" + name);
            } );
        }
    }

    private void GetDamage(float value)  //урон врага
    {
        health = Mathf.Clamp(health - value, 0, health);

        //задаём здоровье в Баре
        healthBar.SetValue(health, maxHealth);

        if(health <= 0)
        {
            Die();
        }
        BloodsFX();
    }

    private void Die()
    {
        isDead = true;
        attackTween.Kill();
        gameObject.SetActive(false);

        if(OnDeath != null) //если кто то подписан на событие OnDeath
        {
            OnDeath();
        }

        DropLoot();

        RespawnDelay();
    }

    private void DropLoot() //выпадание аптечек из врагов (перевод - типа добыча)
    {
        if(Random.value < lootProbability)
        {
            FirstAidPoolManager.Instance.SpawnFirstAid(transform.position);
        }
    }
   

    private void BloodsFX() //брызги крови
    {
        bloodParticle.Stop();
        bloodParticle.Play();
    }
}

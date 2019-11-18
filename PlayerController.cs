using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;
using System.Linq;
using DG.Tweening;


public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    private CharacterController characterController; //персонаж
    private float gravity = 20f; // гравитация
    private Vector3 moveDirection = Vector3.zero;  // вектор движения
    public float speed = 5f;
    public float rotationSpeed = 240f; // скорость поворота
    private const float PLAYER_DONT_MOVE_SPEED = 0.01f;
    private bool isRunning = false;
    [SerializeField]
    private ImageBar healtBar; //здоровье
    private Collider _collider; // коллайдер игрока
    public static System.Action OnDeath;
    private float superAttackTime;
    private float attackTime;
    private float health; // текущие жизни
    [SerializeField]
    private float maxHealth; // max жизней
    [SerializeField]
    private BoxCollider weaponCollider;
    private bool isAttacking = false; // находится ли герой в состоянии атаки
    private const float SUPER_ATTACK_PROBABILITY = 0f; // вероятность супер атаки
    private Tween attackTween;
    private bool isDead = false; // умер герой или нет
    public bool IsDead
    {
        get
        {
            return isDead;
        }
    }
    //[SerializeField]
    //private GameObject swordTrail; //эффект при ударе битой, след в воздухе 
    [SerializeField]
    private ParticleSystem bloodParticle; //кровь

    public Transform rHand; //правая рука для оружия
    private HandItem handItem;


    private void Awake()
    {     // берём длины анимации по времени
        superAttackTime = animator.runtimeAnimatorController.animationClips.ToList().Find(a => a.name == "2Hand-Sword-Attack6").length;
        attackTime = animator.runtimeAnimatorController.animationClips.ToList().Find(a => a.name == "sword_slash_01").length;

        characterController = GetComponent<CharacterController>();

        _collider = GetComponent<Collider>();

        bloodParticle = GetComponentInChildren<ParticleSystem>();
    }

    private void Start()
    {
        weaponCollider.enabled = false; //что бы не активно оржие было когда игрок ничего не делает
        health = maxHealth;

       // swordTrail.SetActive(false);

        handItem = FindObjectOfType<HandItem>();
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }

        Move();
    }

    private void Move()
    {
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");

        //Узнаём, куда направлена камера в плоскости Z, и делаем направление персонажа в ту сторону, куда смотрит джойстик
        Vector3 camForwardDirection = Vector3.Scale(Camera.main.transform.forward,
            new Vector3(1, 0, 1)).normalized;

        //узнаём как сильно и куда наклонён джойстик
        Vector3 move = v * camForwardDirection + h * Camera.main.transform.right;

        //ограничиваем наклон джойстика, max-й наклон перонажа
        if (move.magnitude > 1f)
        {
            move.Normalize();
        }
        //приводим координаты джойстика из глобальных в локальные
        move = transform.InverseTransformDirection(move);

        //узнаём угол поворота персонажа вслед за джойстиком
        float turnAmout = Mathf.Atan2(move.x, move.y);

        //поворачиваем персонажа на заданный угол с указанной скоростью поворота
        transform.Rotate(0, turnAmout * rotationSpeed * Time.deltaTime, 0);

        //проверяем, стоит ли персонаж на земле
        if (characterController.isGrounded)
        {
            moveDirection = transform.forward * move.magnitude;
            moveDirection *= speed;
        }
        // имитируем гравитацию персонажа в игровом пространстве
        moveDirection.y -= gravity * Time.deltaTime;

        if (!isAttacking)
        {
            //Движение персонажа в заданном направлении
            characterController.Move(moveDirection * Time.deltaTime);
        }

        //узнаём скорость притяжения в игре для Animator Контроллера
        Vector3 horVelosity = characterController.velocity;

        //скорость персонажа в осях X и Z
        horVelosity = new Vector3(characterController.velocity.x, 0, characterController.velocity.z);

        //узнаём длину вектора скорости, что бы красиво проиграть анимацию ходьбы и чтобы вовремя персонаж останавливался
        float horSpeed = horVelosity.magnitude; // скорость персонажа в игре

        if (horSpeed > 0 && isRunning == false)
        {

            isRunning = true;
            animator.SetTrigger("Run");
        }
        if (horSpeed < PLAYER_DONT_MOVE_SPEED && isRunning == true)
        {
            isRunning = false;
            animator.SetTrigger("Idle");
        }
    }

    public void Attack()
    {
        if (!isAttacking && !isDead) // если нет атаки и если герой живой, то атакуем
        {
            isAttacking = true;
            weaponCollider.enabled = true; // вкулючаем коллайдер оружия
            animator.SetTrigger("Attack");

            // swordTrail.SetActive(true);

            // что бы знать, какую анимацию будем брать
            string attackAnimation = Random.value > SUPER_ATTACK_PROBABILITY ?
                                         AttackType.Attack.ToString() : AttackType.SuperAttack.ToString();

            animator.SetTrigger(attackAnimation);

            float thisAttackTime = attackAnimation == AttackType.Attack.ToString() ?  //":" -это или
                                              attackTime : superAttackTime;

            //запускаем Tween

            attackTween = DOVirtual.DelayedCall(thisAttackTime, delegate
           {
               isAttacking = false;
               weaponCollider.enabled = false;

              // swordTrail.SetActive(false);

               animator.SetTrigger("Idle");
           });
        }
    }
    //private void OnDisable()
    //{
    //    attackTween.Kill(); //убираем Tween
    //}

    public void GetDamage(float value) // получение урона
    {
        health = Mathf.Clamp(health - value, 0, health); //min, 0, max

        healtBar.SetValue(health, maxHealth);

        if (health <= 0)
        {
            Die();
        }
        Debug.Log("Здоровье");
        Debug.Log(health);
        BloodsFX();
    }

    public void Heal(float value) //Пополнение здоровья. Аптечка
    {
        health = Mathf.Clamp(health + value, 0, maxHealth);

        healtBar.SetValue(health, maxHealth);
    }

    public void Die()
    {
        isDead = true;

        attackTween.Kill();

        _collider.enabled = false; // убираем коллайдер у игрока, что бы не взаимодействовал ни с кем больше

        animator.SetTrigger("Die");

        if (OnDeath != null)
        {
            OnDeath();  // объявление, что герой умер
        }
    }

    private void BloodsFX() //брызги крови
    {
        bloodParticle.Stop();
        bloodParticle.Play();
    }

    public void AddHand(HandItem it)
    {
        //если есть уже что то в руке
        if (handItem != null)
        {
            handItem.transform.SetParent(null); //типа выбрасываем
            handItem.transform.position = transform.position + transform.forward + transform.up / 2;
            handItem.gameObject.AddComponent<Rigidbody>();
          
        }
        it.transform.SetParent(rHand); //делаем родитель - это будет рука
        it.transform.localPosition = it.position; // позиционируем
        it.transform.localRotation = Quaternion.Euler(it.rotation);
        Destroy(it.GetComponent<Rigidbody>()); //удаляем Rigitbody, что бы не выпадывал

        handItem = it;
    }
}

    public enum AttackType
    {
        Attack,
        SuperAttack
    }


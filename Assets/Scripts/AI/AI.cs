﻿using PlatformerPathFinding;
using PlatformerPathFinding.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public event System.Action OnHit;

    public RuntimeAnimatorController patrolController;
    public RuntimeAnimatorController jumpPatrolController;
    public RuntimeAnimatorController guardController;
    public RuntimeAnimatorController flyerController;
    public RuntimeAnimatorController carrierController;

    public PathFindingAgent agent;
    public AiController controller;

    public LayerMask groundLayer;
    public LayerMask platformLayer;
    public LayerMask wallLayer;
    public LayerMask playerLayer;

    public ParticleSystem bloodParticle;
    public Egg eggPrefab;

    [HideInInspector] public AIType aiType;
    [HideInInspector] public int health;

    private Animator animator;
    private Perception perception;
    private Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D collider;

    private AIBehaviour behaviour;

    private bool isBurning;
    [HideInInspector] public Vector2 startingPosition;

    private Material material;

    Transform[] targetsInMap;

    public virtual void Initialise (AIType aIType, Transform[] transforms = null)
    {
        animator = GetComponent<Animator>();
        perception = GetComponent<Perception>();
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<CapsuleCollider2D>();
        material = spriteRenderer.material;

        aiType = aIType;
        spriteRenderer.sprite = aiType.aiSprite;
        health = aiType.health;

        perception.viewingDistance = aiType.viewingDistance;
        perception.fieldOfView = aiType.fieldOfView;
        perception.hearingRadius = aiType.hearingRadius;
        perception.targetMemoryTime = aiType.targetMemoryTime;

        collider.offset = aiType.colliderOffset;
        collider.size = aiType.colliderSize;

        startingPosition = transform.position;
        behaviour = aiType.aiBehaviour;

        targetsInMap = transforms;


        switch (behaviour)
        {
            case AIBehaviour.Patrol:
                controller.enabled = false;
                agent.enabled = false;
                animator.runtimeAnimatorController = patrolController;
                animator.SetBool("CanAttack", true);

                break;
            case AIBehaviour.Guard:
                controller.enabled = false;
                agent.enabled = false;
                animator.runtimeAnimatorController = guardController;
                break;
            case AIBehaviour.Fly:
                controller.enabled = false;
                agent.enabled = false;
                animator.runtimeAnimatorController = flyerController;
                break;
            case AIBehaviour.Carrier:
                controller.enabled = false;
                agent.enabled = false;
                animator.runtimeAnimatorController = carrierController;
                break;
            case AIBehaviour.JumpPatrol:
                controller.enabled = true;
                agent.enabled = true;
                agent.Init(FindObjectOfType<PathFindingGrid>());
                animator.runtimeAnimatorController = jumpPatrolController;
                animator.SetBool("CanAttack", true);
                Destroy(GetComponent<Rigidbody2D>());
                GetComponent<Collider2D>().isTrigger = true;

                agent._fallLimit = aiType.fallLimit;
                agent._jumpStrength = (int)aiType.jumpStrength;
                controller._walkSpeed = aiType.movementSpeed;
                controller._jumpSpeed = aiType.jumpSpeed;
                controller._fallSpeed = aiType.fallSpeed;
                break;
            default:
                break;
        }
    }

    private void FixedUpdate()
    {
        DirectionChecker();
    }

    void DirectionChecker ()
    {
        if (rigidbody == null)
        {
            return;
        }

        if (rigidbody.velocity.x > 0)
        {
            perception.isFacingRight = true;
            spriteRenderer.flipX = false;
        }
        else if (rigidbody.velocity.x < 0)
        {
            perception.isFacingRight = false;
            spriteRenderer.flipX = true;
        }
    }

    public virtual void TakeDamage (int playerNumber,int damage)
    {        
        health -= damage;
        StartCoroutine("FlashHurt");
        if (health <= 0)
        {
            Death(playerNumber);
        }
        else
        {
            if (OnHit != null)
            {
                OnHit();
            }
        }
        ParticleSystem p = Instantiate(bloodParticle, transform.position, Quaternion.identity);
        p.Play();
        Destroy(p, 1);
    }


    public void HitByFlame(int projectilePlayerNumber)
    {
        if (isBurning)
        {
            return;
        }

        isBurning = true;
        StartCoroutine(Burning(projectilePlayerNumber));
        GetComponent<SpriteRenderer>().color = Color.red;
    }

    public void StopBurning ()
    {
        if (isBurning)
        {
            isBurning = false;
            spriteRenderer.color = Color.white;
        }
    }

    IEnumerator Burning(int projectilePlayerNumber)
    {
        yield return new WaitForSeconds(1.0f);
        while (health > 0)
        {
            health--;
            if (health <= 0)
            {
                Death(projectilePlayerNumber);
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

    public virtual void Death (int playerNumber)
    {
        if (GameManager.instance.SelectedGamemode != null)
        {
            GameManager.instance.SelectedGamemode.AwardAiKill(playerNumber);
            ScorePopup scorePopup = Instantiate(LevelManager.instance.scorePopupPrefab, transform.position, Quaternion.identity);
            scorePopup.Init(GameManager.instance.SelectedGamemode.aiKillsPointReward);
        }

        if (LevelManager.instance != null)
        {
            if (LevelManager.instance.weaponTypes.Count > 0)
            {
                float rand = Random.Range(0.0f,1.0f);
                if (rand < aiType.chanceOfDroppingWeapon)
                {
                    Weapon weapon = Instantiate(LevelManager.instance.weaponPrefab,
                    transform.position,
                    Quaternion.identity);
                    weapon.Init(LevelManager.instance.weaponTypes, WeaponSpawnType.FallFromSky);
                }                
            }
        }

        Destroy(gameObject);
    }

    public void StartAttackCooldown()
    {
        StartCoroutine("CoAttackCooldown");
    }

    IEnumerator CoAttackCooldown ()
    {
        yield return new WaitForSeconds(aiType.attackCooldown);
        animator.SetBool("CanAttack", true);
    }

    public void SetRandomGoal()
    {
        if (targetsInMap != null)
        {

        }
        controller._goal = targetsInMap[Random.Range(0, targetsInMap.Length)];
    }

    IEnumerator FlashHurt()
    {
        material.SetFloat("_IsHurt", 1.0f);
        yield return new WaitForSeconds(0.2f);
        material.SetFloat("_IsHurt", 0.0f);

    }
}

public enum AIBehaviour { Patrol, Guard, Fly, Carrier, JumpPatrol }

[System.Serializable]
public class AIType
{
    public string AIName;
    public Sprite aiSprite;
    public string spriteName;
    public int health;
    public float movementSpeed;
    public int attackDamage;
    public float attackCooldown;
    public float attackRange;
    public float attackSize;

    public Vector2 colliderSize;
    public Vector2 colliderOffset;

    public float viewingDistance;
    public float fieldOfView;
    public float hearingRadius;
    public float targetMemoryTime;

    public float chanceOfDroppingWeapon;

    public AIBehaviour aiBehaviour;

    public float smallJumpHeight;
    public float largeJumpHeight;

    public float jumpSpeed;
    public int jumpStrength;
    public float fallSpeed;
    public int fallLimit;
    public float targetResetTime;

    public float wallDetectionDistance;
    public Vector2 FiringPoint;
    public string projectileName;
    public float projectileForce;
    public float bulletDeviation;

    public float swoopSpeed;

    public Vector2 eggOffset;

}

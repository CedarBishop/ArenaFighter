﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConsumableType { DoubleJump, Healing, Shield, SuperShield, SpeedBoost, InfiniteAmmo}

public class Consumable : MonoBehaviour
{
    private Player player;

    private ConsumableType consumableType;

    private float timer;
    private bool isActive;

    public virtual void Use( Player _Player, ConsumableType type, float duration, float amount)
    {
        isActive = true;
        timer = duration;
        player = _Player;
        consumableType = type;

        switch (consumableType)
        {
            case ConsumableType.DoubleJump:
                player.playerMovement.CanDoubleJump(true);
                break;
            case ConsumableType.Healing:
                break;
            case ConsumableType.Shield:
                break;
            case ConsumableType.SuperShield:
                break;
            case ConsumableType.SpeedBoost:
                break;
            case ConsumableType.InfiniteAmmo:
                break;
            default:
                break;
        }
    }

    private void FixedUpdate()
    {
        timer -= Time.fixedDeltaTime;
        if (isActive)
        {
            if (timer <= 0.0f)
            {
                Deactivate();
            }
        }
    }

    private void Deactivate ()
    {
        isActive = false;

        switch (consumableType)
        {
            case ConsumableType.DoubleJump:
                player.playerMovement.CanDoubleJump(false);
                break;
            case ConsumableType.Healing:
                break;
            case ConsumableType.Shield:
                break;
            case ConsumableType.SuperShield:
                break;
            case ConsumableType.SpeedBoost:
                break;
            case ConsumableType.InfiniteAmmo:
                break;
            default:
                break;
        }

        Destroy(gameObject);
    }
}
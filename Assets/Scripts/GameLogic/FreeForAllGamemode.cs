﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeForAllGamemode : BaseGamemode
{
    public override void StartMatch ()
    {
        base.StartMatch();
        Debug.Log("Free for all start match");
        playersStillAliveThisRound = numOfPlayers;

        foreach (Player player in players)
        {
            player.CharacterDied(false);
        }

        StartRound();
    }

    protected override void EndMatch ()
    {
        base.EndMatch();
        StartCoroutine("DelayAtEndOfMatch");
    }

    public override void StartRound()
    {
        base.StartRound();

        playersStillAliveThisRound = numOfPlayers;
        UIManager.instance.StartNewRound(roundNumber);

        foreach (Player player in players)
        {
            player.CreateNewCharacter();
        }

    }

    public override void EndRound (int winningPlayerNumber)
    {
        if ( roundIsUnderway == false)
        {
            return;
        }
        roundIsUnderway = false;
        if (roundNumber >= numberOfRounds)
        {
            EndMatch();
        }
        else
        {
            UIManager.instance.EndRound(winningPlayerNumber , roundNumber);
            roundNumber++;
            print("End Round");
            StartCoroutine("DelayBetweenRounds");
        }

    }



    public override void PlayerDied ()
    {
        base.PlayerDied();

        if (playersStillAliveThisRound == 1)
        {
            int winningPlayerNumber = 0;
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].isStillAlive)
                {
                    winningPlayerNumber = players[i].playerNumber;
                }
            }
            AwardRoundWin(winningPlayerNumber);
            EndRound(winningPlayerNumber);
        }
        else if (playersStillAliveThisRound < 1)
        {
            EndRound(0);
        }
    }

    IEnumerator DelayAtStartOfMatch()
    {
        yield return new WaitForSeconds(0.5f);
        StartRound();
    }

    IEnumerator DelayBetweenRounds ()
    {
        yield return new WaitForSeconds(3);
        GameManager.instance.levelSelector.GoToLevel(GameMode.FreeForAll, StartRound);
    }

    IEnumerator DelayAtEndOfMatch ()
    {
        yield return new WaitForSeconds(3);

        Exit();

        GameManager.instance.EndMatch();
    }
}

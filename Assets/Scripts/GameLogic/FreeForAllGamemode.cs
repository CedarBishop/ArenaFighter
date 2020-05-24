﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeForAllGamemode : BaseGamemode
{
    
    [HideInInspector] public int playersEliminated;

    


    public override void StartMatch ()
    {
        base.StartMatch();
        Debug.Log("Free for all start match");
        playersStillAliveThisRound = numOfPlayers;

        foreach (Player player in players)
        {
            player.CharacterDied(false);
        }

        StartCoroutine("DelayBetweenRounds");
    }

    protected override void EndMatch ()
    {
        int highestWins = 0;
        List<int> currentBestPlayers = new List<int>() {0};
        foreach (PlayerMatchStats player in playerMatchStats)
        {
            if (player.roundWins > highestWins)
            {
                currentBestPlayers.Clear();
                currentBestPlayers.Add(player.playerNumber);
                highestWins = player.roundWins;
            }
            else if (player.roundWins == highestWins)
            {
                currentBestPlayers.Add(player.playerNumber);
            }
        }

        print(currentBestPlayers);

        foreach (int playerNum in currentBestPlayers)
        {
            GameManager.instance.AwardMatchWin(playerNum);
        }

        UIManager.instance.EndMatch(currentBestPlayers);
        StartCoroutine("DelayAtEndOfMatch");
            
    }

    public void StartRound()
    {
       

        playersStillAliveThisRound = numOfPlayers - playersEliminated;
        UIManager.instance.StartNewRound(roundNumber);

        foreach (Player player in players)
        {
            player.CreateNewCharacter();
        }

    }

    public override void EndRound (int winningPlayerNumber)
    {
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


    IEnumerator DelayBetweenRounds ()
    {
        yield return new WaitForSeconds(3);
        StartRound();
        GameManager.instance.levelSelector.GoToLevel(GameMode.FreeForAll);
    }

    IEnumerator DelayAtEndOfMatch ()
    {
        yield return new WaitForSeconds(3);

        Exit();

        GameManager.instance.EndMatch();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestKillEnemies : MonoBehaviour
{
    public int enemiesToKill = 10;
    private int currentKills = 0;
    public bool questCompleted = false;

    public void EnemyKilled()
    {
        if (questCompleted) return;

        currentKills++;

        Debug.Log("Gegner besiegt: " + currentKills + "/" + enemiesToKill);

        if (currentKills >= enemiesToKill)
        {
            questCompleted = true;
            Debug.Log("Quest abgeschlossen!");
        }
    }
}

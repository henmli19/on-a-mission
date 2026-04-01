using UnityEngine;

// ─────────────────────────────────────────────────────────────
//  QuestEnemy
//
//  Attach this to any enemy prefab that should count toward
//  the quest kill counter. It's also added automatically at
//  runtime by QuestNPC when it spawns enemies.
//
//  When this enemy dies (its GameObject is destroyed),
//  it tells the QuestNPC so the kill counter updates.
// ─────────────────────────────────────────────────────────────
public class QuestEnemy : MonoBehaviour
{
    private QuestNPC questNPC;

    public void Init(QuestNPC npc)
    {
        questNPC = npc;
    }

    private void OnDestroy()
    {
        if (!Application.isPlaying) return;

        // Don't report if scene is unloading
        if (!gameObject.scene.isLoaded) return;

        if (questNPC != null)
            questNPC.OnQuestEnemyKilled();
    }
}
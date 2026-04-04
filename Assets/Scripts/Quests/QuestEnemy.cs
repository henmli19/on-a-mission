using UnityEngine;

// ─────────────────────────────────────────────────────────────
//  QuestEnemy
//
//  Attach to any enemy prefab that should count toward the
//  quest kill counter. Also added automatically at runtime
//  by QuestNPC when it spawns enemies.
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
        if (!gameObject.scene.isLoaded) return;

        if (questNPC != null)
            questNPC.OnQuestEnemyKilled();
    }
}

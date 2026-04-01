using UnityEngine;

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
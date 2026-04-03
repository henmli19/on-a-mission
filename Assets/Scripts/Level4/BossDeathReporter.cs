using UnityEngine;

public class BossDeathReporter : MonoBehaviour
{
    private BossLevelManager levelManager;

    public void Init(BossLevelManager manager)
    {
        levelManager = manager;
    }

    private void OnDestroy()
    {
        if (!Application.isPlaying) return;
        if (!gameObject.scene.isLoaded) return;

        if (levelManager != null)
            levelManager.OnBossDefeated();
    }
}
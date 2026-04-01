// QuestManager.cs
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public enum QuestState { NotStarted, Active, Completed }
    public QuestState energyCoreQuestState = QuestState.NotStarted;

    // Event damit die UI sich automatisch aktualisiert
    public System.Action OnQuestStateChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartEnergyQuest()
    {
        if (energyCoreQuestState == QuestState.NotStarted)
        {
            energyCoreQuestState = QuestState.Active;
            Debug.Log("Quest gestartet: Energy Core finden und abliefern!");
            OnQuestStateChanged?.Invoke();
        }
    }

    public void CompleteEnergyQuest()
    {
        if (energyCoreQuestState == QuestState.Active)
        {
            energyCoreQuestState = QuestState.Completed;
            Debug.Log("Quest abgeschlossen!");
            OnQuestStateChanged?.Invoke();
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

// ─────────────────────────────────────────────────────────────
//  PowerupDropTable  (ScriptableObject)
//
//  Create one via:
//    Assets → Create → Boss → Powerup Drop Table
//
//  Assign your existing powerup prefabs in the Inspector,
//  each with a weight (higher = more common).
// ─────────────────────────────────────────────────────────────
[CreateAssetMenu(fileName = "PowerupDropTable", menuName = "Boss/Powerup Drop Table")]
public class PowerupDropTable : ScriptableObject
{
    [System.Serializable]
    public class PowerupEntry
    {
        public GameObject prefab;
        [Tooltip("Relative chance of this drop. E.g. weight 3 is 3× more likely than weight 1.")]
        [Min(1)] public int weight = 1;
    }

    [SerializeField] private List<PowerupEntry> entries = new List<PowerupEntry>();

    // Returns a random prefab based on weights, or null if the table is empty.
    public GameObject GetRandomDrop()
    {
        if (entries == null || entries.Count == 0) return null;

        int totalWeight = 0;
        foreach (var e in entries)
            totalWeight += e.weight;

        int roll = Random.Range(0, totalWeight);
        int running = 0;

        foreach (var e in entries)
        {
            running += e.weight;
            if (roll < running)
                return e.prefab;
        }

        return entries[entries.Count - 1].prefab; // fallback
    }
}
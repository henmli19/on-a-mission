using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private ParticleSystem explosionEffect;

    private Button button;

    private void Awake()
    {
        button = GetComponentInChildren<Button>();
        button.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        explosionEffect.transform.position = transform.position;
        explosionEffect.Play();
        gameManager.TargetHit();
    }
}
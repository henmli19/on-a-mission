using UnityEngine;
using TMPro; // only if you use TextMeshPro

public class TutorialTextManager : MonoBehaviour
{
    [SerializeField] private GameObject jumpText;
    [SerializeField] private GameObject dashText;
    [SerializeField] private GameObject moveText;
    [SerializeField] private GameObject shootText;
    [SerializeField] private GameObject InteractText;
    public GameObject tutorialPanel;      
        
    public float delay = 20f;  

    void Start()
    {
        tutorialPanel.SetActive(false);    
        StartCoroutine(ShowTextCoroutine());
    }
    void Update()
    {
        TextManagerTurnOff();
    }

    private System.Collections.IEnumerator ShowTextCoroutine()
    {
        yield return new WaitForSeconds(delay); 
        tutorialPanel.SetActive(true);          
    }
    
    void TextManagerTurnOff()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)|| Input.GetKeyDown(KeyCode.W))
        {
            jumpText.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)|| Input.GetKeyDown(KeyCode.RightArrow)|| Input.GetKeyDown(KeyCode.A)|| Input.GetKeyDown(KeyCode.D))
        {
            moveText.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            dashText.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            shootText.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            InteractText.SetActive(false);
        }
 

    }
}

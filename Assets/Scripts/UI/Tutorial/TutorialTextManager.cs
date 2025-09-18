using UnityEngine;
using TMPro; // only if you use TextMeshPro

public class TutorialTextManager : MonoBehaviour
{
    [SerializeField] private GameObject jumpText;
    [SerializeField] private GameObject dashText;
    [SerializeField] private GameObject moveText;
   

    void Update()
    {
        TextManagerTurnOff();
        
    } void TextManagerTurnOff()
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
 

    }
}

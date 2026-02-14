using UnityEngine;
using TMPro;
using System.Collections;

public class TurnIndicator : MonoBehaviour
{
    [Header("UI References")]
    public GameObject indicatorPanel;
    public TMP_Text turnText;
    
    [Header("Colors")]
    public Color playerTurnColor = new Color(0.2f, 0.8f, 0.3f); 
    public Color enemyTurnColor = new Color(0.9f, 0.2f, 0.2f);  
    
    [Header("Animation")]
    public float displayDuration = 2f;
    
    void Start()
    {
        if (indicatorPanel != null)
        {
            indicatorPanel.SetActive(false);
        }
    }
    


    public void ShowPlayerTurn()
    {
        StopAllCoroutines();
        StartCoroutine(ShowNotification("YOUR TURN", playerTurnColor));
        Debug.Log("YOUR TURN");
    }
    
 
    public void ShowEnemyTurn()
    {
        StopAllCoroutines();
        StartCoroutine(ShowNotification("ENEMY TURN", enemyTurnColor));
        Debug.Log("ENEMY TURN");
    }

    IEnumerator ShowNotification(string message, Color color)
    {
        if (indicatorPanel != null)
        {
            indicatorPanel.SetActive(true);
        }
        
        if (turnText != null)
        {
            turnText.text = message;
            turnText.color = color;
        }
        
        yield return new WaitForSeconds(displayDuration);
        
        if (indicatorPanel != null)
        {
            indicatorPanel.SetActive(false);
        }
    }
  
    public void Hide()
    {
        StopAllCoroutines();
        
        if (indicatorPanel != null)
        {
            indicatorPanel.SetActive(false);
        }
    }
}
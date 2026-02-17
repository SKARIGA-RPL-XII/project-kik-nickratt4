using UnityEngine;

public class EnemyOriginalPosition : MonoBehaviour
{
    private Vector3 originalPosition;
    private bool originalFlip;
    private bool isInitialized = false;
    
    void Awake()
    {
 
    }
    
    void Start()
    {
        if (!isInitialized)
        {
            SaveInitialState();
        }
    }
    
    void SaveInitialState()
    {
        if (!isInitialized)
        {
            originalPosition = transform.position;
            
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                originalFlip = sr.flipX;
            }
            
            isInitialized = true;
            Debug.Log(gameObject.name + " original position saved (auto): " + originalPosition);
        }
    }
    
    public void SetOriginalPosition(Vector3 pos)
    {
        originalPosition = pos;
        isInitialized = true;
        Debug.Log(gameObject.name + " original position set (manual): " + pos);
    }
    
    public Vector3 GetOriginalPosition()
    {
        return originalPosition;
    }
    
    public void SetOriginalFlip(bool flip)
    {
        originalFlip = flip;
    }
    
    public bool GetOriginalFlip()
    {
        return originalFlip;
    }
    
    public void ResetToOriginal()
    {
        transform.position = originalPosition;
        
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.flipX = originalFlip;
        }
        
        Debug.Log(gameObject.name + " reset to: " + originalPosition);
    }
}

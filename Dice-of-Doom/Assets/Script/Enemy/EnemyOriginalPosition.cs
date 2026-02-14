using UnityEngine;

public class EnemyOriginalPosition : MonoBehaviour
{
    private Vector3 originalPosition;
    private bool originalFlip;
    private bool isInitialized = false;
    
    void Awake()
    {
        // JANGAN save di Awake karena posisi belum final
        // Biarkan spawner yang set posisi
    }
    
    void Start()
    {
        // Hanya save jika belum pernah di-set dari luar
        if (!isInitialized)
        {
            SaveInitialState();
        }
    }
    
    void SaveInitialState()
    {
        // Hanya save jika belum pernah di-set
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
        isInitialized = true; // Mark as initialized
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

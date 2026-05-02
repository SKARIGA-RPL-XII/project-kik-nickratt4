using UnityEngine;
using TMPro;

public class DamageIndicator : MonoBehaviour
{
    public static void Show(Vector3 position, int damage, bool isEnemyAttacking = false)
    {
        GameObject go = new GameObject("DamageText");
        go.transform.position = position + Vector3.up * 1.5f; 
        TMP_Text text = go.AddComponent<TMP_Text>();
        text.font = Resources.Load<TMP_FontAsset>("LiberationSans SDF"); // ganti dengan font Anda
        text.fontSize = 24;
        text.alignment = TextAlignmentOptions.Center;
        text.color = isEnemyAttacking ? Color.red : Color.green;
        text.text = damage.ToString();

        // Animasi sederhana: naik ke atas lalu fade out
        go.AddComponent<DamageIndicatorAnimator>();
    }
}

public class DamageIndicatorAnimator : MonoBehaviour
{
    private TMP_Text text;
    private float timer = 0f;
    private float duration = 1f;
    private Vector3 startPos;

    void Start()
    {
        text = GetComponent<TMP_Text>();
        startPos = transform.position;
        Destroy(gameObject, duration);
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = timer / duration;
        transform.position = startPos + Vector3.up * (t * 0.5f);
        if (text != null)
        {
            Color c = text.color;
            c.a = 1f - t;
            text.color = c;
        }
    }
}
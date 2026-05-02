using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class prolog : MonoBehaviour
{
    [System.Serializable]
    public class Chapter
    {
        public Sprite backgroundImage;
        public string dialogueText;
    }

    public List<Chapter> chapters = new List<Chapter>();
    public Image backgroundImageUI;
    public TextMeshProUGUI dialogueTextUI;
    public Button nextButton;  
    public float typingSpeed = 0.05f;
    public string nextSceneName;

    private int currentIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    void Start()
    {
  

        if (chapters.Count > 0)
            ShowChapter(currentIndex);
    }

    public void OnNextButtonClick()
    {
        if (isTyping)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);
            dialogueTextUI.text = chapters[currentIndex].dialogueText;
            isTyping = false;
            return;
        }

        currentIndex++;
        if (currentIndex < chapters.Count)
            ShowChapter(currentIndex);
        else
        {
            if (!string.IsNullOrEmpty(nextSceneName))
                SceneManager.LoadScene(nextSceneName);
        }
    }

    void ShowChapter(int index)
    {
        backgroundImageUI.sprite = chapters[index].backgroundImage;
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(chapters[index].dialogueText));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueTextUI.text = "";
        foreach (char c in text)
        {
            dialogueTextUI.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }
}
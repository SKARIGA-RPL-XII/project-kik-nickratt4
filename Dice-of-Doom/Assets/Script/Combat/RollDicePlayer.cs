using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;

public class RollDicePlayer : MonoBehaviour
{
    public PlayerAttack playerAttack;
    public EnemyDiceRoller enemyDiceRoller;

    [Header("DICE UI ")]
    public GameObject[] diceObjects;
    public TMP_Text[] diceTexts;

    [Header("Math Question UI")]
    public TMP_InputField answerInput;

    [Header("API")]
    public string baseUrl = "http://localhost/get_player.php";

    [Header("Turn Indicator")]
    public TurnIndicator turnIndicator;

    public int lastRollTotal;

    private int playerBaseDamage = 25;
    private int playerLevel = 2;
    private int minResult;
    private int maxResult;

    private int currentCorrectAnswer;
    private int currentNumber1;
    private int currentNumber2;
    private string currentOperator;

    private bool isPlayerTurn = true;
    private bool isRollingQuestion = false;
    private bool isWaitingForSubmit = false;
    private Coroutine rollingCoroutine;

    void Start()
    {
        Debug.Log("RollDicePlayer Start");
        if (enemyDiceRoller == null) enemyDiceRoller = FindObjectOfType<EnemyDiceRoller>();
        if (turnIndicator == null) turnIndicator = FindObjectOfType<TurnIndicator>();
        if (playerAttack == null) playerAttack = GetComponent<PlayerAttack>();

        // Sembunyikan dadu index 3 dan 4
        if (diceObjects.Length >= 5)
        {
            for (int i = 3; i < diceObjects.Length; i++)
                if (diceObjects[i]) diceObjects[i].SetActive(false);
        }
        for (int i = 0; i < 3 && i < diceObjects.Length; i++)
            if (diceObjects[i]) diceObjects[i].SetActive(true);

        if (answerInput) answerInput.gameObject.SetActive(false);
        else Debug.LogError("AnswerInput belum diassign!");

        StartCoroutine(GetPlayerFromAPI());
    }

    IEnumerator GetPlayerFromAPI()
    {
        Debug.Log("GetPlayerFromAPI dimulai, URL: " + baseUrl);
        int playerId = PlayerPrefs.GetInt("player_id", 1);
        string url = baseUrl + "?player_id=" + playerId;
        bool success = false;

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;
                Debug.Log("Response JSON: " + json);
                try
                {
                    PlayerData data = JsonUtility.FromJson<PlayerData>(json);
                    if (data != null && data.status == "success")
                    {
                        playerBaseDamage = data.base_damage;
                        playerLevel = data.level > 0 ? data.level : Mathf.Max(1, playerBaseDamage / 10);
                        Debug.Log($"Base Damage = {playerBaseDamage}, Level = {playerLevel}");
                        success = true;
                    }
                }
                catch (System.Exception e) { Debug.LogError("JSON error: " + e.Message); }
            }
            else Debug.LogError("API error: " + www.error);
        }

        if (!success)
        {
            Debug.LogWarning("Using default base_damage=25, level=2");
            playerBaseDamage = 25;
            playerLevel = 2;
        }

        minResult = Mathf.RoundToInt(playerBaseDamage * 0.5f) + 1;
        maxResult = playerBaseDamage;
        Debug.Log($"Range soal: {minResult} - {maxResult}");

        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        if (!isPlayerTurn)
        {
            isPlayerTurn = true;
            isRollingQuestion = false;
            isWaitingForSubmit = false;
            if (answerInput) answerInput.gameObject.SetActive(false);
            if (turnIndicator) turnIndicator.ShowPlayerTurn();
            SetDiceTexts("?", "?", "?");
            Debug.Log("Player turn dimulai.");
        }
    }

    void SetDiceTexts(string t1, string t2, string t3)
    {
        if (diceTexts.Length >= 3)
        {
            diceTexts[0].text = t1;
            diceTexts[1].text = t2;
            diceTexts[2].text = t3;
        }
    }

    void StartRollingQuestion()
    {
        if (rollingCoroutine != null) StopCoroutine(rollingCoroutine);
        rollingCoroutine = StartCoroutine(RollingQuestionCoroutine());
    }

    IEnumerator RollingQuestionCoroutine()
    {
        isRollingQuestion = true;
        float rollSpeed = 0.1f;
        while (isRollingQuestion)
        {
            int a = Random.Range(1, maxResult * 2);
            int b = Random.Range(1, maxResult * 2);
            int op = Random.Range(0, 4);
            string opSym = (op == 0) ? "+" : (op == 1) ? "-" : (op == 2) ? "×" : "÷";
            SetDiceTexts(a.ToString(), opSym, b.ToString());
            yield return new WaitForSeconds(rollSpeed);
        }
        GenerateFinalQuestion();
        isWaitingForSubmit = true;
        if (answerInput)
        {
            answerInput.gameObject.SetActive(true);
            answerInput.text = "";
            answerInput.ActivateInputField();
        }
        Debug.Log("Soal final. Ketik jawaban lalu tekan tombol lagi.");
    }

    void GenerateFinalQuestion()
    {
        for (int attempt = 0; attempt < 15; attempt++)
        {
            int target = Random.Range(minResult, maxResult + 1);
            int op = GetWeightedOperator();
            int a = 0, b = 0;
            bool valid = true;
            switch (op)
            {
                case 0: a = Random.Range(1, target); b = target - a; break;
                case 1: b = Random.Range(1, Mathf.Min(maxResult, target + 10)); a = target + b; if (a > maxResult * 2) valid = false; break;
                case 2:
                    valid = false;
                    for (int f = 2; f <= Mathf.Sqrt(target); f++)
                        if (target % f == 0) { a = f; b = target / f; valid = true; break; }
                    break;
                case 3:
                    b = Random.Range(2, Mathf.Min(10, maxResult));
                    a = target * b;
                    if (a > maxResult * 3) valid = false;
                    else valid = true;
                    break;
            }
            if (valid && a >= 1 && b >= 1 && a <= maxResult * 3 && b <= maxResult * 3)
            {
                currentNumber1 = a;
                currentNumber2 = b;
                currentOperator = (op == 0) ? "+" : (op == 1) ? "-" : (op == 2) ? "×" : "÷";
                currentCorrectAnswer = target;
                SetDiceTexts(a.ToString(), currentOperator, b.ToString());
                Debug.Log($"Soal: {a} {currentOperator} {b} = {target}");
                return;
            }
        }
        currentCorrectAnswer = Random.Range(minResult, maxResult + 1);
        SetDiceTexts(currentCorrectAnswer.ToString(), "+", "0");
        Debug.LogWarning("Fallback soal");
    }

    int GetWeightedOperator()
    {
        int rand = Random.Range(0, 100);
        if (playerLevel >= 5)
        {
            if (rand < 20) return 0;
            else if (rand < 40) return 1;
            else if (rand < 70) return 2;
            else return 3;
        }
        else if (playerLevel >= 3)
        {
            if (rand < 25) return 0;
            else if (rand < 50) return 1;
            else if (rand < 75) return 2;
            else return 3;
        }
        else
        {
            if (rand < 35) return 0;
            else if (rand < 70) return 1;
            else if (rand < 85) return 2;
            else return 3;
        }
    }

    void SubmitAnswer()
    {
        if (!isWaitingForSubmit) return;
        if (!answerInput) return;
        int playerAnswer;
        if (!int.TryParse(answerInput.text, out playerAnswer))
        {
            Debug.Log("Input tidak valid");
            return;
        }
        bool isCorrect = (playerAnswer == currentCorrectAnswer);
        Debug.Log($"Jawaban: {playerAnswer}, Benar: {isCorrect}");
        answerInput.gameObject.SetActive(false);
        isWaitingForSubmit = false;
        
        if (isCorrect)
        {
            lastRollTotal = currentCorrectAnswer;
            if (playerAttack) playerAttack.DealDiceDamage();
            else Debug.LogError("PlayerAttack missing");
        }
        else
        {
            EndPlayerTurnAndStartEnemy();
        }
    }

    void EndPlayerTurnAndStartEnemy()
    {
        isPlayerTurn = false;
        Debug.Log("Giliran enemy dimulai");
        if (enemyDiceRoller) enemyDiceRoller.StartEnemyTurn();
        else StartCoroutine(DelayedStartPlayerTurn(1f));
    }

    IEnumerator DelayedStartPlayerTurn(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartPlayerTurn();
    }

    public void OnEnemyTurnFinished()
    {
        StartPlayerTurn();
    }

    public void OnRollButtonClicked()
    {
        if (enemyDiceRoller != null && enemyDiceRoller.IsEnemyTurn())
        {
            Debug.Log("Giliran enemy, tombol tidak berfungsi.");
            return;
        }
        if (!isPlayerTurn) return;

        if (!isRollingQuestion && !isWaitingForSubmit)
        {
            StartRollingQuestion();
        }
        else if (isRollingQuestion)
        {
            isRollingQuestion = false;
            if (rollingCoroutine != null) StopCoroutine(rollingCoroutine);
            GenerateFinalQuestion();
            isWaitingForSubmit = true;
            if (answerInput)
            {
                answerInput.gameObject.SetActive(true);
                answerInput.text = "";
                answerInput.ActivateInputField();
            }
        }
        else if (isWaitingForSubmit)
        {
            SubmitAnswer();
        }
    }

    public int GetLastRollDamage() => lastRollTotal;
}

[System.Serializable]
public class PlayerData
{
    public string status;
    public int player_id;
    public string username;
    public int hp;
    public int base_damage;
    public int level = 0;
}
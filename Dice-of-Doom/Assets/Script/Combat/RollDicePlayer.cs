using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;

public class RollDicePlayer : MonoBehaviour
{
    [Header("DICE UI")]
    public GameObject[] diceObjects;  
    
        public TMP_Text[] diceTexts;      
    [Header("ROLL SETTING")]
    public float rollSpeed = 0.05f;

    [Header("API")]
    public string baseUrl = "http://localhost/yourfolder/get_player.php";

    private int baseDamage;
    private bool isRolling = false;
    private int diceCount;
    private int[] maxDice = new int[5];
    private int[] currentDice = new int[5];

    void Start()
    {
        StartCoroutine(GetPlayerFromAPI());
    }

    public void OnRollButtonClicked()
    {
        if (!isRolling)
        {
            StartCoroutine(StartRolling());
        }
        else
        {
            StopRollingAndCalculate();
        }
    }

    IEnumerator StartRolling()
    {
        isRolling = true;

        while (isRolling)
        {
            for (int i = 0; i < diceCount; i++)
            {
                currentDice[i] = Random.Range(0, maxDice[i] + 1);
                diceTexts[i].text = currentDice[i].ToString();
            }

            yield return new WaitForSeconds(rollSpeed);
        }
    }

    void StopRollingAndCalculate()
    {
        isRolling = false;

        int total = 0;

        for (int i = 0; i < diceCount; i++)
        {
            int value = int.Parse(diceTexts[i].text);
            total += value;
        }

        Debug.Log("FINAL DAMAGE = " + total);
    }

    IEnumerator GetPlayerFromAPI()
    {
        int playerId = PlayerPrefs.GetInt("player_id");
        string url = baseUrl + "?player_id=" + playerId;

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            PlayerData data = JsonUtility.FromJson<PlayerData>(www.downloadHandler.text);

            if (data.status == "success")
            {
                baseDamage = data.base_damage;
                Debug.Log("Base Damage = " + baseDamage);

                // ====== PENTING: SET DICE DI SINI ======
                diceCount = GetDiceCount(baseDamage);
                maxDice = GetMaxPerDice(baseDamage, diceCount);

                // aktifkan dice sesuai baseDamage
                for (int i = 0; i < diceObjects.Length; i++)
                {
                    diceObjects[i].SetActive(i < diceCount);
                }

                // isi angka awal (acak biar nggak kosong)
                for (int i = 0; i < diceCount; i++)
                {
                    currentDice[i] = Random.Range(0, maxDice[i] + 1);
                    diceTexts[i].text = currentDice[i].ToString();
                }
            }
            else
            {
                Debug.LogError("Gagal ambil data player!");
            }
        }
        else
        {
            Debug.LogError("Error koneksi: " + www.error);
        }
    }

    int GetDiceCount(int dmg)
    {
        if (dmg <= 8) return 1;
        if (dmg <= 14) return 2;
        if (dmg <= 20) return 3;
        return 5;
    }

    int[] GetMaxPerDice(int dmg, int diceCount)
    {
        int[] max = new int[5];

        if (diceCount == 1)
        {
            max[0] = dmg;
        }
        else if (diceCount == 2)
        {
            max[0] = 9;
            max[1] = 9;
        }
        else if (diceCount == 3)
        {
            int baseVal = dmg / 3;
            int sisa = dmg % 3;

            max[0] = baseVal + Mathf.Min(1, sisa);
            max[1] = baseVal + Mathf.Max(0, sisa - 1);
            max[2] = baseVal;
        }
        else // 5 dice
        {
            int baseVal = dmg / 5;
            int sisa = dmg % 5;

            for (int i = 0; i < 5; i++)
            {
                max[i] = baseVal + (i < sisa ? 1 : 0);
            }
        }

        return max;
    }
}

[System.Serializable]
public class PlayerData
{
    public string status;
    public int player_id;
    public string username;
    public int hp;
    public int base_damage;
}

using UnityEngine;
using UnityEngine.UI;

public class MoneyController : MonoBehaviour
{
    public static long money;
    public Text moneyText;

    void Start()
    {
    }

    void Update()
    {
        moneyText.text = money.ToString();
    }

    public void addMoney(long moneyToAdd)
    {
        AddGold(moneyToAdd);
    }

    public static void AddGold(long moneyToAdd)
    {
        money += moneyToAdd;

        int pointsToAdd = Mathf.Clamp((int)moneyToAdd, int.MinValue, int.MaxValue);
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddToCurrentScore(pointsToAdd);
        }
        else
        {
            int latest = PlayerPrefs.GetInt("LatestScore", 0);
            long newLatest = (long)latest + moneyToAdd;
            int newLatestClamped = Mathf.Clamp((int)newLatest, int.MinValue, int.MaxValue);
            PlayerPrefs.SetInt("LatestScore", newLatestClamped);

            int bestSoFar = PlayerPrefs.GetInt("BestScore", 0);
            if (newLatestClamped > bestSoFar)
            {
                PlayerPrefs.SetInt("BestScore", newLatestClamped);
            }
            PlayerPrefs.Save();
        }
    }

    public void substractMoney(long moneyToSubstract)
    {
        if (money - moneyToSubstract < 0)
        {
            Debug.Log("Cannot substract money !");
        }
        else
        {
            money -= moneyToSubstract;
        }
    }

    public bool canBuyItems(long moneyToSubstract)
    {
        if (money - moneyToSubstract < 0)
        {
            return false;
        }
        else return true;
    }
}

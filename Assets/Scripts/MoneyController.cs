using UnityEngine;
using UnityEngine.UI;

public class MoneyController : MonoBehaviour
{
    public static long money;
    public Text moneyText;

    void Start()
    {
        money = 300;
    }

    void Update()
    {
        moneyText.text = money.ToString();
    }

    public void addMoney(long moneyToAdd)
    {
        // Delegate to the static AddGold so there's one centralized implementation
        AddGold(moneyToAdd);
    }

    /// <summary>
    /// Static wrapper so other scripts can call MoneyController.AddGold(amount) instead of
    /// manipulating the static `money` field directly. This ensures score updates happen
    /// consistently.
    /// </summary>
    public static void AddGold(long moneyToAdd)
    {
        money += moneyToAdd;

        // Also add equivalent points to ScoreManager (points = gold earned)
        int pointsToAdd = Mathf.Clamp((int)moneyToAdd, int.MinValue, int.MaxValue);
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddToCurrentScore(pointsToAdd);
        }
        else
        {
            // Fallback: update LatestScore and BestScore directly in PlayerPrefs
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

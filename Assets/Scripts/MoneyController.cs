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
        money += moneyToAdd;
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

using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Economy : MonoBehaviour
{
    // ===== Public Variables =====================================================================
    
    [Header("City Information")]
    public int money;
    public int population;
    public int comBuildings;
    public int indBuildings;
    public float resTaxRate;
    public float comTaxRate;
    public float indTaxRate;
    
    [Space(10)]
    [Header("Demand")]

    public float resDemand;
    public float resWeight;
    public float comDemand;
    public float comWeight;
    public float indDemand;
    public float indWeight;

    [Space(10)]
    [Header("Time")]

    public float speed;
    public float dayLength;

    [Space(10)]
    [Header("UI")]

    public Text dayText;
    public Text moneyText;
    public Text speedText;


    // ===== Private Variables ====================================================================

    private DateTime date;

    // ===== Start ================================================================================
    
    private void Start ()
    {
        UpdateMoneyText();
        date = DateTime.Now;
        UpdateDemand();
        StartCoroutine(KeepTime());
    }

    // ===== Public Functions =====================================================================

    public void AddMoney (int amnt) 
    {
        money += amnt;
        UpdateMoneyText();
    }

    public void CollectTaxes () 
    {
        money += (int)(
                        (population * resTaxRate) + 
                        (comBuildings * comTaxRate) + 
                        (indBuildings * indTaxRate));
        UpdateMoneyText();
    }
    public void IncreasePopulation (int amnt) 
    { 
        population += amnt;
        UpdateMoneyText();
    }

    public void BuildComBuilding (int size) => comBuildings += size;
    public void BuildIndBuilding (int size) => indBuildings += size;
    public void UpdateMoneyText () => moneyText.text = String.Format("{0:# ### ### ###}", money);
    
    public void IncreaseSpeed ()
    {
        speed = speed == 3 ? 1 : speed + 1;
        speedText.text = speed.ToString() + "X";
    }

    public void UpdateDemand ()
    {
        float totalBuildings = (population * resWeight) + (comBuildings * comWeight) + (indBuildings * indWeight);

        resDemand = (population * resWeight) / totalBuildings;
        comDemand = (comBuildings * comWeight) / totalBuildings;
        indDemand = (indBuildings * indWeight) / totalBuildings;

        Debug.Log(resDemand + "; " + comDemand + "; " + indDemand);
    }

    // ===== Private Functions ====================================================================

    private IEnumerator KeepTime ()
    {
        while (true)
        {
            date = date.AddDays(1);
            
            if (date.Day == 1)
                CollectTaxes();

            dayText.text = date.ToString("dd / MMM / yyyy");
            yield return new WaitForSeconds(dayLength / speed);
        }
    }
}
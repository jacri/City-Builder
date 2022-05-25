using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Economy : MonoBehaviour
{
    // ===== Public Variables =====================================================================
    
    [Header("City Information")]

    public int population;
    public int comBuildings;
    public int indBuildings;

    [Space(10)]
    [Header("Economy")]

    public int money;
    public int monthlyOperatingCost;

    [Space(10)]

    public float comTaxRate;
    public float indTaxRate;
    public float resTaxRate;
    public float taxMultiplier;
    
    [Space(10)]
    [Header("Demand")]

    public float resDemand;
    public float indDemand;
    public float comDemand;

    [Space(10)]
    [Header("Time")]

    public float speed;
    public float dayLength;

    [Space(10)]
    [Header("UI")]

    public Text dayText;
    public Text popText;
    public Text moneyText;
    public Text speedText;

    [Space(10)]

    public Text resTaxText;
    public Text comTaxText;
    public Text indTaxText;

    [Space(10)]
    
    public Slider comDemandSlider;
    public Slider indDemandSlider;
    public Slider resDemandSlider;

    [Space(10)]
    [Header("Debug")]

    public bool enableEconomy = true;


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

    // ===== Economy ==============================================================================

    public void CollectTaxes ()  => money += (int)( taxMultiplier * 
                                                    (population * resTaxRate) + 
                                                    (comBuildings * comTaxRate) + 
                                                    (indBuildings * indTaxRate));

    public void SpendMonthlyOperatingCost () => money -= monthlyOperatingCost;

    public void MonthlyEconomyUpdate ()
    {
        CollectTaxes();
        SpendMonthlyOperatingCost();
        
        UpdateMoneyText();
    }

    public void SpendMoney (int amnt)
    {
        if (enableEconomy)
            money -= amnt;

        UpdateMoneyText();
    }

    public void StartMonthlyOperatingCost (int amnt) => monthlyOperatingCost += amnt;

    // ===== Buildings ============================================================================

    public void BuildZone (Zone.Type type, int density)
    {
        if (Zone.IsCommercial(type))
            BuildComBuilding(density);

        else if (Zone.IsIndustrial(type))
            BuildIndBuilding(density);

        else if (Zone.IsResidential(type))
            IncreasePopulation(density);

        UpdateDemand();
    }

    public void BuildComBuilding (int density) => comBuildings += density;
    public void BuildIndBuilding (int density) => indBuildings += density;
    public void IncreasePopulation (int density) 
    { 
        population += density;
        UpdatePopulationText();
    }

    // ===== UI ===================================================================================

    public void UpdatePopulationText () => popText.text = String.Format("{0:# ### ### ###}", population);
    public void UpdateMoneyText () => moneyText.text = String.Format("{0:# ### ### ###}", money);
    
    public void IncreaseSpeed ()
    {
        speed = speed == 3 ? 1 : speed + 1;
        speedText.text = speed.ToString() + "X";
    }

    public void UpdateDemand ()
    {
        float totalBuildings = population + comBuildings + indBuildings;

        if (totalBuildings == 0)
        {
            resDemand = 1;
            comDemand = 0;
            indDemand = 0;
        }

        else
        {
            resDemand = (totalBuildings - population) / totalBuildings;
            comDemand = (population - comBuildings) / totalBuildings;
            indDemand = (population - indBuildings) / totalBuildings;
        }

        resDemandSlider.value = resDemand;
        comDemandSlider.value = comDemand;
        indDemandSlider.value = indDemand;
    }

    public void UpdateResTaxFromUI (System.Single sliderVal)
    {
        resTaxText.text = $"{sliderVal:P0}";
        resTaxRate = (float)Math.Round(sliderVal, 2);
    }

    public void UpdateComTaxFromUI (System.Single sliderVal)
    {
        comTaxText.text = $"{sliderVal:P0}";
        comTaxRate = (float)Math.Round(sliderVal, 2);
    }

    public void UpdateIndTaxFromUI (System.Single sliderVal)
    {
        indTaxText.text = $"{sliderVal:P0}";
        indTaxRate = (float)Math.Round(sliderVal, 2);
    }

    // ===== Private Functions ====================================================================

    private IEnumerator KeepTime ()
    {
        while (true)
        {
            date = date.AddDays(1);
            
            if (date.Day == 1) 
                MonthlyEconomyUpdate();

            dayText.text = date.ToString("dd / MMM / yyyy");

            yield return new WaitForSeconds(dayLength / speed);
        }
    }
}
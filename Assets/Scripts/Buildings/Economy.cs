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
    public float comDemand;
    public float indDemand;

    [Space(10)]
    [Header("Time")]

    public float speed;
    public float dayLength;

    [Space(10)]
    [Header("UI")]

    public Text dayText;
    public Text moneyText;
    public Text speedText;
    public Slider resDemandSlider;
    public Slider comDemandSlider;
    public Slider indDemandSlider;


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

    public void CollectTaxes () 
    {
        money += (int)(
                        (population * resTaxRate) + 
                        (comBuildings * comTaxRate) + 
                        (indBuildings * indTaxRate));
        UpdateMoneyText();
    }

    public void BuildZone (Zone.Type type, int size)
    {
        if (type == Zone.Type.Residential)
            IncreasePopulation(size);

        else if (type == Zone.Type.Commercial)
            BuildComBuilding(size);

        else if (type == Zone.Type.Industrial)
            BuildIndBuilding(size);

        UpdateDemand();
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
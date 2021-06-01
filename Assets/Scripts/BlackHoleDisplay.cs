using System;
using UnityEngine;
using UnityEngine.UI;

public class BlackHoleDisplay : MonoBehaviour
{
    // Config Parameters
    [SerializeField] GameObject bh1Supermassive = null;
    [SerializeField] GameObject bh1Intermediate = null;
    [SerializeField] GameObject bh1Stellar = null;
    [SerializeField] GameObject bh2Supermassive = null;
    [SerializeField] GameObject bh2Intermediate = null;
    [SerializeField] GameObject bh2Stellar = null;

    [SerializeField] Text bh1Mass = null;
    [SerializeField] Text bh2Mass = null;

    [SerializeField] Text bh1SolarSymbol = null;
    [SerializeField] Text bh2SolarSymbol = null;

    [SerializeField] public float stellarMinMass = 3.8f;
    [SerializeField] public float stellarIntermediateMassBoundary = 100f;
    [SerializeField] public float intermediateSupermassiveMassBoundary = 100000.0f;
    [SerializeField] public float supermassiveMaxMass = 66000000000.0f;

    // Constants
    const string STELLAR_NAME = "stellar";
    const string INTERMEDIATE_NAME = "intermediate";
    const string SUPERMASSIVE_NAME = "supermassive";

    private void Start()
    {
        HideBlackHoleInformation();
    }

    public void DisplayBlackHoles(bool display, float mass1 = 0f, float mass2 = 0f)
    {
        if (display)
        {
            ShowBlackHoleInformation(mass1, mass2);
        }
        else
        {
            HideBlackHoleInformation();
        }
    }

    private void HideBlackHoleInformation()
    {
        bh1Supermassive.SetActive(false);
        bh1Intermediate.SetActive(false);
        bh1Stellar.SetActive(false);

        bh2Supermassive.SetActive(false);
        bh2Intermediate.SetActive(false);
        bh2Stellar.SetActive(false);

        bh1Mass.gameObject.SetActive(false);
        bh2Mass.gameObject.SetActive(false);

        bh1SolarSymbol.gameObject.SetActive(false);
        bh2SolarSymbol.gameObject.SetActive(false);
    }

    private void ShowBlackHoleInformation(float mass1, float mass2)
    {
        HideBlackHoleInformation();

        string bh1Type = GetBlackHoleType(mass1);
        string bh2Type = GetBlackHoleType(mass2);

        DisplayBlackHoleImages(bh1Type, bh2Type);

        bh1Mass.text = ScientificNotation(mass1);
        bh2Mass.text = ScientificNotation(mass2);

        bh1Mass.gameObject.SetActive(true);
        bh2Mass.gameObject.SetActive(true);

        bh1SolarSymbol.gameObject.SetActive(true);
        bh2SolarSymbol.gameObject.SetActive(true);
    }

    private void DisplayBlackHoleImages(string bh1Type, string bh2Type)
    {
        switch (bh1Type)
        {
            case STELLAR_NAME:
                bh1Stellar.SetActive(true);
                break;

            case INTERMEDIATE_NAME:
                bh1Intermediate.SetActive(true);
                break;

            case SUPERMASSIVE_NAME:
                bh1Supermassive.SetActive(true);
                break;

            default:
                Debug.LogError("Incorrect BH1 type of " + bh1Type + ". Try another mass.");
                break;
        }

        switch (bh2Type)
        {
            case STELLAR_NAME:
                bh2Stellar.SetActive(true);
                break;

            case INTERMEDIATE_NAME:
                bh2Intermediate.SetActive(true);
                break;

            case SUPERMASSIVE_NAME:
                bh2Supermassive.SetActive(true);
                break;

            default:
                Debug.LogError("Incorrect BH2 type of " + bh2Type + ". Try another mass.");
                break;
        }
    }

    private string GetBlackHoleType(float mass)
    {
        if (mass > stellarMinMass && mass <= stellarIntermediateMassBoundary)
        {
            return STELLAR_NAME;
        }
        else if (mass > stellarIntermediateMassBoundary && mass <= intermediateSupermassiveMassBoundary)
        {
            return INTERMEDIATE_NAME;
        }
        else if (mass > intermediateSupermassiveMassBoundary && mass <= supermassiveMaxMass)
        {
            return SUPERMASSIVE_NAME;
        }
        else
        {
            Debug.LogError("Incorrect BH mass of " + mass + ". Try another mass.");

            return null;
        }
    }

    private string ScientificNotation(float number)
    {
        string displayText = "";

        if (number > 1000f)
        {
            float basis = number / 1000f;
            int power = 3;

            while (basis >= 10f)
            {
                basis /= 10f;
                power++;

            }

            displayText = "" + (Mathf.RoundToInt(basis)).ToString() + "e" + power + " M";
        }
        else
        {
            displayText = (Mathf.RoundToInt(number)).ToString() + " M";
        }

        return displayText;
    }
}



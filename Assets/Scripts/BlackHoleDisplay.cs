﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlackHoleDisplay : MonoBehaviour
{
    // Config Parameters
    [SerializeField] GameObject bh1 = null;
    [SerializeField] GameObject bh2 = null;
    [SerializeField] GameObject bh3 = null;
    [SerializeField] float maxScale = 1.2f;
    [SerializeField] float minScale = 0.5f;
    [SerializeField] float massRetained = 0.95f;

    [SerializeField] TextMeshProUGUI bh1Mass = null;
    [SerializeField] TextMeshProUGUI bh2Mass = null;
    [SerializeField] TextMeshProUGUI bh3Mass = null;

    [SerializeField] TextMeshProUGUI nameBBH = null;

    [SerializeField] public float stellarMinMass = 3.8f;
    [SerializeField] public float stellarIntermediateMassBoundary = 100f;
    [SerializeField] public float intermediateSupermassiveMassBoundary = 100000.0f;
    [SerializeField] public float supermassiveMaxMass = 66000000000.0f;

    [SerializeField] string stellarBBH =      "Stellar-mass binary black hole";
    [SerializeField] string intermediateBBH = "Intermediate-mass binary black hole";
    [SerializeField] string supermassiveBBH = "Supermassive binary black hole";
    [SerializeField] string emriBBH =         "Extreme-mass-ratio inspiral (EMRI)";
    [SerializeField] string imriBBH =         "Intermediate-mass-ratio inspiral (IMRI)";
    [SerializeField] string smriBBH =         "Stellar-mass-ratio inspiral (SMRI)";

    // Constants
    const string STELLAR_NAME = "stellar";
    const string INTERMEDIATE_NAME = "intermediate";
    const string SUPERMASSIVE_NAME = "supermassive";

    // Cached References
    float coalescenceTime = 10f;
    Vector3 bh1InitialPosition, bh2InitialPosition, diffVector1, diffVector2;
    float bhDiff1, bhDiff2;
    RectTransform bh1RectTransform, bh2RectTransform, bh3RectTransform;

    private void Start()
    {
        bh1RectTransform = bh1.GetComponent<RectTransform>();
        bh2RectTransform = bh2.GetComponent<RectTransform>();
        bh3RectTransform = bh3.GetComponent<RectTransform>();

        bh1InitialPosition = bh1RectTransform.localPosition;
        bh2InitialPosition = bh2RectTransform.localPosition;

        HideBlackHoleInformation();

        diffVector1 = Vector3.Normalize(bh3RectTransform.localPosition - bh1InitialPosition);
        diffVector2 = Vector3.Normalize(bh3RectTransform.localPosition - bh2InitialPosition);

        bhDiff1 = Vector3.Distance(bh3RectTransform.localPosition, bh1InitialPosition);
        bhDiff2 = Vector3.Distance(bh3RectTransform.localPosition, bh2InitialPosition);

        coalescenceTime = FindObjectOfType<GravitationalWave>().coalescenceTime;
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
        bh1.SetActive(false);
        bh2.SetActive(false);
        bh3.SetActive(false);

        bh1RectTransform.localPosition = bh1InitialPosition;
        bh2RectTransform.localPosition = bh2InitialPosition;

        bh1Mass.transform.parent.gameObject.SetActive(false);
        bh2Mass.transform.parent.gameObject.SetActive(false);
        bh3Mass.transform.parent.gameObject.SetActive(false);

        nameBBH.gameObject.SetActive(false);
    }

    private void ShowBlackHoleInformation(float mass1, float mass2)
    {
        HideBlackHoleInformation();

        string bh1Type = GetBlackHoleType(mass1);
        string bh2Type = GetBlackHoleType(mass2);

        DisplayBlackHoleImages(GetScale(mass1), GetScale(mass2));

        bh1Mass.text = ScientificNotation(mass1);
        bh2Mass.text = ScientificNotation(mass2);
        bh3Mass.text = ScientificNotation(massRetained * (mass1 + mass2));

        nameBBH.text = GetBinaryType(bh1Type, bh2Type);

        bh1Mass.transform.parent.gameObject.SetActive(true);
        bh2Mass.transform.parent.gameObject.SetActive(true);
        bh3Mass.transform.parent.gameObject.SetActive(false);

        nameBBH.gameObject.SetActive(true);

        StartCoroutine(MoveBlackHoles());
    }

    private float GetScale(float mass)
    {
        float massRatio = (mass - stellarMinMass) / (supermassiveMaxMass - stellarMinMass);

        float scale = minScale + massRatio * (maxScale - minScale);

        return scale;
    }

    private string GetBinaryType(string bh1Type, string bh2Type)
    {
        if (bh1Type == STELLAR_NAME && bh2Type == STELLAR_NAME)
        {
            return stellarBBH;
        }
        else if (bh1Type == INTERMEDIATE_NAME && bh2Type == INTERMEDIATE_NAME)
        {
            return intermediateBBH;
        }
        else if (bh1Type == SUPERMASSIVE_NAME && bh2Type == SUPERMASSIVE_NAME)
        {
            return supermassiveBBH;
        }
        else if (bh1Type == STELLAR_NAME && bh2Type == INTERMEDIATE_NAME || bh2Type == STELLAR_NAME && bh1Type == INTERMEDIATE_NAME)
        {
            return smriBBH;
        }
        else if (bh1Type == INTERMEDIATE_NAME && bh2Type == SUPERMASSIVE_NAME || bh2Type == INTERMEDIATE_NAME && bh1Type == SUPERMASSIVE_NAME)
        {
            return imriBBH;
        }
        else if (bh1Type == STELLAR_NAME && bh2Type == SUPERMASSIVE_NAME || bh2Type == STELLAR_NAME && bh1Type == SUPERMASSIVE_NAME)
        {
            return emriBBH;
        }
        else
        {
            Debug.LogError("Invalid binary type with " + bh1Type + " and " + bh2Type + ".");
            return null;
        }
    }

    private void DisplayBlackHoleImages(float bh1Scale, float bh2Scale)
    {
        bh1.transform.localScale = new Vector3(bh1Scale, bh1Scale, 1f);
        bh2.transform.localScale = new Vector3(bh2Scale, bh2Scale, 1f);
        bh3.transform.localScale = new Vector3(massRetained * (bh1Scale + bh2Scale), massRetained * (bh1Scale + bh2Scale), 1f);

        bh1.SetActive(true);
        bh2.SetActive(true);
        bh3.SetActive(false);
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

            displayText = "" + (Mathf.RoundToInt(basis)).ToString() + "e" + power + " M<sub><b><size=120%><voffset=-.2em>⊙</voffset></size></b></sub>";
        }
        else
        {
            displayText = (Mathf.RoundToInt(number)).ToString() + " M<sub><b><size=120%><voffset=-.2em>⊙</voffset></size></b></sub>";
        }

        return displayText;
    }

    private IEnumerator MoveBlackHoles()
    {
        float timeElapsed = 0;

        while (timeElapsed <= coalescenceTime)
        {
            timeElapsed += Time.deltaTime;

            bh1RectTransform.localPosition += diffVector1 * bhDiff1 * (Time.deltaTime / coalescenceTime);
            bh2RectTransform.localPosition += diffVector2 * bhDiff2 * (Time.deltaTime / coalescenceTime);

            yield return null;
        }

        bh1.gameObject.SetActive(false);
        bh2.gameObject.SetActive(false);
        bh3.gameObject.SetActive(true);

        bh1Mass.transform.parent.gameObject.SetActive(false);
        bh2Mass.transform.parent.gameObject.SetActive(false);
        bh3Mass.transform.parent.gameObject.SetActive(true);
    }
}



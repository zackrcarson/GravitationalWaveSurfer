using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlackHoleDisplay : MonoBehaviour
{
    // Config Parameters
    [SerializeField] RectTransform bbhDisplayPanel = null;

    [SerializeField] GameObject bh1 = null;
    [SerializeField] GameObject bh2 = null;
    [SerializeField] GameObject bh3 = null;

    [SerializeField] GameObject pointerArrow = null;
    [SerializeField] float angleRotationTime = 0.5f;

    [SerializeField] float maxScale = 1.2f;
    [SerializeField] float minScale = 0.5f;
    [SerializeField] float massRetained = 0.95f;

    [SerializeField] float panelMaxHeight = 555f;
    [SerializeField] public float panelMovementTime = 1f;
    [SerializeField] public float warningMessageTime = 2f;

    [SerializeField] TextMeshProUGUI bh1Mass = null;
    [SerializeField] TextMeshProUGUI bh2Mass = null;
    [SerializeField] TextMeshProUGUI bh3Mass = null;

    [SerializeField] TextMeshProUGUI nameBBH = null;
    [SerializeField] GameObject warningText = null;

    [SerializeField] public float stellarMinMass = 3.8f;
    [SerializeField] public float stellarIntermediateMassBoundary = 100f;
    [SerializeField] public float intermediateSupermassiveMassBoundary = 100000.0f;
    [SerializeField] public float supermassiveMaxMass = 66000000000.0f;

    [SerializeField] string stellarBBH = "Stellar-mass binary black hole";
    [SerializeField] string intermediateBBH = "Intermediate-mass binary black hole";
    [SerializeField] string supermassiveBBH = "Supermassive binary black hole";
    [SerializeField] string emriBBH = "Extreme-mass-ratio inspiral (EMRI)";
    [SerializeField] string imriBBH = "Intermediate-mass-ratio inspiral (IMRI)";
    [SerializeField] string smriBBH = "Stellar-mass-ratio inspiral (SMRI)";

    // Constants
    const string STELLAR_NAME = "stellar";
    const string INTERMEDIATE_NAME = "intermediate";
    const string SUPERMASSIVE_NAME = "supermassive";

    // Cached References
    float coalescenceTime = 10f;
    float bhDiff1, bhDiff2;

    float panelMinHeight = 60f;
    float panelMinPosition, panelMaxPosition;
    float panelPositionX, panelWidth;

    Vector3 bh1InitialPosition, bh2InitialPosition, diffVector1, diffVector2;
    RectTransform bh1RectTransform, bh2RectTransform, bh3RectTransform;

    private void Start()
    {
        panelMinHeight = bbhDisplayPanel.rect.height;
        panelMinPosition = bbhDisplayPanel.localPosition.y;
        panelMaxPosition = panelMinPosition + (panelMaxHeight - panelMinHeight) / 2f;

        panelPositionX = bbhDisplayPanel.localPosition.x;
        panelWidth = bbhDisplayPanel.rect.width;

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

    public void DisplayBlackHoles(bool display, float mass1 = 0f, float mass2 = 0f, float angle = 0f)
    {
        if (display)
        {
            StartCoroutine(OpenBlackHolePanel(mass1, mass2, angle));
        }
        else
        {
            StartCoroutine(CloseBlackHolePanel());
        }
    }

    private IEnumerator OpenBlackHolePanel(float mass1, float mass2, float angle)
    {
        HideBlackHoleInformation();

        warningText.SetActive(true);
        yield return new WaitForSeconds(warningMessageTime);

        float timeElapsed = 0;
        Vector2 panelHeightDelta = new Vector2(0f, 0f);
        Vector3 panelPositionDelta = new Vector3(0f, 0f, 0f);

        while (timeElapsed <= panelMovementTime)
        {
            timeElapsed += Time.deltaTime;

            panelHeightDelta.y = (panelMaxHeight - panelMinHeight) * (Time.deltaTime / panelMovementTime);
            panelPositionDelta.y = (panelMaxHeight - panelMinHeight) * (Time.deltaTime / panelMovementTime);

            bbhDisplayPanel.sizeDelta += panelHeightDelta;
            bbhDisplayPanel.localPosition += panelPositionDelta / 2f;

            yield return null;
        }

        bbhDisplayPanel.localPosition = new Vector3(panelPositionX, panelMaxPosition, 0f);
        bbhDisplayPanel.sizeDelta = new Vector2(panelWidth, panelMaxHeight);

        yield return new WaitForSeconds(warningMessageTime);

        ShowBlackHoleInformation(mass1, mass2, angle);
    }

    private IEnumerator CloseBlackHolePanel()
    {
        HideBlackHoleInformation();

        float timeElapsed = 0;
        Vector2 panelHeightDelta = new Vector2(0f, 0f);
        Vector3 panelPositionDelta = new Vector3(0f, 0f, 0f);

        while (timeElapsed <= panelMovementTime)
        {
            timeElapsed += Time.deltaTime;

            panelHeightDelta.y = (panelMaxHeight - panelMinHeight) * (Time.deltaTime / panelMovementTime);
            panelPositionDelta.y = (panelMaxHeight - panelMinHeight) * (Time.deltaTime / panelMovementTime);

            bbhDisplayPanel.sizeDelta -= panelHeightDelta;
            bbhDisplayPanel.localPosition -= panelPositionDelta / 2f;

            yield return null;
        }

        bbhDisplayPanel.localPosition = new Vector3(panelPositionX, panelMinPosition, 0f);
        bbhDisplayPanel.sizeDelta = new Vector2(panelWidth, panelMinHeight);
    }

    private void HideBlackHoleInformation()
    {
        bh1.SetActive(false);
        bh2.SetActive(false);
        bh3.SetActive(false);

        pointerArrow.transform.parent.gameObject.SetActive(false);

        bh1RectTransform.localPosition = bh1InitialPosition;
        bh2RectTransform.localPosition = bh2InitialPosition;

        bh1Mass.transform.parent.gameObject.SetActive(false);
        bh2Mass.transform.parent.gameObject.SetActive(false);
        bh3Mass.transform.parent.gameObject.SetActive(false);

        nameBBH.gameObject.SetActive(false);
    }

    private void ShowBlackHoleInformation(float mass1, float mass2, float angle)
    {
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

        pointerArrow.transform.parent.gameObject.SetActive(true);
        StartCoroutine(RotateArrow(angle));

        warningText.SetActive(false);
        nameBBH.gameObject.SetActive(true);

        StartCoroutine(MoveBlackHoles());
    }

    private IEnumerator RotateArrow(float newAngle)
    {
        float startAngle = pointerArrow.transform.localEulerAngles.z;
        float deltaAngle = newAngle - pointerArrow.transform.localEulerAngles.z;

        if (deltaAngle <= -180f)
        {
            deltaAngle += 360f;
        }
        else if (deltaAngle >= 180f)
        {
            deltaAngle -= 360f;
        }

        Vector3 currentAngle = new Vector3(0f, 0f, 0f);

        float t = 0f;
        while (t < angleRotationTime)
        {
            t += Time.deltaTime;

            currentAngle.z = startAngle + deltaAngle * (t / angleRotationTime);

            pointerArrow.transform.localEulerAngles = currentAngle;

            yield return null;
        }

        currentAngle.z = newAngle;
        pointerArrow.transform.localEulerAngles = currentAngle;
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



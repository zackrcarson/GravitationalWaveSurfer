using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GWS.Gameplay;
using GWS.UI.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GWS.AtomCreation.Runtime
{
    public class AtomFormationManager : MonoBehaviour
    {
        [SerializeField] private AtomInfo currentElement;

        [SerializeField] private Image elementBox;
        [SerializeField] private TextMeshProUGUI atomicNumber;
        [SerializeField] private TextMeshProUGUI atomicSymbol;
        [SerializeField] private TextMeshProUGUI elementName;
        [SerializeField] private TextMeshProUGUI atomicMass;

        [SerializeField] private TextMeshProUGUI warningMessage;
        [SerializeField] private TextMeshProUGUI finishMessage;
        [SerializeField] private TextMeshProUGUI numElementsLeft;

        [SerializeField] private AtomIndex formationIndex;

        [SerializeField] private AudioClip correctAnswerSound;
        [SerializeField] private AudioClip onClickSound;
        [SerializeField] private AudioSource constantAudioSource;
        [SerializeField] private AudioSource pitchChangingAudioSource;

        private const int NumRandomElements = 2;
        private int currElementIndex = 0;
        private List<AtomInfo> elementsToForm;

        private bool canInteract = true;

        public static Action OnComplete;

        private void OnEnable()
        {
            CreationManagement.OnParticlesChanged += HandleParticleChange;
            CreationManagement.OnWarningRaised += HandleWarning;
        }

        private void OnDisable()
        {
            CreationManagement.OnParticlesChanged -= HandleParticleChange;
            CreationManagement.OnWarningRaised -= HandleWarning;
        }

        private void Start()
        {
            InitializeElementSequence();
            SetCurrentElement(elementsToForm[currElementIndex]);
            finishMessage.text = "";
        }

        private void InitializeElementSequence()
        {
            elementsToForm = new List<AtomInfo> { AtomInfo.Order[formationIndex.CurrentAtomIndex] };
            elementsToForm.AddRange(GetRandomAtoms(NumRandomElements));
        }

        private void HandleParticleChange(int currentProtons, int currentNeutrons, int currentElectrons)
        {
            if (!canInteract) return;
            if (currentElement.Protons == currentProtons &&
                currentElement.Neutrons == currentNeutrons &&
                currentElement.Electrons == currentElectrons)
            {
                PlayerFormedElement();
            }
        }

        private void PlayerFormedElement()
        {
            Outcome atom = elementsToForm[currElementIndex].Atom;

            UnlockManager.Instance.UnlockOutcome(atom);
            currElementIndex++;
            constantAudioSource.PlayOneShot(correctAnswerSound);

            if (currElementIndex < elementsToForm.Count)
            {
                SetCurrentElement(elementsToForm[currElementIndex]);
            }
            else
            {
                CompletedAllElements();
            }
        }

        private void CompletedAllElements()
        {
            //Debug.Log("All elements completed!");
            formationIndex.CurrentAtomIndex++;
            finishMessage.text = $"Good job!\r\n\r\nThe next element in order is:\r\n\r\n<color=#FFBAB5>{AtomInfo.Order[formationIndex.CurrentAtomIndex].FullName}</color>\r\n\r\nNow returning...";
            SetNumElementsLeftText();
            StartCoroutine(Wait());
            canInteract = false;
        }

        private IEnumerator Wait()
        {
            yield return new WaitForSeconds(2f);
            EmitComplete();
        }

        private void EmitComplete()
        {
            OnComplete?.Invoke();
        }

        private void HandleWarning(string message)
        {
            warningMessage.text = message;
        }

        private void SetCurrentElement(AtomInfo element)
        {
            currentElement = element;
            SetElementUI();
        }

        private void SetElementUI()
        {
            atomicNumber.text = currentElement.Protons.ToString();
            atomicSymbol.text = currentElement.Atom.ToString();
            elementName.text = currentElement.FullName;
            atomicMass.text = currentElement.Mass.ToString();

            // if this is the first element (aka one in the order of star fusion), color it special
            if (currElementIndex == 0)
            {
                elementBox.color = DisplayElementUnlock.WeirdOrange;
            }
        
            else
            {
                elementBox.color = Color.white;
            }

            SetNumElementsLeftText();
        }

        private void SetNumElementsLeftText()
        {
            numElementsLeft.text = (NumRandomElements - currElementIndex + 1).ToString();
        }

        private static AtomInfo[] GetRandomAtoms(int count)
        {
            // randomly grab 5 elements => order those elements by mass 
            return AtomInfo.AllRandomAtoms.OrderBy(atom => Random.value).Take(count).OrderBy(atom => atom.Mass).ToArray();
        }

        public void PlayOnClick()
        {
            pitchChangingAudioSource.pitch = Random.Range(0.9f, 1.1f);
            pitchChangingAudioSource.PlayOneShot(onClickSound);
        }
    }

}

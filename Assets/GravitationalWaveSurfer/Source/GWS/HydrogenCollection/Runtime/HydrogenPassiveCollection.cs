using System.Collections;
using UnityEngine;

namespace GWS.HydrogenCollection.Runtime
{
    public class HydrogenPassiveCollection : MonoBehaviour
    {
        public static HydrogenPassiveCollection Instance { get; private set; }

        [Header("Amount collected passively per sec")]
        public double passiveCollection = 0;

        [Space(6)]
        [Header("(Debug) Toggle passive collection")]
        public bool passiveCollect = true;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            if (passiveCollect)
            {
                StartCoroutine(TickEverySecond());
            }
        }

        private IEnumerator TickEverySecond()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                CollectPassiveHydrogen();
            }
        }

        private void CollectPassiveHydrogen()
        {
            HydrogenManager.Instance.AddHydrogen(passiveCollection);
        }

        /// <summary>
        /// Change amount collected passively
        /// </summary>
        /// <param name="value">int, negative for decrease</param>
        public void ChangePassiveCollection(double value)
        {
            passiveCollection += value;
        }
    }
}
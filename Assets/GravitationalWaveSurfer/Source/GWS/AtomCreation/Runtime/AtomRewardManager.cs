using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GWS.HydrogenCollection.Runtime;
using GWS.GameStage.Runtime;
using System;
using Random = UnityEngine.Random;

namespace GWS.AtomCreation.Runtime
{
    public class AtomRewardManager : MonoBehaviour
    {
        private bool usePassiveReward = false;
        public static Action<String> OnCreateRewardMessage;

        private void Start()
        {
            AtomFormationManager.OnComplete += GrantReward;
        }

        private void OnDestroy()
        {
            AtomFormationManager.OnComplete -= GrantReward;
        }

        private void GrantReward()
        {
            usePassiveReward = Random.value > 0.5f;
            double adjustedReward = CalculateReward();

            if (usePassiveReward)
            {
                HydrogenPassiveCollection.Instance.ChangePassiveCollection(adjustedReward);
                AtomFormationManager.rewardMessage = $"Granted passive reward: +{adjustedReward} Mass/sec";
            }
            else
            {
                HydrogenManager.Instance.AddHydrogen(adjustedReward);
                AtomFormationManager.rewardMessage = $"Granted one-time reward: +{adjustedReward} Mass";
            }

            GameStageManager.Instance.GameStageIncQuiz();
        }

        private double CalculateReward()
        {
            // To-do: add .asm to environment file so i can reference these ranges from ChunkManager instead of hard coding
            if (usePassiveReward)
            {
                return Random.Range(1, 5);
            }
            else
            {
                return Random.Range(100, 1000);
            }
        }
    }
}


using GWS.CollisionEvents.Runtime;
using UnityEngine;

namespace GWS.HydrogenCollection.Runtime
{
    public class ParticleLossOnCollision: MonoBehaviour
    {
        [SerializeField]
        private CollisionEventChannel collisionEventChannel;

        [SerializeField] 
        private ParticleInventory particleInventory;

        private void OnEnable()
        {
            collisionEventChannel.OnTriggerEnter += HandleOnTriggerEnter;
        }
        
        private void OnDisable()
        {   
            collisionEventChannel.OnTriggerEnter -= HandleOnTriggerEnter;
        }

        private void HandleOnTriggerEnter(Collider _)
        {
            particleInventory.HydrogenCount -= 10;
        }
    }
}
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GWS.ParticleSystem.Runtime.DirectionGeneration;
using GWS.ParticleSystem.Runtime.Shapes;
using JetBrains.Annotations;
using UnityEngine;

namespace GWS.ParticleSystem.Runtime
{
    /// <summary>
    /// Creates a particle system of items from a <see cref="ParticlePool"/>
    /// </summary>
    public class ParticlePoolSystem: MonoBehaviour 
    {
        [Header("Particle System")]
        [SerializeField]
        private ParticlePool pool;
        
        /// <summary>
        /// How long each particle lives in seconds.
        /// </summary>
        [SerializeField, Min(0)] 
        private float lifetime;

        /// <summary>
        /// The initial linear velocity of each particle.
        /// </summary>
        [SerializeField, Min(0)] 
        private float initialLinearVelocity;

        /// <summary>
        /// Whether or not to play the particle system on <see cref="Start"/>.
        /// </summary>
        [SerializeField] 
        private bool playOnStart;
        
        /// <summary>
        /// The rate at which to spawn particles in seconds. 
        /// </summary>
        [Header("Emission")]
        [SerializeField, Min(0)] 
        private float rateOverTime;

        /// <summary>
        /// The number of particles to emit at once. 
        /// </summary>
        [SerializeField, Min(1)] 
        private int burstCount;

        /// <summary>
        /// The <see cref="IShape"/> through which this should emit particles.
        /// </summary>
        [UsedImplicitly, SerializeReference, SubclassSelector]
        private IDirectionGenerator directionGenerator;

        private CancellationTokenSource resetCancellation;

        private void Awake()
        {
            pool.Prewarm();
            burstCount = Mathf.Min(burstCount, pool.MaxPoolSize);
        }

        private void Start()
        {
            if (!playOnStart) return;
            Play();
        }

        public void Stop()
        {
            resetCancellation.Dispose();
            resetCancellation.Cancel();
        }

        public void Play()
        {
            resetCancellation = new CancellationTokenSource();
            _ = UpdateSystem(resetCancellation.Token);
        }

        private async UniTaskVoid UpdateSystem(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var particles = new ParticleBase[burstCount];
                
                for (var i = 0; i < burstCount; i++)
                {
                    if (cancellationToken.IsCancellationRequested || pool == null) return;
                    var particleArgs = new ParticleArgs(pool.Parent.position, 
                        transform.rotation * directionGenerator?.GetDirection() ?? Vector3.zero, 
                        initialLinearVelocity);
                    
                    particles[i] = pool.Allocate(particleArgs);

                    if (!ReferenceEquals(particles[i], null)) continue;
                    await UniTask.WaitUntil(() => !pool.IsCompletelyInUse, cancellationToken: cancellationToken);
                }

                foreach (var particle in particles)
                {
                    if (particle == null) continue;
                    particle.gameObject.SetActive(true);
                    _ = FreeParticleAfterSeconds(particle, cancellationToken);
                }
                
                await UniTask.Delay(TimeSpan.FromSeconds(rateOverTime), ignoreTimeScale: false, cancellationToken: cancellationToken);
            }
        }

        private async UniTaskVoid FreeParticleAfterSeconds(ParticleBase particle, CancellationToken cancellationToken)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(lifetime), ignoreTimeScale: false, cancellationToken: cancellationToken);
            particle.gameObject.SetActive(false);
            pool.Free(particle);
        }
    }
}
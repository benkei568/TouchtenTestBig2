using UnityEngine;
using ObjectPooling;

namespace VFXSystem
{
    public class VFXAutoDestroy : MonoBehaviour
    {
        public ParticleSystem lastDurationParticle;
        public ParticleSystem lastLifetimeParticle;

        private float durationRemaining;
        
        private void OnEnable()
        {
            if (lastDurationParticle == null)
                lastDurationParticle = GetComponentInChildren<ParticleSystem>();
            if (lastLifetimeParticle == null)
                lastLifetimeParticle = GetComponentInChildren<ParticleSystem>();
            if (lastDurationParticle != null)
            {
                durationRemaining = lastDurationParticle.main.duration + lastLifetimeParticle.main.startLifetime.constantMax;
            }
            else
            {
                Debug.LogWarning("VFXAutoDestroy on gameobject " + gameObject.name + " is invalid because gameobject doesn't have any ParticleSystem, will be immediately deactivated");
            }
        }
        
        void Update()
        {
            if (gameObject.activeInHierarchy) {
                durationRemaining -= Time.deltaTime;
                if (durationRemaining <= 0)
                {
                    ObjectPools.Instance.DeactivateObject(gameObject);
                }
            }
        }
    }
}

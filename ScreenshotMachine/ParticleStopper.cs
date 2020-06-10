using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace ScreenshotMachine
{
    public static class ParticleStopper
    {
        private const float  DisplayTime = 60f;
        private const float StoppedTime = 20f;

        private static readonly List<Coroutine> CoroutinesRunning = new List<Coroutine>();

        private static readonly List<ParticleSystem> StoppedParticles = new List<ParticleSystem>();

        /* Not needed anymore now that I just scan for the ParticleController type, but these are the names of the particles that are part of the camera and not the scene
         * Might be useful in the future
         
        private static readonly string[] ParticleNames =
        {
            "royal_garden_particles",
            "white_palace_particles",
            "town_particle_set",
            "outskirts_particles",
            "default_particles",
            "fungus_particles",
            "mines_particles",
            "fog_canyon_particles",
            "waterways_particles",
            "ruins_interior_particles",
            "dream_particles",
            "resting_grounds_particles",
            "hive_drip_particles",
            "fungal_wastes_particles",
            "Deepnest Particles",
            "abyss particles"
        };
        */
        
        
        public static void CallFreezeParticles(Scene arg0, LoadSceneMode loadSceneMode)
        {
            GameManager.instance.StartCoroutine(FreezeParticles());
        }

        public static void StopFreeze(On.GameManager.orig_BeginSceneTransition orig, GameManager self, GameManager.SceneLoadInfo info)
        {
            foreach (ParticleSystem particles in StoppedParticles)
            {
                if(particles == null)
                    continue;
                particles.Play(); //Play the particles in case they were off due to the class stopping them
            }
            
            foreach (Coroutine coroutine in CoroutinesRunning)
            {
                GameManager.instance.StopCoroutine(coroutine);
            }
            
            orig(self, info);
        }
        private static IEnumerator FreezeParticles()
        {
            
           
            yield return null; // Wait 1 frame wait for sceneLoaded hook

            yield return new WaitForSeconds(DisplayTime);
            
            CoroutinesRunning.Add(GameManager.instance.StartCoroutine(StopParticles(UnityEngine.Object.FindObjectsOfType<ParticleSystem>().Where(particle => particle.isPlaying).ToArray())));  
            
        }

        private static IEnumerator StopParticles(ParticleSystem[] particleSystems)
        {

                float[] ratesOverTimeMultipliers = new float[particleSystems.Length]; //saved for when we make the particles exist again

                for (int i = 0; i < particleSystems.Length; ++i)
                {

                    ratesOverTimeMultipliers[i] = particleSystems[i].emission.rateOverTimeMultiplier; //saved for when we make the particles exist again
                    particleSystems[i].Stop();
                    StoppedParticles.Add(particleSystems[i]);
                }

                yield return new WaitForSeconds(StoppedTime); //Wait for the particles to disappear

                for (int i = 0; i < particleSystems.Length; ++i) //Start the particles back up. This should probably be a different method and not one called StopParticles lol
                {

                    if (particleSystems[i]==null)
                        continue;
                    ParticleSystem.EmissionModule emissionModule = particleSystems[i].emission;
                    emissionModule.rateOverTimeMultiplier = 0f;
                    particleSystems[i].randomSeed = (uint) i;
                    particleSystems[i].Play();
                    StoppedParticles.Remove(particleSystems[i]);
                    GameManager.instance.StartCoroutine(Lerp(particleSystems[i].emission.rateOverTimeMultiplier, ratesOverTimeMultipliers[i], 0, emissionModule));

                }
        }

        private static IEnumerator Lerp(float start, float end, float t, ParticleSystem.EmissionModule emissionModule)
        {
            float lerpRate = end/((DisplayTime+1 - (DisplayTime*9/10)) * 60); //Increase the particles at a variable rate so that every particle can reach the maximum amount. 
            //we do +1 to avoid division by 0 errors. DisplayTime*9/10 means that we will display the particles in full a 10% of the DisplayTime, and lerp them up during the rest
            while (t < 1){
                try
                {
                    emissionModule.rateOverTimeMultiplier = Mathf.Lerp(start, end, t);

                }
                catch (NullReferenceException) //If we change scene while lerping and a particle controller disappears, the NulLReferenceException thrown will kill other lerps that may still be useful, interrupting them and bleeding particles
                {
                    yield break;
                }
                t += lerpRate;
                yield return new WaitForSeconds(0.16f);
            }

            emissionModule.rateOverTimeMultiplier = end; //make sure we don't bleed particles due to lerpRate not adding up to 1 cleanly

        }
    }
}
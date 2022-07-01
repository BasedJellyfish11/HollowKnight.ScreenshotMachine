using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GlobalEnums;
using Modding;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace ScreenshotMachine
{
    public static class ParticleStopper
    {
        private static readonly Dictionary<ParticleSystem, float> LoopingParticles = new();

        public static NonBouncer CoroutineStarter;

        private static readonly PhysLayers[] LayersToStop =
        {
            PhysLayers.DEFAULT,
            PhysLayers.GRASS,
            PhysLayers.WATER,
            PhysLayers.ITEM
        };

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

        public static void CallFreezeParticles(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.name == GameManager.GetBaseSceneName(scene.name))
                CoroutineStarter.StartCoroutine(FindParticles());
        }

        private static IEnumerator FindParticles()
        {
            // Wait 1 frame wait for sceneLoaded hook
            yield return null;

            LoopingParticles.Clear();

            ScreenshotMachine.Log
                ("Finding particles for scene " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

            foreach (ParticleSystem particleSystem in Object.FindObjectsOfType<ParticleSystem>().Where
            (
                particle => particle.isPlaying
                    && !IsParentedToLiving(particle.transform)
                    && LayersToStop.Contains((PhysLayers) particle.gameObject.layer)
            ))
            {
                if (particleSystem == null)
                    continue;
                
                LoopingParticles.Add(particleSystem, particleSystem.emission.rateOverTimeMultiplier);
            }

            CoroutineStarter.StartCoroutine(StopParticles());
        }

        private static IEnumerator StopParticles()
        {
            foreach (ParticleSystem particles in LoopingParticles.Keys.Where(particles => particles != null))
            {
                ScreenshotMachine.Log("Stopping particle " + particles.name);
                particles.Stop();
            }

            yield return new WaitForSeconds(ScreenshotMachine.Settings.StoppedTime); // Wait the stopped time

            foreach (ParticleSystem key in LoopingParticles.Keys)
            {
                if (key == null)
                    continue;

                var particleSystems = new ParticleSystem.Particle[key.particleCount];
                key.GetParticles(particleSystems);
                
                float extraWait = 0f;
                
                foreach (ParticleSystem.Particle particle in particleSystems)
                {
                    if (particle.remainingLifetime > 0f && particle.remainingLifetime > extraWait)
                        extraWait = particle.remainingLifetime;
                }

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (extraWait != 0f)
                {
                    ScreenshotMachine.Log
                    (
                        "The established stopped time wasn't enough for all the particles to disappear!"
                        + $" Waiting an extra {extraWait} seconds"
                    );
                }

                yield return new WaitForSeconds(extraWait);

                ParticleSystem.EmissionModule emissionModule = key.emission;
                emissionModule.rateOverTimeMultiplier = 0f;
            }

            CoroutineStarter.StartCoroutine(StartParticles());
        }

        private static IEnumerator StartParticles()
        {
            ScreenshotMachine.Log("Starting particles back up");

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (ScreenshotMachine.Settings.DisplayTime == 0f)
                yield break;

            float startTime = Time.time;

            foreach (ParticleSystem key in LoopingParticles.Keys.Where(key => key != null))
            {
                if (ScreenshotMachine.Settings.AlwaysUseSameSeedForParticles)
                    key.randomSeed = 1;

                key.time = 0f;
                key.Play();
            }

            float t = 0f;

            while (t <= 1f)
            {
                foreach ((ParticleSystem system, float max) in LoopingParticles.Where(pair => pair.Key != null))
                {
                    if (!system.isPlaying)
                        system.Play();

                    ParticleSystem.EmissionModule emissionModule = system.emission;
                    emissionModule.rateOverTimeMultiplier = Mathf.Lerp(0, max, t);
                }

                t = (Time.time - startTime) / (ScreenshotMachine.Settings.DisplayTime);
                yield return null;
            }

            foreach ((ParticleSystem system, float rate) in LoopingParticles)
            {
                ScreenshotMachine.Log("End of loop");

                if (system == null)
                    continue;

                ParticleSystem.EmissionModule emissionModule = system.emission;

                emissionModule.rateOverTimeMultiplier = rate;
            }

            CoroutineStarter.StartCoroutine(StopParticles());
        }

        public static void StopFreeze
        (
            On.GameManager.SceneLoadInfo.orig_NotifyFetchComplete orig,
            GameManager.SceneLoadInfo sceneLoadInfo
        )
        {
            CoroutineStarter.StopAllCoroutines();

            foreach ((ParticleSystem system, float rate) in LoopingParticles.Where(pair => pair.Key != null))
            {
                // Play the particles in case they were off due to the class stopping them
                system.Play();

                ParticleSystem.EmissionModule emissionModule = system.emission;
                emissionModule.rateOverTimeMultiplier = rate;
            }

            LoopingParticles.Clear();

            orig(sceneLoadInfo);
        }

        private static bool IsParentedToLiving(Transform objectToCheck)
        {
            while (
                objectToCheck != null
                && objectToCheck.name != "Knight"
                && objectToCheck.gameObject.GetComponent<HealthManager>() == null
            )
            {
                objectToCheck = objectToCheck.parent;
            }

            return objectToCheck != null;
        }
    }
}
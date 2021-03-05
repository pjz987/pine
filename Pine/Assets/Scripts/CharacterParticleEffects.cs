using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class CharacterParticleEffects : MonoBehaviour
{
     public GameObject vfxPineconeSaplingGrowth;
     public GameObject vfxTreeFlick;
     public GameObject vfxPineconeGroundImpact;
     public GameObject vfxPineconeSmallGroundImpact;

     public List<GameObject> currentSystems = new List<GameObject>();


     private void Update()
     {
          // If the list of particle systems is 0, don't do anything.
          if (currentSystems.Count == 0)
               return;

          // For every particle system, check if any particles exist for that system. If not, destroy that object.
          for (int i = 0; i < currentSystems.Count; i++)
          {
               if (currentSystems[i].GetComponent<ParticleSystem>().IsAlive() == false)
               {
                    GameObject systemToDestroy = currentSystems[i];
                    currentSystems.Remove(currentSystems[i]);         // Remove from list
                    Destroy(systemToDestroy);                         // Destroy object
               }
          }
     }


     bool CheckVFX(GameObject vfx)
     {
          // Check if a vfx object is attached.
          if (vfx == null)
          {
               Debug.Log("Failed to play vfx: No vfx object attached.");
               return false;
          }

          // Check if a particle system is attached to the object.
          ParticleSystem vfxCurrent = vfx.GetComponent<ParticleSystem>();
          if (vfxCurrent == null)
          {
               Debug.Log("Failed to play vfx: No particle system attached to vfx object.");
               return false;
          }

          // If both checks were passed, then able to play vfx.
          return true;
     }



     public void PlayVFX(GameObject vfx, Vector3 creationPosition, float strength = 0f)
     {
          // Check if any vfx is attached.
          if (CheckVFX(vfx) == false)
               return;

          // Create the new particle system and add it to the list of systems.
          GameObject newSystem = Instantiate(vfx, creationPosition, Quaternion.identity);
          currentSystems.Add(newSystem);

          ParticleSystem vfxCurrent = newSystem.GetComponent<ParticleSystem>();
          int emitAmount = vfxCurrent.emission.burstCount;
          vfxCurrent.Emit(emitAmount);
     }
}

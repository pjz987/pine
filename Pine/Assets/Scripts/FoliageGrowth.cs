using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoliageGrowth : MonoBehaviour
{

     float additionalHeight = 0.5f;
     float cutoffDistance = 0.51f;

     public LayerMask layerMask;
     public GameObject[] foliageGroups;



     /// <summary>
     /// Creates a randomly selected foliage group and places it on the ground below the controller.
     /// It rotates the group and each individual object within the group. If there is no ground
     /// below the individual object, it's deactivated.
     /// </summary>
     public void GenerateFoliage(Transform foliageParent)
     {
          Vector3 groundPosition = FindGroundPosition();
          int randomGroup = Random.Range(0, foliageGroups.Length);

          // Create an instance of one of the foliage groups and rotate the foliage group along its y-axis.
          GameObject thisFoliageGroup = Instantiate(foliageGroups[randomGroup], groundPosition, Quaternion.identity);
          RotateAlongY(thisFoliageGroup.transform);

          // Get the 'GrowRange' child - First child of the foliage group.
          Transform growRange = thisFoliageGroup.transform.GetChild(0);

          // Loop through all the children of the GrowRange.
          int numberOfChildren = growRange.childCount;
          for (int i = 0; i < numberOfChildren; i++)
          {
               Transform currentChild = growRange.GetChild(i);

               // If this child has any children,
               if (currentChild.childCount != 0)
               {
                    // Raycast from every child position in this object.
                    for (int j = 0; j < currentChild.childCount; j++)
                    {
                         Transform raycastChild = currentChild.GetChild(j);

                         // If any of these are too far from the ground, then hide this object.
                         if (DetectGroundBelow(raycastChild) == false)
                              currentChild.gameObject.SetActive(false);
                    }
               }

               // If this child has NO children, raycast down from its own position
               else
               {
                    // If this is too far from the ground, then hide this object.
                    if (DetectGroundBelow(currentChild) == false)
                         currentChild.gameObject.SetActive(false);

                    // Rotate this child along its y-axis
                    RotateAlongY(currentChild);
               }
          }

          // After finishing the generation, put this foliage group under the foliageParent.
          thisFoliageGroup.transform.parent = foliageParent;
          thisFoliageGroup.name = "foliageGroup_" + foliageParent.childCount;
     }


     
     /// <summary>
     /// Returns false if no ground is detected below object, true otherwise.
     /// </summary>
     /// <param name="objectToCheck"></param>
     bool DetectGroundBelow(Transform objectToCheck)
     {
          RaycastHit hit;
          float checkX = objectToCheck.position.x;
          float checkY = objectToCheck.root.position.y + additionalHeight;
          float checkZ = objectToCheck.position.z;
          Vector3 positionToCheck = new Vector3(checkX, checkY, checkZ);
         
          Physics.Raycast(positionToCheck, Vector3.down, out hit, 1000f, layerMask);

          // If there is no ground detected within the cutoff distance, then return false.
          if (hit.distance >= cutoffDistance)
               return false;

          return true;
     }



     /// <summary>
     /// Find the ground position below the attached transform.
     /// </summary>
     /// <returns></returns>
     Vector3 FindGroundPosition()
     {
          RaycastHit hit;
          Physics.Raycast(this.transform.position, Vector3.down, out hit);

          return hit.point;
     }



     /// <summary>
     /// Rotate the object around its y-axis by a random amount between 0 and 359.
     /// </summary>
     /// <param name="objectToRotate"></param>
     void RotateAlongY(Transform objectToRotate)
     {
          // Randomize the y rotation
          float newY = Random.Range(0f, 359f);
          float currentX = objectToRotate.transform.rotation.eulerAngles.x;
          float currentZ = objectToRotate.transform.rotation.eulerAngles.z;
          Quaternion newRotation = Quaternion.Euler(currentX, newY, currentZ);

          // Apply new rotation to the object
          objectToRotate.rotation = newRotation;
     }

}

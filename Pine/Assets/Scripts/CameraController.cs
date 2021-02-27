using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
     [SerializeField] Vector3 mountainCenter = new Vector3(0, 0, 0);
     [SerializeField, Range(1,20)] float distanceFromPlayer = 5f;
     [SerializeField, Range(0, 20)] float cameraHeight = 3f;
     [SerializeField, Range(0, 60)] float tiltAngle = 20f;

     GameObject characterController = null;


     private void Awake()
     {
          // Find the character controller object.
          if (characterController == null)
               characterController = GameObject.FindWithTag("GameController");

          // If the character controller reference is still null, send debug message.
          if (characterController == null)
               Debug.Log("No character controller in scene. Must add to scene for camera to work properly.");
     }



     private void Update()
     {
          SetPositionOffset();
          SetRotation(); 
     }



     /// <summary>
     /// Offsets the position of the camera from its local position and rotation.
     /// </summary>
     void SetPositionOffset()
     {
          // Move the camera away from the its parent along the Y and Z axis
          Vector3 offset = new Vector3(0, cameraHeight, -distanceFromPlayer);
          this.transform.localPosition = offset;

          // Rotate the camera along its X axis.
          Quaternion targetTilt = Quaternion.Euler(tiltAngle, 0, 0);
          this.transform.localRotation = targetTilt;
     }



     /// <summary>
     /// Have the camera's parent's position follow another object's position.
     /// </summary>
     public void FollowTransform(Transform objectToFollow)
     {
          transform.parent.position = objectToFollow.position;
     }



     /// <summary>
     /// Sets the rotation of the child camera around its parent.
     /// This will have it looking at the pinecone while always facing the mountain.
     /// </summary>
     void SetRotation()
     {
          // If the character controller reference doesn't exist, don't do anything.
          if (characterController == null)
               return;

          // Find the direction from the character controller to the mountain.
          Vector3 targetDirection = (characterController.transform.position - mountainCenter).normalized;

          // Find the position to look at, but only rotating along the camera's parent's y axis.
          Vector3 targetPosition = new Vector3(targetDirection.x,
                                                  this.transform.parent.position.y,
                                                  targetDirection.z);

          // Apply the rotation to the camera's parent.
          transform.parent.LookAt(targetPosition);
     }
}

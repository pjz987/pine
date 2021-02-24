using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickingMechanics : MonoBehaviour
{
     // Camera
     GameObject playerCamera = null;

     // Mouse
     
     Vector2 mouseClickPosition = new Vector2(0, 0);
     Vector2 mouseCurrentPosition = new Vector2(0, 0);

     protected bool mouseButtonDown = false;
     bool showClickAbility = false;
     public float uiClickArea = 1f;
     public float uiMouseRadius = 1f;

     // Flicking
     public GameObject objectToFlick = null;
     public float flickStrengthScalar = 15f;  
     public float flickStrengthUpwards = 1f;

     Vector3 flickDirectionPhysics = new Vector3(0, 0, 0);
     bool allowPineconeFlick = false;



     private void Awake()
     {
          if (playerCamera == null)
          {
               playerCamera = GameObject.FindWithTag("MainCamera");

               // If camera still cannot be found, then log the problem.
               if (playerCamera == null)
                    Debug.Log("No camera found. Please add the 'MainCamera' tag to the camera object.");
          }
     }



     private void FixedUpdate()
     {
          // If there is no object to flick, then do nothing.
          if (objectToFlick == null)
               return;

          FollowTransform(objectToFlick.transform);
          playerCamera.GetComponent<CameraController>().FollowTransform(objectToFlick.transform);

          // PineconeFlick
          if (allowPineconeFlick == true)
          {
               // Using the inverse of the flick direction to simulate pulling backwards.
               PineconeFlickPhysics(-flickDirectionPhysics);
          }
     }



     /// <summary>
     /// If there is an objectToFlick attached, then follow that object's position.
     /// </summary>
     /// <param name="objectToFollow"></param>
     protected void FollowTransform(Transform objectToFollow)
     {
          if (objectToFlick != null)
          {
               transform.position = objectToFollow.position;
          }
     }



     /// <summary>
     /// Prints out error message for not having an attached object to flick.
     /// </summary>
     protected bool CheckForObjectToFlick()
     {
          if (objectToFlick == null)
          {
               Debug.Log("No objectToFlick attached. Cannot perform flick action.");
               return false;
          }

          return true;
     }


     #region Mouse Input

     /// <summary>
     /// Checks for all mouse input.
     /// </summary>
     protected void MouseInput()
     {
          // Get the mouse position in screen space
          mouseCurrentPosition = GetMouseConstraints();

          
          // Error handling: If player clicks and there's nothing attached, don't do anything.
          if (Input.GetButtonDown("MouseButton") && CheckForObjectToFlick() == false)
               return;
          

          // Check to see if the mouse is within grabbing range of the object to flick
          if (CheckMouseWithinFlickingArea(uiClickArea) == true && CheckForObjectToFlick() == true)
          {
               // If the player clicks, then find the click position
               if (Input.GetButtonDown("MouseButton"))     
               {
                    mouseButtonDown = true;
                    mouseClickPosition = GetMouseConstraints();
               }
          }

          // When the player lets go, reset the click position to 0
          if (Input.GetButtonUp("MouseButton"))
          {
               mouseButtonDown = false;
               mouseClickPosition = Vector3.zero;
          }
     }


     /// <summary>
     /// Gets the position of the mouse to the playable screen size of the game, either windowed or fullscreen.
     /// </summary>
     /// <returns></returns>
     Vector2 GetMouseConstraints()
     {
          Vector2 mouseConstrainedPosition = Input.mousePosition;
          float screenWidth = Screen.width * 0.5f;
          float screenHeight = Screen.height * 0.5f;

          // Set mouse origin position to the center of the playable area.
          mouseConstrainedPosition.x -= screenWidth;
          mouseConstrainedPosition.y -= screenHeight;

          // Clamp the mouse position to the playable screen size.
          mouseConstrainedPosition.x = Mathf.Clamp(mouseConstrainedPosition.x, -screenWidth, screenWidth);
          mouseConstrainedPosition.y = Mathf.Clamp(mouseConstrainedPosition.y, -screenHeight, screenHeight);

          return mouseConstrainedPosition;
     }


     /// <summary>
     /// Gets the normalized vector3 of the mouse position relative to the center of the camera root position. 
     /// </summary>
     /// <returns></returns>
     Vector3 GetMouseDragAngle()
     {
          // Get the camera root position.
          Transform cameraRootTransform = playerCamera.transform.root.transform;

          // Find the angle of the mouse position relative to the mouse's zero position (0, 0).
          // Vector2.Angle wasn't working with (0, 0) as a point. Using Atan2 is fine.
          float mouseAngle = Mathf.Atan2(mouseCurrentPosition.y, mouseCurrentPosition.x) * Mathf.Rad2Deg;
          mouseAngle += 90f;  // Need to offset to align upcoming vector with mouse screen position

          // Creates a vector based on an angle around the camera root's y-axis, and at the camera root's position. 
          // Inverted mouseAngle to align with mouse screen position.
          Vector3 mouseDragDirection = Quaternion.AngleAxis(-mouseAngle, cameraRootTransform.up) * cameraRootTransform.position;

          // Returning normalized vector to keep consistency.
          return mouseDragDirection.normalized;
     }


     
     /// <summary>
     /// 
     /// </summary>
     /// <returns></returns>
     float GetMouseDragDistance()
     {
          // Get percentage of mouse X to screen X. Repeat for Y.
          float xPercent = mouseCurrentPosition.x / (Screen.width * 0.5f);
          float yPercent = mouseCurrentPosition.y / (Screen.height * 0.5f);

          // Set the percentage values in a new vector
          Vector2 mouseDistance = new Vector2(xPercent, yPercent);

          // Return the magnitude of the new vector
          return mouseDistance.magnitude * flickStrengthScalar;
     }



     /// <summary>
     /// Determines whether or not the mouse is within the range of grabbing the tree/pinecone.
     /// </summary>
     /// <returns></returns>
     bool CheckMouseWithinFlickingArea(float mouseGrabRange)
     {
          // If there is no object attached, only return false.
          if (objectToFlick == null)
          {
               return false;
          }
          // Else if the mouse is within the flicking area, show flicking mouse grab range.
          else if (Mathf.Abs(PreparePineconeFlick().magnitude) <= mouseGrabRange)
          {
               showClickAbility = true;
               return true;
          }
          // If mouse is NOT within grabbing range, then return false.
          else
          {
               showClickAbility = false;
               return false;
          }
     }


     #endregion

     #region Pinecone Flicking


     /// <summary>
     /// Returns the direction and strength of a flick in a vector3. Strength is store in magnitude.
     /// </summary>
     /// <returns></returns>
     protected Vector3 PreparePineconeFlick()
     {
          Vector3 flickDirection = new Vector3(0, 0, 0);

          // Set the direction of the flick based on the angle on mouse click and orientation of camera.
          flickDirection = GetMouseDragAngle();

          // Set the strength of the flick in the vector's magnitude.
          flickDirection = flickDirection * GetMouseDragDistance();

          // Adjust the upward strength of the flick based on a percent of the flick's strength.
          flickDirection = AdjustUpwardFlickStrength(flickDirection);

          return flickDirection;
     }



     /// <summary>
     /// 
     /// </summary>
     Vector3 AdjustUpwardFlickStrength(Vector3 inputDirection)
     {
          // Get the upward strength based on the magnitude
          float upwardForce = inputDirection.magnitude * -flickStrengthUpwards;

          // Apply the upward force to the input vector.
          return inputDirection + new Vector3(0, upwardForce, 0);
     }



     /// <summary>
     /// Checks if there is all the required information needed to flick the pinecone, then allows the flick via FixedUpdate.
     /// </summary>
     /// <param name="flickDirection"></param>
     protected void PineconeFlick(Vector3 flickDirection)
     {
          // If there is no object to flick, abort.
          if (objectToFlick == null)
          {
               Debug.Log("PineconeFlick: No object attached. Please add in order to flick.");
               return;
          }

          // If the attached object does not have a rigidbody, abort.
          if (objectToFlick.GetComponent<Rigidbody>() == null)
          {
               Debug.Log("PineconeFlick: Attached object has no rigidbody. Please add in order to flick.");
               return;
          }

          // Allow the pinecone to be flicked (run via FixedUpdate)
          flickDirectionPhysics = flickDirection;
          allowPineconeFlick = true;
     }



     /// <summary>
     /// Flicks the pinecone in the direction indicated by the mouse. Called via FixedUpdate because interaction with physics. 
     /// </summary>
     /// <param name="flickDirection"></param>
     void PineconeFlickPhysics(Vector3 flickDirection)
     {
          // Get the object's rigidbody.
          Rigidbody _rb = objectToFlick.GetComponent<Rigidbody>();

          // Apply a force impulse to the object's rigidbody.
          _rb.AddForce(flickDirection, ForceMode.Impulse);

          // Zero out the flick position and remove ability to flick.
          flickDirectionPhysics = Vector3.zero;
          allowPineconeFlick = false;
     }

     #endregion

     #region Gizmos


     // Temporary UI for flicking
     private void OnDrawGizmos()
     {
          if (playerCamera != null)
          {
               Vector3 mouseDragDirection = GetMouseDragAngle();
               float mouseStrength = GetMouseDragDistance();

               // Mouse Direction
               Vector3 from = playerCamera.transform.parent.position;
               Vector3 to = (mouseDragDirection * mouseStrength) + from;

               Gizmos.color = Color.yellow;
               Gizmos.DrawLine(from, to);

               // Mouse Action - Ability to Click
               if (showClickAbility == true)
               {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(objectToFlick.transform.position, uiClickArea*0.3f);
               }

               // MouseAction - Click Being Held
               if (mouseButtonDown == true)
               {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(to, uiMouseRadius);

                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(from, -PreparePineconeFlick() + from);
               }
          }
     }

     #endregion

}

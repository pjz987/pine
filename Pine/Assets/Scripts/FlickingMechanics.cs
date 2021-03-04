using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FlickingMechanics : MonoBehaviour
{
     // VFX
     CharacterParticleEffects _vfx = null;

     // UI Menu Control
     UserInteractionControl _ui = null;

     // Camera
     GameObject playerCamera = null;
     private CameraController workingCameraController = null;

     // Mouse
     Vector2 mouseClickPosition = new Vector2(1, 1);
     Vector2 mouseCurrentPosition = new Vector2(0, 0);
     Vector3 flickDirection = new Vector3(0, 0, 0);
     Vector3 flickDirectionPhysics = new Vector3(0, 0, 0);

     protected bool mouseButtonDown = false;
     bool allowPineconeFlick = false;
     bool showClickAbility = false;
     float uiMouseRadius = 0.5f;

     // Flicking
     [Header("Flicking Objects")]
     public GameObject objectToFlick = null;
     public GameObject saplingObject = null;
     public GameObject treeObjectFinal = null;
     public GameObject[] treeSelection;
     protected GameObject treeObject = null;

     GameObject followingObject = null;
     GameObject currentSapling = null;
     GameObject treeHolder = null;
     GameObject foliageHolder = null;
     FoliageGrowth _foliageGrowth = null;

     [Header("Flick Attributes")]    
     [SerializeField] float objectFlickForward = 6f;  
     [SerializeField] float objectFlickUpward = 0.8f;
     [SerializeField] float treeFlickForward = 3.5f;
     [SerializeField] float treeFlickUpward = 2.5f;
     [SerializeField] float treeBendAmount = 6f;

     [SerializeField] int flicksUntilTreeGrowth = 2;
     int currentFlickCounter = 0;

     float timeElapsedSinceFlick = 2f;
     float flickTimerCountdown = 0f;

     [Header("Transition Timers")]
     [SerializeField] float delaySaplingLifeTimer = 1.3f;
     float delaySaplingLifeCurrent = 0f;
     [SerializeField] float delayTreeGrowthTimer = 0.6f;
     float delayTreeGrowthCurrent = 0f;

     // Raycasting and Tree Growing Thresholds
     float groundDistance = 0f;
     float pineconeVelocityAverage = 0f;

     [Header("Parameters for Tree Growth")]
     public float groundDistanceThreshold = 0.3f;
     public float pineconeVelocityThreshold = 0.3f;
     float tempTreeTop = 3.8f;
     public float bigTreeTop = 14f;
     protected bool insideEndGoal = false;



     #region --- Awake, FixedUpdate and Misc. ---

     private void Awake()
     {
          // Camera
          if (playerCamera == null)
          {
               playerCamera = GameObject.FindWithTag("MainCamera");
               workingCameraController = playerCamera.GetComponent<CameraController>();
               // If camera still cannot be found, then log the problem.
               if (playerCamera == null)
                    Debug.Log("No camera found. Please add the 'MainCamera' tag to the camera object.");
          }

          // Sapling Object
          CheckForSapling(true);

          // Tree Holder
          if (treeHolder == null)
               SearchForTreeHolder();

          // Foliage Holder
          if (foliageHolder == null)
               SearchForFoliageHolder();

          // Foliage Growth
          _foliageGrowth = GetComponent<FoliageGrowth>();
           CheckForFoliageGrowth(true);

          // VFX
          _vfx = GetComponent<CharacterParticleEffects>();
          CheckForVFX(true);

          // UI
          _ui = GameObject.Find("UIMenuControl").GetComponent<UserInteractionControl>();
          CheckForUI(true);

          // Set the flick counter.
          currentFlickCounter = flicksUntilTreeGrowth;

          // Set the transition timers.
          delaySaplingLifeCurrent = delaySaplingLifeTimer;
          delayTreeGrowthCurrent = delayTreeGrowthTimer;

          // Initially follow the pinecone.
          followingObject = objectToFlick;
     }



     private void FixedUpdate()
     {
        // If there is no object to flick, then do nothing.
        if (workingCameraController.IsInGame == true)
        {
            if (objectToFlick == null)
                return;
            
            FollowTransform(followingObject.transform);
            playerCamera.GetComponent<CameraController>().FollowTransform(objectToFlick.transform);

            // PineconeFlick
            if (allowPineconeFlick == true)
            {
                // Using the inverse of the flick direction to simulate pulling backwards.
                PineconeFlickPhysics(-flickDirectionPhysics);
            }
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
               transform.position = objectToFollow.localPosition;
          }
     }



     /// <summary>
     /// Prints out error message for not having an attached object to flick.
     /// </summary>
     bool CheckForObjectToFlick()
     {
          if (objectToFlick == null)
          {
               Debug.Log("No objectToFlick attached. Cannot perform flick action.");
               return false;
          }

          return true;
     }



     /// <summary>
     /// Prints out error message for not having an attached tree object for tree flicking.
     /// </summary>
     /// <param name="sendMessage"></param>
     /// <returns></returns>
     protected bool CheckForTree(bool sendMessage = false)
     {
          if (treeObject == null)
          {
               if (sendMessage == true)
                    Debug.Log("No tree object attached. Cannot perform tree flicking action.");

               return false;
          }

          return true;
     }



     /// <summary>
     /// Prints out error message for not having an attached sapling object for growth transitions.
     /// </summary>
     /// <param name="sendMessage"></param>
     /// <returns></returns>
     protected bool CheckForSapling(bool sendMessage = false)
     {
          if (saplingObject == null)
          {
               if (sendMessage == true)
                    Debug.Log("No sapling object attached.");

               return false;
          }

          return true;
     }



     /// <summary>
     /// Checks for and can print out an error message for not having an attached foliage growth script.
     /// </summary>
     /// <param name="sendMessage"></param>
     /// <returns></returns>
     bool CheckForFoliageGrowth(bool sendMessage = false)
     {
          if (_foliageGrowth == null)
          {
               if (sendMessage == true)
                    Debug.Log("No foliage growth script attached. Cannot spawn foliage around tree.");

               return false;
          }
          return true;
     }


     /// <summary>
     /// Checks for and can print out an error message for not having an attached character particle effects script.
     /// </summary>
     /// <param name="sendMessage"></param>
     /// <returns></returns>
     bool CheckForVFX(bool sendMessage = false)
     {
          if (_vfx == null)
          {
               if (sendMessage == true)
                    Debug.Log("No CharacterParticleEffects script attached. Cannot use vfx.");

               return false;
          }
          return true;
     }


     /// <summary>
     /// Checks for and can print out an error message for not finding the UI object in scene.
     /// </summary>
     /// <param name="sendMessage"></param>
     /// <returns></returns>
     bool CheckForUI(bool sendMessage = false)
     {
          if (_ui == null)
          {
               if (sendMessage == true)
                    Debug.Log("Could not find the UIMenuControl object. Please rename the object with the 'UserInteractionControl' script to UIMenuControl.");

               return false;
          }
          return true;
     }


     /// <summary>
     /// 
     /// </summary>
     void SearchForTreeHolder()
     {
          GameObject treeHolderRef;

          // Search in scene if a Tree Holder exists.
          // If false, create one and assign the reference.
          if (GameObject.FindGameObjectWithTag("TreeHolder") == null)
          {
               GameObject newTreeHolder = new GameObject();
               newTreeHolder.name = "TreeHolder";
               newTreeHolder.tag = "TreeHolder";
               treeHolderRef = newTreeHolder;
          }
          // If true, assign the reference.
          else
               treeHolderRef = GameObject.FindGameObjectWithTag("TreeHolder");

          // Assign the reference to the controller's Tree Holder.
          treeHolder = treeHolderRef;
     }



     /// <summary>
     /// 
     /// </summary>
     void SearchForFoliageHolder()
     {
          GameObject foliageHolderRef;

          // Search scene if a Foliage holder exists.
          // If false, create one and assign the reference.
          if (GameObject.FindGameObjectWithTag("FoliageHolder") == null)
          {
               GameObject newFoliageHolder = new GameObject();
               newFoliageHolder.name = "FoliageHolder";
               newFoliageHolder.tag = "FoliageHolder";
               foliageHolderRef = newFoliageHolder;
          }
          // If true, assign the reference.
          else
               foliageHolderRef = GameObject.FindGameObjectWithTag("FoliageHolder");

          // Assign the reference to the controller's Tree Holder.
          foliageHolder = foliageHolderRef;
     }


     /// <summary>
     /// Sets the boolean for the whether or not the pinecone is inside the end goal trigger. 
     /// Called via GoalPointCheck collision.
     /// </summary>
     /// <param name="status"></param>
     public void SetGoalStatus(bool status)
     {
          insideEndGoal = status;
     }

     #endregion


     #region --- Mouse Input --------------------

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
          if (/*CheckMouseWithinFlickingArea() == true && */ CheckForObjectToFlick() == true)
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
               mouseClickPosition = Vector3.one;
          }
     }



     /// <summary>
     /// Gets the position of the mouse to the playable screen size of the game, either windowed or fullscreen. 
     /// Does not set the center screen position regarding mouse input.
     /// </summary>
     /// <returns></returns>
     Vector2 GetMouseConstraints()
     {
          Vector2 mouseConstrainedPosition = Input.mousePosition;
          float screenWidth = Screen.width * 0.5f;
          float screenHeight = Screen.height * 0.5f;

          // Set mouse origin position to the center of the playable area.
          mouseConstrainedPosition.x -= screenWidth;
          mouseConstrainedPosition.y -= screenHeight; // * customScreenCenter;

          // Clamp the mouse position to the playable screen size.
          mouseConstrainedPosition.x = Mathf.Clamp(mouseConstrainedPosition.x, -screenWidth, screenWidth);
          mouseConstrainedPosition.y = Mathf.Clamp(mouseConstrainedPosition.y, -screenHeight, screenHeight);

          return mouseConstrainedPosition;
     }


     /// <summary>
     /// Gets the angle between the mouse's current and clicked position and return a normalized vector from the camera's root position. 
     /// </summary>
     /// <returns></returns>
     Vector3 GetMouseDragAngle()
     {
          // Find the angle of the mouse current position relative to the mouse's clicked position.
          float yDelta = mouseClickPosition.y - mouseCurrentPosition.y;
          float xDelta = mouseClickPosition.x - mouseCurrentPosition.x;
          float mouseAngle = -1f * Mathf.Atan2(yDelta, xDelta) * Mathf.Rad2Deg;

          mouseAngle += 90f;       // Need to offset and invert X to align vector to camera's direction.

          // Creates a vector based on an angle around the camera root's y-axis, and at the camera root's position. 
          Transform cameraRootTransform = playerCamera.transform.root.transform;
          Vector3 cameraPositionIgnoringHeight = new Vector3(cameraRootTransform.position.x, 0f, cameraRootTransform.position.z);

          Vector3 mouseDragDirection = Quaternion.AngleAxis(mouseAngle, Vector3.up) * cameraPositionIgnoringHeight;

          // Returning normalized vector to keep consistency.
          return mouseDragDirection.normalized;
     }


     
     /// <summary>
     /// Gets the amount the mouse is pulled back relative to screen size AND screen center. Returns the scaled magnitude.
     /// </summary>
     /// <returns></returns>
     float GetMouseDragDistance(float forceAmount)
     {    
          // Get percentage of mouse X to screen X. Repeat for Y.
          float xCurrentPercent = mouseCurrentPosition.x / (Screen.width * 0.5f);
          float yCurrentPercent = mouseCurrentPosition.y / (Screen.height * 0.5f); // * customScreenCenter);

          // Get percentage of click X to screen X. Repeat for Y.
          float xClickPercent = mouseClickPosition.x / (Screen.width * 0.5f);
          float yClickPercent = mouseClickPosition.y / (Screen.height * 0.5f); // * customScreenCenter);

          // Set the distance between percentage values in a new vector
          Vector2 mouseDistance = new Vector2(xCurrentPercent, yCurrentPercent) - new Vector2(xClickPercent, yClickPercent);                 

          // Return the magnitude of the new vector
          return mouseDistance.magnitude * forceAmount;
     }


     #endregion


     #region --- Pinecone Flicking --------------


     /// <summary>
     /// Returns the direction and strength of a flick in a vector3. Strength is store in magnitude.
     /// </summary>
     /// <returns></returns>
     protected Vector3 PreparePineconeFlick()
     {
          // If the mouse button is currently held down, update flick direction
          if (mouseButtonDown == true)
          {
               // Set the direction of the flick based on the angle on mouse click and orientation of camera.
               flickDirection = GetMouseDragAngle();

               // Set the strength of the flick in the vector's magnitude.
               flickDirection = flickDirection * GetMouseDragDistance(objectFlickForward);

               // Adjust the upward strength of the flick based on a percent of the flick's strength.
               flickDirection = AdjustUpwardFlickStrength(flickDirection, objectFlickUpward);
          }

          return flickDirection;
     }



     /// <summary>
     /// Applies an upward force to the pinecone based on flick strength.
     /// </summary>
     Vector3 AdjustUpwardFlickStrength(Vector3 inputDirection, float forceAmount)
     {
          // Get the upward strength based on the magnitude
          float upwardForce = inputDirection.magnitude * -forceAmount;

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

          // Once flicked, set the flick timer to determine next action.
          SetPineconeFlickTimer(timeElapsedSinceFlick);

          // And decrease the flick counter until tree growth.
          currentFlickCounter--;
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
          //_rb.AddForceAtPosition(flickDirection, _rb.transform.position + (_rb.transform.up*0.1f), ForceMode.Impulse);

          // Zero out the flick position and remove ability to flick.
          flickDirectionPhysics = Vector3.zero;
          allowPineconeFlick = false;
     }



     /// <summary>
     /// Returns true if enough time has passed since the pinecone was flicked.
     /// </summary>
     /// <returns></returns>
     bool PineconeFlickTimer()
     {
          // Count down the timer.
          flickTimerCountdown -= Time.deltaTime;

          if (flickTimerCountdown <= 0f)
               return true;
          else
               return false;
     }



     /// <summary>
     /// Set the pinecone flick timer.
     /// </summary>
     /// <param name="timerAmount"></param>
     void SetPineconeFlickTimer(float timerAmount)
     {
          flickTimerCountdown = timerAmount;
     }



     /// <summary>
     /// Determines the conditions for planting the pinecone and growing the tree model.
     /// </summary>
     /// <returns></returns>
     protected bool PineconePlantingRequirements()
     {
          Rigidbody _rb = objectToFlick.GetComponent<Rigidbody>();
          RaycastHit hit;

          // Velocity average
          float CalculateAverageOfVector3(Vector3 input)
          {
               float total = input.x + input.y + input.z;
               total = Mathf.Abs(total / 3);
               return total;
          }

          // Raycast to the ground and calculate thresholds.
          if (Physics.Raycast(_rb.transform.position, -Vector3.up, out hit))
          {
               groundDistance = hit.distance;
               pineconeVelocityAverage = CalculateAverageOfVector3(_rb.velocity);

               // If enough time has passed since pinecone was flicked,
               if (PineconeFlickTimer() == true)
               {
                    // AND if pinecone is within ground threshold AND velocity threshold, return true.
                    if (groundDistance <= groundDistanceThreshold && pineconeVelocityAverage <= pineconeVelocityThreshold)
                         return true;
               }
          }

          // If no conditions met, return false.
          return false;
     }



     /// <summary>
     /// Determines whether pinecone can keep flicking before another flick or tree growth.
     /// </summary>
     /// <returns></returns>
     protected bool AllowPineconeFlick()
     {
          // If the flick counter has reached 0 (or somehow surpassed it), don't allow more flicks.
          if (currentFlickCounter <= 0)
               return false;

          // Else it's okay to keep flicking.
          return true;
     }

     #endregion


     #region --- Tree Flicking ------------------


     /// <summary>
     /// Creates growth particles, hides the pinecone and creates a sapling object.
     /// </summary>
     protected void GrowSapling()
     {
          // If there is no sapling object to use, abort.
          if (CheckForSapling() == false)
               return;

          // Play the sapling growth vfx
          if (CheckForVFX() == true)
               _vfx.PlayVFX(_vfx.vfxPineconeSaplingGrowth);

          // Find the pinecone's render child and hide it
          GameObject pineconeRender = objectToFlick.transform.Find("render").gameObject;
          pineconeRender.SetActive(false);

          // Stop pinecone's movement
          Rigidbody _rb = objectToFlick.GetComponent<Rigidbody>();
          _rb.freezeRotation = true;

          // Find the ground to spawn the sapling on
          RaycastHit hit;
          Vector3 additionalHeight = new Vector3(0f, 0.2f, 0f);
          Physics.Raycast(this.transform.position + additionalHeight, Vector3.down, out hit);
          Vector3 groundPosition = hit.point;

          // Create the sapling object
          currentSapling = Instantiate(saplingObject, groundPosition, saplingObject.transform.rotation);
     }


     /// <summary>
     /// Counts down the timer used for the sapling state's screen time.
     /// </summary>
     /// <returns></returns>
     protected bool DelaySaplingLifeTimer()
     {
          // If the timer has not reached 0, then continue to count down.
          if (delaySaplingLifeCurrent > 0)
          {
               delaySaplingLifeCurrent -= Time.deltaTime;
               return false;
          }

          // Once the timer has reached 0, reset the timer for future use and return true.
          return true;
     }


     /// <summary>
     /// Counts down the timer used for the Tree Growth state.
     /// </summary>
     /// <returns></returns>
     protected bool DelayTreeGrowthTimer()
     {
          // If the timer has not reached 0, then continue to count down.
          if (delayTreeGrowthCurrent > 0)
          {
               delayTreeGrowthCurrent -= Time.deltaTime;
               return false;
          }

          // Once the timer has reached 0, reset the timer for future use and return true.
          return true;
     }


     /// <summary>
     /// Resets both transition timers used for the sapling and tree growth states.
     /// </summary>
     protected void ResetTimers()
     {
          // Reset both transition timers.
          delaySaplingLifeCurrent = delaySaplingLifeTimer;
          delayTreeGrowthCurrent = delayTreeGrowthTimer;
     }


     /// <summary>
     /// Calls the UI Menu Controls script 'DisplayTransition'.
     /// </summary>
     protected void BeginScreenTransition()
     {
          // UI Screen Transition
          _ui.DisplayTransition();
     }


     /// <summary>
     /// Calls the UI Menu Controls script 'Display Finished Menu'.
     /// </summary>
     protected void DisplayEndScreen()
     {
          _ui.DisplayFinishedMenu();
     }

     /// <summary>
     /// Selects a random tree to use for tree growth. 
     /// </summary>
     /// <returns></returns>
     protected bool SetRandomTree()
     {
          if (treeSelection.Length == 0)
               return CheckForTree(true);

          // Select a random tree
          int randomTree = Random.Range(0, treeSelection.Length);
          treeObject = treeSelection[randomTree];

          // Return the check for the tree. Can be null if no possible trees.
          return CheckForTree(true);
     }



     /// <summary>
     /// Destroys the sapling, creates a tree, moves the pinecone to the top of the tree, reveals the pinecone and grows foliage.
     /// </summary>
     protected void GrowTreeAfterSapling()
     {
          // If inside the end goal, swap references.
          if (insideEndGoal == true)
               treeObject = treeObjectFinal;

          // Create the tree
          GameObject newTree = Instantiate(treeObject, objectToFlick.transform.position, treeObject.transform.rotation);
          newTree.transform.parent = GameObject.FindGameObjectWithTag("TreeHolder").transform;
          newTree.name = "tree_" + newTree.transform.parent.childCount;

          // Set the new tree as the treeObject and have the camera follow it
          treeObject = newTree;
          followingObject = treeObject;

          // Destroy the sapling
          Destroy(currentSapling);

          // Cancel most of objectToFlick's movement
          Rigidbody _rb = objectToFlick.GetComponent<Rigidbody>();
          _rb.freezeRotation = false;   // changing back from Sapling state
          _rb.velocity = Vector3.zero;
          _rb.angularVelocity = Vector3.zero;
          _rb.useGravity = false;

          // Find the pinecone's render child and show it again
          GameObject pineconeRender = objectToFlick.transform.Find("render").gameObject;
          pineconeRender.SetActive(true);

          // Move the objectToFlick to the top of the tree
          HoldObjectToFlickInTree();

          // Generate the foliage around the tree.
          if (CheckForFoliageGrowth() == true)
               _foliageGrowth.GenerateFoliage(foliageHolder.transform);
     }


     /// <summary>
     /// Instantiates a tree at the pinecone's ground position and moves the pinecone to the top of the tree.
     /// </summary>
     /// <param name="treeToGrow"></param>
     protected void GrowTreeSimple(GameObject treeToGrow)
     {
          // Create the tree
          GameObject newTree = Instantiate(treeToGrow, objectToFlick.transform.position, Quaternion.identity);
          newTree.transform.parent = GameObject.FindGameObjectWithTag("TreeHolder").transform;
          newTree.name = "tree_" + newTree.transform.parent.childCount;

          // Set the new tree as the treeObject
          treeObject = newTree;

          // Cancel most of objectToFlick's movement
          Rigidbody _rb = objectToFlick.GetComponent<Rigidbody>();
          _rb.velocity = Vector3.zero;
          _rb.angularVelocity = Vector3.zero;
          _rb.useGravity = false;

          // Move the objectToFlick to the top of the tree
          HoldObjectToFlickInTree();

          // Generate the foliage around the tree.
          if (CheckForFoliageGrowth() == true)
               _foliageGrowth.GenerateFoliage(foliageHolder.transform);
     }



     /// <summary>
     /// Keeps the objectToFlick at a postition in a tree.
     /// </summary>
     protected void HoldObjectToFlickInTree(bool useBigTreeTop = false)
     {
          // Determine the height to hold the pinecone at depending on if the pinecone is in the end goal or not via the tree grown.
          float treeTop;
          if (useBigTreeTop == true)
               treeTop = bigTreeTop;
          else
               treeTop = tempTreeTop;

          // Get the holding position
          Vector3 holdPosition = treeObject.transform.position + treeObject.transform.rotation * (Vector3.up * treeTop);

          // Set the objectToFlick's position to the holding position.
          objectToFlick.transform.position = holdPosition;
          
          // Rotate the objectToFlick so it's up direction is the same as the tree's.
          objectToFlick.transform.up = treeObject.transform.up;
     }



     /// <summary>
     /// Returns the direction and strength of a flick in a vector3. Strength is store in magnitude.
     /// Also rotates the treeObject in the direction being pulled (flickDirection);
     /// </summary>
     /// <returns></returns>
     protected Vector3 PrepareTreeFlickSimple()
     {
          // If the mouse button is currently held down, update flick direction.
          if (mouseButtonDown == true)
          {
               // Set the direction of the flick based on the angle on mouse click and orientation of camera.
               flickDirection = GetMouseDragAngle();

               // Set the strength of the flick in the vector's magnitude.
               flickDirection = flickDirection * GetMouseDragDistance(treeFlickForward);

               // Rotating to flick direction - rotate tree along y-axis
               Vector3 treeBend = (flickDirection * Mathf.Abs(treeBendAmount)) - new Vector3(0, -90f, 0);
               treeObject.transform.localRotation = Quaternion.Euler(treeBend);

               // Flicking back - rotate tree along x-axis
               //Quaternion flickRotation = Quaternion.Euler(flickDirection.magnitude,0,0);
               //treeObject.transform.localRotation = flickRotation;

               // Adjust the upward strength of the flick based on a percent of the flick's strength.
               flickDirection = AdjustUpwardFlickStrength(flickDirection, treeFlickUpward);
          }

          return flickDirection;
     }



     /// <summary>
     /// 
     /// </summary>
     /// <param name="flickDirection"></param>
     protected void TreeFlick(Vector3 flickDirection)
     {
          // Have camera follow pinecone again
          followingObject = objectToFlick;

          // Center the tree's rotation
          treeObject.transform.rotation = Quaternion.Euler(0, 0, 0);

          // Allow the pinecone to be flicked (run via FixedUpdate)
          flickDirectionPhysics = flickDirection;
          allowPineconeFlick = true;

          // Reactive pinecone's gravity
          objectToFlick.GetComponent<Rigidbody>().useGravity = true;

          // Once flicked, set the flick timer to determine next action.
          SetPineconeFlickTimer(timeElapsedSinceFlick);

          // And reset flick counter to allow for pinecone flicking.
          currentFlickCounter = flicksUntilTreeGrowth;
     }


     #endregion


     #region --- Gizmos -------------------------


     // Temporary UI for flicking
     private void OnDrawGizmos()
     {
          if (playerCamera != null)
          {
               Vector3 mouseDragDirection = GetMouseDragAngle();
               float mouseStrength = GetMouseDragDistance(objectFlickForward);

               // Mouse Direction
               Vector3 from = playerCamera.transform.parent.position;
               Vector3 to = (mouseDragDirection * mouseStrength) + from;

               Gizmos.color = Color.yellow;
               Gizmos.DrawLine(from, to);

               // Mouse Action - Ability to Click
               if (showClickAbility == true & mouseClickPosition != Vector2.one)
               {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(objectToFlick.transform.position, uiMouseRadius*0.3f);  
               }

               
               // MouseAction - Click Being Held
               if (mouseButtonDown == true)
               {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(to, uiMouseRadius);

                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(from, -flickDirection + from);
               }
               
          }
     }

     #endregion

}

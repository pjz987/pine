using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateMachine : FlickingMechanics
{

     // Character Controller player states.
     public enum PlayerState
     {
          UI,
          Pinecone_StandingBy,
          Pinecone_Flicking,
          Pinecone_Movement,
          Pinecone_GrowTransition,
          Tree_StandingBy,
          Tree_Flicking
     }
     public PlayerState playerState = PlayerState.UI;

     // Indicates whether or not the player is in the UI state.
     bool uiPlayerState = true;

     // Pete audio start
     
     AudioManager audioManager;

     // RNG for Random Sounds
     Random random = null;
     string[] treeGroans = new string [] {"TreeGroan1", "TreeGroan2", "TreeGroan3", "TreeGroan4"};
     Vector3 lastFlickDirection =  Vector3.zero;
     // Pete audio end




     void Start ()
     {
          random = new Random();
          audioManager = FindObjectOfType<AudioManager>();
     }


     /// <summary>
     /// Meant to be called outside of the State Machine script.
     /// Transitions player input between UI and Flicking Mechanics.
     /// </summary>
     /// <param name="setState"></param>
     public void SetUiState(bool setState)
     {
          // Exiting the UI State
          if (setState == false)
          {
               uiPlayerState = false;
          }

          // Entering the UI State
          if (setState == true)
          {
               uiPlayerState = true;
               playerState = PlayerState.UI;
          }
     }



     // The State Machine for the player.
     private void Update()
     {
          // Mouse Input is always running.
          MouseInput();


          #region UI State

          if (playerState == PlayerState.UI)
          {
               // Once declared false, then exit the UI state into the Pinecone StandingBy state.
               if (uiPlayerState == false)
                    playerState = PlayerState.Pinecone_StandingBy;
          }


          #endregion

          #region PineCone States --------------------

          // Pinecone is NOT moving and controller is waiting for player input.
          if (playerState == PlayerState.Pinecone_StandingBy)
          {
               // If the player clicks, begin flicking.
               if (mouseButtonDown == true)
               {
                    playerState = PlayerState.Pinecone_Flicking;
               }
          }

          // Pinecone is NOT moving and controller is GETTING player input.
          if (playerState == PlayerState.Pinecone_Flicking)
          {
               Vector3 flickDirection = PreparePineconeFlick();

               // If the player lets go of the flick, apply force and watch pinecone movement.
               if (mouseButtonDown == false)
               {
                    PineconeFlick(flickDirection);
                    playerState = PlayerState.Pinecone_Movement;

                    // Pete audio
                    audioManager.Play("GroundFlicking");
               }
          }

          // Pinecone IS moving and controller CANNOT get player input.
          if (playerState == PlayerState.Pinecone_Movement)
          {
               // If the flick counter is not finished, then allow for more pinecone flicking.
               if (AllowPineconeFlick() == true)
               {
                    playerState = PlayerState.Pinecone_StandingBy;
               }


               // Else, look for ground below AND low velocity threshold. Once both met, grow tree.
               else if (PineconePlantingRequirements() == true)
               {
                    playerState = PlayerState.Pinecone_GrowTransition;
                    
                    // Pete audio
                    audioManager.Play("TreeGrow");
               }
          }

          // Pinecone is locked in place, grows a tree and transitions to Tree_StandBy state.
          if (playerState == PlayerState.Pinecone_GrowTransition)
          {
               // If no tree is attached in controller, switch to pinecone flicking.
               if (CheckForTree() == false)
               {
                    playerState = PlayerState.Pinecone_StandingBy;
                    return;
               }

               GrowTreeSimple(treeObject);
               playerState = PlayerState.Tree_StandingBy;
          }
          #endregion

          #region Tree States --------------------

          // Tree is planted and controller is waiting for player input.
          if (playerState == PlayerState.Tree_StandingBy)
          {
               // Hold the pinecone in position
               HoldObjectToFlickInTree();

               if (mouseButtonDown == true)
               {
                    playerState = PlayerState.Tree_Flicking;
                    // Pete audio
                    lastFlickDirection = Vector3.zero;
               }
          }

          // Controller is GETTING player input, bending the tree and waiting for input release.
          if (playerState == PlayerState.Tree_Flicking)
          {
               // Continue to hold the pinecone in position
               HoldObjectToFlickInTree();

               // Find the direction to flick while also rotation the tree.
               Vector3 flickDirection = PrepareTreeFlickSimple();

               // Pete audio start
               if (Mathf.Floor(flickDirection.magnitude) != Mathf.Floor(lastFlickDirection.magnitude))
               {
                    audioManager.PlayRandom(audioManager.treeGroansShort);

               }
               lastFlickDirection = flickDirection;
               // Pete audio end



               // If the player lets go of the flick, apply force and watch pinecone movement.
               if (mouseButtonDown == false)
               {
                    TreeFlick(flickDirection);
                    playerState = PlayerState.Pinecone_Movement;
                    // Pete audio
                    audioManager.PlayWaitPlay("TreeWhoosh1", 0.3f, "Whee");
               }

          }

          #endregion
     }
}

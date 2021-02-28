using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateMachine : FlickingMechanics
{


     public enum PlayerState
     {
          Pinecone_StandingBy,
          Pinecone_Flicking,
          Pinecone_Movement,
          Pinecone_GrowTransition,
          Tree_StandingBy,
          Tree_Flicking
     }
     public PlayerState playerState;
     AudioManager audioManager;
     // RNG for Random Sounds
     Random random = null; // new System.Random();
     string[] treeGroans = new string [] {"TreeGroan1", "TreeGroan2", "TreeGroan3", "TreeGroan4"};
     void Start ()
     {
          random = new Random();
          audioManager = FindObjectOfType<AudioManager>();
     }

     // The State Machine for the player.
     private void Update()
     {
          // Mouse Input is always running.
          MouseInput();


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
               }
          }

          // Pinecone is locked in place, grows a tree and transitions to Tree_StandBy state.
          if (playerState == PlayerState.Pinecone_GrowTransition)
          {
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
                    audioManager.Play(treeGroans[Random.Range(0, treeGroans.Length)]);
               }
          }

          // Controller is GETTING player input, bending the tree and waiting for input release.
          if (playerState == PlayerState.Tree_Flicking)
          {
               // Continue to hold the pinecone in position
               HoldObjectToFlickInTree();

               // Find the direction to flick while also rotation the tree.
               Vector3 flickDirection = PrepareTreeFlickSimple();

               // If the player lets go of the flick, apply force and watch pinecone movement.
               if (mouseButtonDown == false)
               {
                    TreeFlick(flickDirection);
                    playerState = PlayerState.Pinecone_Movement;
               }
          }

          #endregion
     }
}

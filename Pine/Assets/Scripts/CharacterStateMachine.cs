using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateMachine : FlickingMechanics
{


     enum PlayerState
     {
          Pinecone_StandingBy,
          Pinecone_Flicking,
          Pinecone_Movement,
          Tree_Flicking
     }
     PlayerState playerState;



     // The State Machine for the player.
     private void Update()
     {
          // Mouse Input is always running.
          MouseInput();


          #region PineCone States

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
               }
          }

          // Pinecone IS moving and controller CANNOT get player input.
          if (playerState == PlayerState.Pinecone_Movement)
          {

               //TESTING -- will come back to this state later.
               playerState = PlayerState.Pinecone_StandingBy;

          }
          #endregion

          #region Tree States

          if (playerState == PlayerState.Tree_Flicking)
          {

          }

          #endregion
     }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalPointCheck : MonoBehaviour
{


    CharacterStateMachine _stateMachine;
    [SerializeField] 
    private CameraController workingCameraController;

    private void Awake()
    {
        // The state machine will always be in the scene, so just find it.
        _stateMachine = GameObject.FindGameObjectWithTag("GameController").GetComponent<CharacterStateMachine>();
    }


    private void OnTriggerEnter(Collider other)
    {
        // If the other colliding object is the pinecone, then call the state machine's ending script.
        if (other.tag == "Pinecone")
        {
            _stateMachine.SetGoalStatus(true);
            workingCameraController.IsInGame = false;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        // If the other colliding object is the pinecone, then call the state machine's ending script.
        if (other.tag == "Pinecone")
        {
            _stateMachine.SetGoalStatus(false);
        }
    }
}

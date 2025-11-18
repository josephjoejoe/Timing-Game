using UnityEngine;
using System;
using System.Collections.Generic;

public class StateMachine : MonoBehaviour
{
    States currnetState = States.Idle;
    //public BaseState currentState1 = new IdleState();

    void Start()
    {

    }

    void Update() // this will be called once per frame
    {
        //currnetState1.Update();
        //StateUpdate();
        //currentState = States.Grapping;
        //currentState = States.Idle;
    }

    void OnTriggerEnter(Collider other)
    {
       
    }
}

public enum States
{
    Idle,
    Grapping,

}


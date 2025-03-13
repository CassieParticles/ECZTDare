using AK.Wwise;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//State machine used by guards, so has enum for each state
public enum GuardStates
{
    StateChangedExternally, //Used when state is changed externally (calling alarm)
    Idle,
    Patrol,
    HearNoise,  //Dev one, immediately transition to whichever behaviour should be performed
    Observe,
    Investigate,
    Chase,
    RaiseAlarm,
    Bumped
};
public abstract class BaseState
{
    public BaseState(GameObject guard)
    {
        guardAttached = guard;
        guardBehaviour = guardAttached.GetComponent<GuardBehaviour>(); 
    }
    public abstract void Start();
    public abstract void Stop();
    public abstract GuardStates RunTick();

    protected GameObject guardAttached;
    protected GuardBehaviour guardBehaviour;
}

public class StateMachine
{
    Dictionary<GuardStates, BaseState> states = new Dictionary<GuardStates, BaseState>();

    GuardStates currentState;

    public StateMachine()
    {
        
    }

    public void AddState(GuardStates state, BaseState StateObj)
    {
        if(states.ContainsKey(state)){ return; }
        states.Add(state, StateObj);
    }

    public void Start(GuardStates intialState)
    {
        currentState = intialState;
        states[currentState].Start();
    }
    public void BehaviourTick()
    {
        GuardStates newState = states[currentState].RunTick();
        if(newState==GuardStates.StateChangedExternally)    //Do not update if this is returned
        {
            return;
        }
        if(newState!=currentState)
        {
            states[currentState].Stop();
            currentState = newState;
            states[currentState].Start();
        }
    }

    public void MoveToState(GuardStates state)
    {
        states[currentState].Stop();
        currentState = state;
        states[currentState].Start();
    }

    public GuardStates getCurrentState()
    {
        return currentState;
    }
}
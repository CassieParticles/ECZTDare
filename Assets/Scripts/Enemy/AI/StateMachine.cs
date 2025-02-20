using AK.Wwise;
using System.Collections.Generic;
using Unity.VisualScripting;

//State machine used by guards, so has enum for each state
public enum GuardStates
{
    Patrol,
    Observe,
    Chase,
    RaiseAlarm
};
public abstract class BaseState
{
    public abstract void Start();
    public abstract void Stop();
    public abstract GuardStates RunTick();
}

public class StateMachine
{
    Dictionary<GuardStates, BaseState> states = new Dictionary<GuardStates, BaseState>();

    GuardStates currentState;

    public void AddState(GuardStates state, BaseState StateObj)
    {
        if(states.ContainsKey(state)){ return; }
        states.Add(state, StateObj);
    }
    public void BehaviourTick()
    {
        GuardStates newState = states[currentState].RunTick();
        if(newState!=currentState)
        {
            states[currentState].Stop();
            currentState = newState;
            states[currentState].Start();
        }
    }
}
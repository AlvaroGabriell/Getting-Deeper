using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonstroBase : MonoBehaviour
{
    public enum State { Idle, Chasing, Returning }
    State currentState;
    bool tocandoPlayer;

    public State getCurrentState()
    {
        return currentState;
    }
    public void setCurrentState(State newState)
    {
        currentState = newState;
    }

    public bool isTocandoPlayer()
    {
        return tocandoPlayer;
    }
    public void setTocandoPlayer(bool value)
    {
        tocandoPlayer = value;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPlayer : MonoBehaviour
{
    MonstroBase monstro;

    void Awake()
    {
        monstro = GetComponentInParent<MonstroBase>();
        if(monstro == null)
        {
            Debug.LogError("MonstroBase component not found in parent!");
        }
    }

    public void Attack()
    {
        if (monstro.getCurrentState() == MonstroBase.State.Chasing && monstro.isTocandoPlayer())
        {
            FindObjectOfType<UIHandler>().chamarGameOver();
        }
    }
}

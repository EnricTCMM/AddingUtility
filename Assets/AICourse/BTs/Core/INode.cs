using System;
using UnityEngine;


// all code by ESN

namespace BTs
{
    // the methods and stuff that all nodes in a BT must have
    // even BTs must implement this interface for they can be nodes in other BTs
    public interface INode
    {
        Status GetStatus();
        Status Tick();
        void Abort();
        void Clear();
        string Name { get; }
        void Contextualize(GameObject g);
    }


    /*
     Can an FSM be a NODE entity?

    GetStatus => must be added. Here the TERMINATED state may come handy BUT we shold also consider
                 the addition of a FAILED pit state.
    Tick => just call Update and verify the currentState
    Abort => just call OnExit
    Clear => just call OnEnter
    Name => fsms already have a name
     
     */


}

using UnityEngine;

// Code by ESN

// States and FSMs (Finite State Machines) are meant to implement this interface

namespace FSMs
{
    public interface IState
    {
        public string Name { get; set; }
        public void OnEnter();
        public void InState();
        public void OnExit();
        public void Construct(GameObject gm);
    }
}


// Code by ESN

namespace FSMs
{
    // The objects of this class represent FSM transitions
    // BEWARE: the source and destination states are not part of a transition
    // transitions only include the triggering condition and the OnTrigger logic.

    public class Transition
    {
        private static int id = 0; // used to give a number to anonymous transitions

        public string Name { get; private set; }

        // the check function is a parameterless bool function
        public delegate bool TransitionCheckDelegate();
        // the OnTrigger action is just a parameterless void action
        public delegate void TransitionOnTriggerAction();

        private TransitionCheckDelegate triggeringCondition;
        private TransitionOnTriggerAction onTrigger;

        // Constructor (1)
        public Transition(TransitionCheckDelegate triggeringCondition,
                           TransitionOnTriggerAction onTrigger = null )
        {
            id++;
            Name = "transition_" + id;
            this.triggeringCondition = triggeringCondition;
            this.onTrigger = onTrigger;
        }

        // Constructor (2)
        public Transition(string name, 
                          TransitionCheckDelegate transitionCheck,
                          TransitionOnTriggerAction onTrigger = null)
        {
            this.Name = name;
            this.triggeringCondition = transitionCheck;
            this.onTrigger = onTrigger;
        }

        public bool Check() { return triggeringCondition(); }
        public void OnTrigger() { onTrigger?.Invoke(); }


    }

}

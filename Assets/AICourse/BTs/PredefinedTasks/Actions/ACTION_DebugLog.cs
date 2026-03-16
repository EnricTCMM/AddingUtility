
using UnityEngine;

namespace BTs
{
    public class ACTION_DebugLog : Action
    {
        public enum Type { NORMAL, WARNING, ERROR };

        public string verbatimMessage;
        public Type type = Type.NORMAL;

        public ACTION_DebugLog() { }

        public ACTION_DebugLog(string verbatimMessage, Type type = Type.NORMAL)
        {
            this.verbatimMessage = verbatimMessage;
            this.type = type;
        }

        public override Status OnTick()
        {
            switch (type)
            {
                case Type.ERROR:
                    Debug.LogError(blackboard.Get<string>(verbatimMessage));
                    break;
                case Type.WARNING:
                    Debug.LogWarning(blackboard.Get<string>(verbatimMessage));
                    break;
                default:
                    Debug.Log(blackboard.Get<string>(verbatimMessage));
                    break;
            }
            return Status.SUCCEEDED;
        }
    }
}

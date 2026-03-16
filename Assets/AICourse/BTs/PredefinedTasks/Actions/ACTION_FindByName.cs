using UnityEngine;

namespace BTs
{
    public class ACTION_FindByName : Action
    {
        public string keyName;
        public string keyoutObjectFound;

        public ACTION_FindByName(string keyName, string keyoutObjectFound)
        {
            this.keyName = keyName;
            this.keyoutObjectFound = keyoutObjectFound;
        }


        public override Status OnTick()
        {
            GameObject found = GameObject.Find(blackboard.Get<string>(keyName));
            if (found != null)
            {
                blackboard.Put(keyoutObjectFound, found);
                return Status.SUCCEEDED;
            }
            else
                return Status.FAILED;

        }
    }
}
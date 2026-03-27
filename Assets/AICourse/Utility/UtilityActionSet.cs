using System.Collections.Generic;
using UnityEngine;
using BTs;

namespace Utility
{
    //[CreateAssetMenu(fileName = "UtilityActionSet", menuName = "Scriptable Objects/UtilityActionSet")]
    public abstract class UtilityActionSet : ScriptableObject
    {
        public List<ActionConsiderationPair> list = new List<ActionConsiderationPair>();

        public void Contextualize(GameObject go)
        {
            foreach (ActionConsiderationPair pair in list)
            {
                pair.consideration.Contextualize(go);
            }
        }

        private ActionConsiderationPair GetPair(Action action)
        {
            // in list find the pair that matches the action
            return list.Find(pair => pair.action == action);
        }
        
        private void Bind(Action action, IConsideration consideration)
        {
            // sanity checks: no nulls allowed
            if (action == null) 
                throw new System.ArgumentNullException("action", "Action cannot be null in Bind");
            if (consideration == null) 
                throw new System.ArgumentNullException("consideration", "Consideration cannot be null in Bind");
            
            // only one consideration per action is allowed so...
            if (GetPair(action).action ==null)
            {
                // if action in the returned pair is null it means that no pair containing this action
                // exists. So we can safely add a new pair.
                list.Add(new ActionConsiderationPair(action, consideration));
            }
            else throw new System.ArgumentException("Action "+action.Name+" already bound to a consideration");
        }
        
        public virtual void OnConstruction()
        {
        }
    }
    
    public struct ActionConsiderationPair {
        public Action action;
        public IConsideration consideration; // quite often this will be a Scorer
        
        public ActionConsiderationPair(Action action, IConsideration consideration)
        {
            this.action = action;
            this.consideration = consideration;
        }
    }
}



using System.Collections.Generic;
using UnityEngine;
using BTs;
using UnityEditor.Tilemaps;

namespace Utility
{
    //[CreateAssetMenu(fileName = "UtilityActionSet", menuName = "Scriptable Objects/UtilityActionSet")]
    public abstract class UtilityActionSet : ScriptableObject
    {
        public List<ActionConsiderationPair> list = new List<ActionConsiderationPair>();
        private List<ActionScorePair> scores = new List<ActionScorePair>();

        public void Contextualize(GameObject go)
        {
            foreach (ActionConsiderationPair pair in list)
            {
                pair.action.Contextualize(go);
                pair.consideration.Contextualize(go);
            }
        }

        private ActionConsiderationPair GetPair(Action action)
        {
            // in list find the pair that matches the actionName
            return list.Find(pair => pair.action == action);
        }
        
        public void Bind(Action action, IConsideration consideration)
        {
            // sanity checks: no nulls allowed
            if (action == null) 
                throw new System.ArgumentNullException("action", "Action cannot be null in Bind");
            if (consideration == null) 
                throw new System.ArgumentNullException("consideration", "Consideration cannot be null in Bind");
            
            // only one consideration per actionName is allowed so...
            if (GetPair(action).action ==null)
            {
                // if actionName in the returned pair is null it means that no pair containing this actionName
                // exists. So we can safely add a new pair.
                // notice that default inertia is used.
                list.Add(new ActionConsiderationPair(action, consideration, 0.1f));
            }
            else throw new System.ArgumentException("Action "+action.Name+" already bound to a consideration");
        }

        public void SetInertia(Action action, float inertia)
        {
            int index = list.FindIndex((p)=> p.action == action);
            if (index<0)
                throw new System.ArgumentException($"Cannot set inertia. Action {action.Name} is not bound yet.");
            
            // cannot work with a reference to the pair because it's a struct
            // list[index].inertia =... does not change the value of the pair stored. 
            // for list[index] returns a copy...
            ActionConsiderationPair pair = list[index];
            pair.inertia = inertia;
            list[index] = pair;
        }

        public List<ActionScorePair> ScoreAllActions()
        {
            scores.Clear();
            foreach (ActionConsiderationPair pair in list)
            {
                scores.Add(new ActionScorePair(pair.action, pair.consideration.GetScore(), pair.consideration, pair.inertia) );
            }
            // sort in descending order
            scores.Sort((a,b) => b.score.CompareTo(a.score));
            return scores;
        }
        
        public virtual void OnConstruction()
        {
            // this is the only method that subclasses must implement
        }
    }
    
    public struct ActionConsiderationPair {
        // even if it's named "Pair" it is a triad...
        public Action action;
        public IConsideration consideration; // quite often this will be a Scorer
        public float inertia;
        
        public ActionConsiderationPair(Action action, IConsideration consideration, float inertia = 0.1f)
        {
            this.action = action;
            this.consideration = consideration;
            this.inertia = inertia;
        }
    }
    
    // keeps last score of an action for later retrieval
    public struct ActionScorePair {
        // even if it's named "Pair" it is a quartet...
        public Action action;
        public float score;
        public IConsideration consideration; // <--- the scorer responsible for this score
        public float inertia;
        
        public ActionScorePair(Action action, float score, IConsideration consideration, float inertia)
        {
            this.action = action;
            this.score = score;
            this.consideration = consideration;
            this.inertia = inertia;
        }
    } 
}



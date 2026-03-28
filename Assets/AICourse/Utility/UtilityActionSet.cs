using System.Collections.Generic;
using System.Text;
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
        
        private void Bind(Action action, IConsideration consideration)
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
                list.Add(new ActionConsiderationPair(action, consideration));
            }
            else throw new System.ArgumentException("Action "+action.Name+" already bound to a consideration");
        }

        public List<ActionScorePair> ScoreAllActions(StringBuilder info)
        {
            
            scores.Clear();
            foreach (ActionConsiderationPair pair in list)
            {
                float score = pair.consideration.GetScore(info);
                scores.Add(new ActionScorePair(pair.action, score));
                info.AppendLine(pair.action.Name + " --> " + score + "\n");
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
        public Action action;
        public IConsideration consideration; // quite often this will be a Scorer
        
        public ActionConsiderationPair(Action action, IConsideration consideration)
        {
            this.action = action;
            this.consideration = consideration;
        }
    }
    
    public struct ActionScorePair {
        public Action action;
        public float score;
        
        public ActionScorePair(Action action, float score)
        {
            this.action = action;
            this.score = score;
        }
    }   
}



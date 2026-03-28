using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BTs;
using Action = BTs.Action;

namespace Utility
{
    public class UtilityExecutor : MonoBehaviour 
    {
        public UtilityActionSet actionSet;
        private Action currentAction;
        public string actionName;
        public float currentScore;
        public Status status;
        public float scoringInterval = 0.2f;
        public float inertiaThreshold = 0.1f;

        private StringBuilder info = new StringBuilder();
        private float timeSinceLastScoring = 0;
        private List<ActionScorePair> currentScores;

        void Start()
        {
            if (actionSet == null) return; // should we complain instead of silently returning?

            if (GetComponent<DynamicBlackboard>() == null)
            {
                Debug.LogError("Executing \'Utility\' without a Dynamic Blackboard in " + gameObject.name +
                               " \nIf a blackboard is not needed, just add an empty one");
            }

            actionSet = (UtilityActionSet)ScriptableObject.CreateInstance(actionSet.GetType().Name);

            actionSet.OnConstruction();
            if (actionSet.list.Count == 0)
            {
                throw new ArgumentException("UtilityActionSet given to executor in"  + gameObject.name + " is empty (has no action/consideration pairs)");
            }

            actionSet.Contextualize(gameObject);
        }

        void Update()
        {
            info.Clear();
            
            if (actionSet == null) throw new System.NullReferenceException("UtilityExecutor.actionSet is null");

            timeSinceLastScoring += Time.deltaTime;

            if (currentAction == null || currentAction.IsTerminated())
            {
                // first frame or after termination of the current action
                currentScores = actionSet.ScoreAllActions(info);
                currentAction = currentScores[0].action;
                currentScore = currentScores[0].score;
                actionName = currentAction.Name;

                currentAction.Initialize();

                timeSinceLastScoring = 0;
            }
            else if (timeSinceLastScoring >= scoringInterval)
            {
                // let's re-evaluate utility. This may involve abortion of the current action
                currentScores = actionSet.ScoreAllActions(info);

                if (currentScores[0].action != currentAction)
                {
                    // best-scoring action has changed. Let's see how is currentAction scoring
                    float currentActionScoreNow = currentScores.Find(x => x.action == currentAction).score;
                    if (currentActionScoreNow + inertiaThreshold < currentScores[0].score)
                    {
                        // new action "wins". Let's chage 
                        currentAction.Abort();
                        currentAction = currentScores[0].action;
                        currentScore = currentScores[0].score;
                        actionName = currentAction.Name;
                        currentAction.Initialize();
                    }
                }

                timeSinceLastScoring = 0;
            }

            // always tick the current action
            status = currentAction.Tick();
        }
    }
}
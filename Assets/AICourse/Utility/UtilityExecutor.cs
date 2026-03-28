using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BTs;
using Action = BTs.Action;  // otherwise it's ambiguous since System also has Action

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

        [TextArea(15, 30)]
        public string debugInfoText;
        
        private float timeSinceLastScoring = 0;
        private List<ActionScorePair> currentScores;
        
        private StringBuilder debugInfo = new StringBuilder();

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
            if (actionSet == null) throw new System.NullReferenceException("UtilityExecutor.actionSet is null");

            timeSinceLastScoring += Time.deltaTime;

            // should we re-evaluate?
            bool needsEvaluation = currentAction == null || 
                                   currentAction.IsTerminated() || 
                                   timeSinceLastScoring >= scoringInterval;

            if (needsEvaluation)
            {
                currentScores = actionSet.ScoreAllActions();
                debugInfo.Clear();

                
                if (currentAction == null || currentAction.IsTerminated()) // No action selected yet, or current action is terminated
                {
                    debugInfo.AppendLine($"*** STARTING ACTION: {currentScores[0].action.Name} score: {currentScores[0].score} ***\n");
                    currentAction = currentScores[0].action;
                    currentScore = currentScores[0].score;
                    actionName = currentAction.Name;
                    currentAction.Initialize();
                } else {  // it's time to check if we should abort the current action or keeo it going
                    
                    if (currentScores[0].action != currentAction)
                    {
                        // there's a new best action candidate. Let's see if inertia allows the current action to continue or not
                        float currentActionScoreNow = currentScores.Find(x => x.action == currentAction).score;
                        
                        if (currentActionScoreNow + inertiaThreshold < currentScores[0].score)
                        {
                            debugInfo.AppendLine($"*** ABORTING ACTION: {currentAction.Name} score: {currentActionScoreNow} + {inertiaThreshold}  ***");
                            debugInfo.AppendLine($"*** SELECTED ACTION: {currentScores[0].action.Name} score: {currentScores[0].score} ***\n");
                            
                            currentAction.Abort();
                            currentAction = currentScores[0].action;
                            currentScore = currentScores[0].score;
                            actionName = currentAction.Name;
                            currentAction.Initialize();
                        }
                        else
                        {
                            debugInfo.AppendLine($"*** INERTIA KEPT ACTION: {currentAction.Name} (Score: {currentActionScoreNow}). Top was {currentScores[0].action.Name} ({currentScores[0].score}) ***\n");
                        }
                    }
                    else
                    {
                        debugInfo.AppendLine($"*** MAINTAINING ACTION: {currentAction.Name} score: {currentScores[0].score} ***\n");
                    }
                }

                // full information regarding all scored actions
                debugInfo.AppendLine("ALL ACTIONS EVALUATION SUMMARY");
                debugInfo.AppendLine("------------------------------");
                debugInfo.AppendLine();
                foreach (ActionScorePair pair in currentScores)
                {
                    debugInfo.AppendLine($"[ACTION] {pair.action.Name} : {pair.score:F2}");
                    if (pair.consideration != null) 
                    {
                        pair.consideration.AppendDebugInfo(debugInfo, 1);
                    }
                    debugInfo.AppendLine();
                }
                
                debugInfoText = debugInfo.ToString();
                timeSinceLastScoring = 0;
            }

            // always tick the current action
            if (currentAction != null) status = currentAction.Tick();
        }
    }
}
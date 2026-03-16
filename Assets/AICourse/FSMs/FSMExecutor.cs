
using UnityEngine;
using TMPro;

namespace FSMs
{
    public class FSMExecutor : MonoBehaviour
    {

        public FiniteStateMachine fsm;
        public string currentState;
        public TextMeshProUGUI textMesh;
        private string originalText;

        void Start()
        {
            if (fsm == null) return;
            fsm = (FiniteStateMachine)ScriptableObject.CreateInstance(fsm.GetType().Name);
            fsm.Construct(gameObject);
            fsm.OnEnter();
            currentState = fsm.currentState.Name;
            if (textMesh != null) originalText = textMesh.text;
        }

       
        void FixedUpdate()
        {
            if (fsm == null) return;
            fsm.Update();
            currentState = fsm.currentState.Name;
            if (textMesh!=null)
                if (originalText!=null && originalText.Length>0)
                    textMesh.text = originalText+" " + currentState;
                else 
                    textMesh.text = "State: "+currentState;
        }
    }
}

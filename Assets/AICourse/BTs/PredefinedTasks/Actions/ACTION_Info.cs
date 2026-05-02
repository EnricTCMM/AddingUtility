using BTs;
using UnityEngine;

public class ACTION_Info : Action {

    public string keyMessage;
    public string keyInfoPanel;
    public string keyAppend;

    public ACTION_Info(string keyMessage, string keyInfoPanel="onScreenInfo", string keyAppend="true")
    {
        this.keyMessage = keyMessage;
        this.keyInfoPanel = keyInfoPanel;
        this.keyAppend = keyAppend;
    }

    public override Status OnTick()
    {
        // find the info panel where we will display the message
        OnScreenInfo panel = blackboard.Get<OnScreenInfo>(keyInfoPanel);
        if (panel == null)
        {
            Debug.LogError("Could not find OnScreenInfo panel with key " + keyInfoPanel);
            return Status.FAILED;
        }
        string message = blackboard.Get<string>(keyMessage);
        bool append = blackboard.Get<bool>(keyAppend);
        
        panel.InjectInfo(message,append); // display the message
        return Status.SUCCEEDED;
    }
}

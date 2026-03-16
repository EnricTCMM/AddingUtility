using UnityEngine;
using TMPro;
using BTs;

public class ACTION_Speak : Action
{

    public string keyMessage;


    // construtor
    public ACTION_Speak(string keyMessage)  {
        this.keyMessage = keyMessage;
    }

    private string theMessage;
    private GameObject bubble;
    private TextMeshPro textLine;

    public override void OnInitialize()
    {
        theMessage = blackboard.Get<string>(keyMessage);
        bubble = FindChildWithTag(gameObject, "BUBBLE");
        if (bubble != null) textLine = bubble.transform.GetChild(0).GetComponent<TextMeshPro>();
    }

    public override Status OnTick ()
    {
        if (textLine != null)
        {
            bubble.SetActive(true);
            textLine.text = theMessage;
            return Status.SUCCEEDED;
        }
        else
            return Status.FAILED;
    }

   
    // -----------------

    private GameObject FindChildWithTag(GameObject go, string tag)
    {
        for (int i=0; i<go.transform.childCount; i++)
        {
            if (go.transform.GetChild(i).tag == tag)
                return go.transform.GetChild(i).gameObject;
        }
        return null;
    }

}

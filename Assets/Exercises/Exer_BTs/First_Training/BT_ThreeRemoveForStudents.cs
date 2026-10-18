using UnityEngine;
using BTs;

[CreateAssetMenu(fileName = "BT_ThreeRemoveForStudents", menuName = "Behaviour Trees/BT_ThreeRemoveForStudents", order = 1)]
public class BT_ThreeRemoveForStudents : BehaviourTree
{
    

  
    
    public override void OnConstruction()
    {

        Sequence moneySeq = new Sequence(
            new CONDITION_InstanceNear("moneyDetectionRadius", "moneyTag", "false", "theMoney"),
            new ACTION_Take("theMoney"),
            new ACTION_Arrive("bank"),
            new ACTION_Drop("theMoney"),
            new ACTION_Arrive("home")
        );

        Sequence rubbishSeq = new Sequence(
            new CONDITION_InstanceNear("trashDetectionRadius", "trashTag", "false", "theTrash"),
            new ACTION_Take("theTrash"),
            new ACTION_Arrive("dump"),
            new ACTION_Drop("theTrash"),
            new ACTION_Arrive("home")
        );

        Sequence nothingSeq = new Sequence(
            new ACTION_Speak("Nothing\nto do"),
            new ACTION_WaitForSeconds("2"),
            new ACTION_Quiet()
        );
        
        root = new Sequence(
            new ACTION_Arrive("home"),
            new Selector(moneySeq, rubbishSeq, nothingSeq)
        );
    }
}

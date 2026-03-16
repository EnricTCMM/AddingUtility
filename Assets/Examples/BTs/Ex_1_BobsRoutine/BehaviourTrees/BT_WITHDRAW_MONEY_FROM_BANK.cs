using UnityEngine;
using BTs;

[CreateAssetMenu(fileName = "BT_WITHDRAW_MONEY_FROM_BANK", menuName = "Behaviour Trees/BT_WITHDRAW_MONEY_FROM_BANK", order = 1)]
public class BT_WITHDRAW_MONEY_FROM_BANK : BehaviourTree
{
    public override void OnConstruction()
    {
        root = new Selector(
            new Sequence(
                new ACTION_Arrive("theBank"),
                new ACTION_MAkeWithdrawal(),
                new ACTION_PlaySound("moneysound"),
                new ACTION_Activate("theDollars"),
                new ACTION_WaitForSeconds("3"),
                new ACTION_Deactivate("theDollars")
            ),
            new Sequence(
                new ACTION_PlaySound("theCurseSound"),
                new ACTION_Fail()
            )
        );
    }
}

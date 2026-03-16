using UnityEngine;
using UnityEditor;

public class ScriptFromTemplateCreator
{
    private const string pathToTemplate_BT = "Assets/AICourse/BTs/Editor/Templates/BT_Template.cs.txt";
    private const string pathToTemplate_Action = "Assets/AICourse/BTs/Editor/Templates/Action_Template.cs.txt";
    private const string pathToTemplate_Condition = "Assets/AICourse/BTs/Editor/Templates/Condition_Template.cs.txt";
    private const string pathToTemplate_FSM = "Assets/AICourse/BTs/Editor/Templates/FSM_Template.cs.txt";
    private const string pathToTemplate_LinearSteering = "Assets/AICourse/BTs/Editor/Templates/LinearSteering_Template.cs.txt";

    [MenuItem(itemName: "Assets/Create/C# BT Script", isValidateFunction: false, priority: 22)]
    public static void CreateScriptFromTemplate_00()
    {
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(pathToTemplate_BT, "new BT.cs");
    }

    [MenuItem(itemName: "Assets/Create/C# ACTION Script", isValidateFunction: false, priority: 22)]
    public static void CreateScriptFromTemplate_01()
    {
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(pathToTemplate_Action, "new Action.cs");
    }

    [MenuItem(itemName: "Assets/Create/C# CONDITION Script", isValidateFunction: false, priority: 22)]
    public static void CreateScriptFromTemplate_02()
    {
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(pathToTemplate_Condition, "new Condition.cs");
    }

    [MenuItem(itemName: "Assets/Create/C# FSM Script", isValidateFunction: false, priority: 22)]
    public static void CreateScriptFromTemplate_03()
    {
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(pathToTemplate_FSM, "new FSM.cs");
    }

    [MenuItem(itemName: "Assets/Create/C# Linear Steering Script", isValidateFunction: false, priority: 22)]
    public static void CreateScriptFromTemplate_04()
    {
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(pathToTemplate_LinearSteering, "new Steering.cs");
    }
}

using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

public enum SnapItemType
{
    Default, Battery, Gearbox, Chuck, ElectricMotor, Trigger, RearBody, FrontBody, LED, Screw, Complete_Drill, drill, OrangeBox, OrangeBoxFinal
}

[System.Serializable]
public class Item
{
    public SnapItemType type;
    public string name;
    public string itemModule;
    public Sprite picture;
    public int stepNumber;
    public bool isDefective;

    [TextArea]
    public string description;
}

//Stores which main tab and environment sub-tab a button belongs to for content panel.
using UnityEngine;

public enum MainTab
{
    Environment,
    Characters,
    Triggers,
    Goals,
    Utility
}

public enum EnvironmentGroup
{
    None,
    Trees,
    Rocks,
    Ground,
    Crystals
}

public class ButtonInfo : MonoBehaviour
{
    public MainTab mainTab;
    public EnvironmentGroup environmentSubTab;
}
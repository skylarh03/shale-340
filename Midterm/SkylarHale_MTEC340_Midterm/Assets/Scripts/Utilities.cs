using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public enum GameState
    {
        Play, Pause, WinLevel, UpgradeScreen, Death, GameOver, TitleScreen, Cutscene
    }

    public enum Powerups
    {
        SuperHammer, BoomerangFlower, IceFlower
    }
    
    public enum Upgrades
    {
        BlueShell, DashPepper, SuperMushroom, GoombasShoe, SuperLeaf, SuperHammer, 
        // BoomerangFlower, IceFlower
    }

    public static List<string> UpgradeNames = new List<string>()
    {
        "Blue Shell",
        "Dash Pepper",
        "Super Mushroom",
        "Goomba's Shoe",
        "Super Leaf",
        "Super Hammer"
        //"Boomerang Flower",
        //"Ice Flower",
    };
    
    public static List<string> UpgradeDescriptions =  new List<string>()
    {
        "Allows Mario to dash (press Right Shift)", 
        "Increases Mario's base speed", 
        "Increases Mario's Max HP by 1", 
        "Increases Mario's jump height", 
        "Allows Mario to glide after jumping (press Space mid-jump) for some amount of time",
        "Increases duration of Hammer",
        // "Unlocks Boomerang Flower",
        // "Unlocks Ice Flower"
    };

    public static void PlaySound(AudioSource source, AudioClip clip, bool loop = false)
    {
        source.clip = clip;
        source.Play();
        source.loop = loop;
    }

    public static Upgrades GenerateRandomUpgrade()
    {
        Upgrades upgradeToReturn = (Upgrades)Random.Range(0, System.Enum.GetValues(typeof(Upgrades)).Length);
        return upgradeToReturn;
    }
}

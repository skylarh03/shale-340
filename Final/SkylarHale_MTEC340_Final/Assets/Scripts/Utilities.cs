using UnityEngine;

public static class Utilities
{
    public enum Root
    {
        C,
        Db,
        D,
        Eb,
        E,
        F,
        Gb,
        G,
        Ab,
        A,
        Bb,
        B
    }

    public enum Mode
    {
        Ionian,
        Dorian,
        Phrygian,
        Lydian,
        Mixolydian,
        Aeolian,
        Locrian
    }

    public enum GameState
    {
        MainMenu,
        Gameplay
    }

    public enum PauseState
    {
        On, Off
    }
}

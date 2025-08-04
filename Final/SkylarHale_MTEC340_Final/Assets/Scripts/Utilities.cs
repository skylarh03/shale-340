using UnityEngine;

public static class Utilities
{
    // Corresponds to RootSelection Switch Group in Wwise. Options must be named exactly how they are in Wwise
    public enum Root
    {
        C, CSharp, D, Eb, E, F, FSharp, G, Ab, A, Bb, B
    }

    // Similar to above, but for the ModeSelection SwitchGroup instead
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
}

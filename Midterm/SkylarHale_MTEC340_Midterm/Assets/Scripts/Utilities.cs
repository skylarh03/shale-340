using UnityEngine;

public static class Utilities
{
    public enum GameState
    {
        Play, Pause, WinLevel, PointsShop, Death, GameOver, TitleScreen, Cutscene
    }

    public static void PlaySound(AudioSource source, AudioClip clip, bool loop = false)
    {
        source.clip = clip;
        source.Play();
        source.loop = loop;
    }
}

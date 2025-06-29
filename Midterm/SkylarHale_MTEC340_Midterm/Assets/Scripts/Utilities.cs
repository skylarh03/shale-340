using UnityEngine;

public static class Utilities
{
    public enum GameState
    {
        Play, Pause, Death, GameOver, TitleScreen
    }

    public static void PlaySound(AudioSource source, AudioClip clip)
    {
        source.clip = clip;
        source.Play();
    }
}

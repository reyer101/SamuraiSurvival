using UnityEngine;

// Constants
public class Constants : MonoBehaviour {
    // Audio
    public static string FireSpellAudio = "Audio/FireSpell";
    public static string EnemySpellAudio = "Audio/EnemySpell";

    // General Animation
    private static string AnimationPrefix = "AnimationControllers/";  

    // Player Animations 
    public static string GirlPrefix = AnimationPrefix + "Girl/";
    public static string BoyPrefix = AnimationPrefix + "Boy/";
    public static string Walk = "main_walk";
    public static string Crouch = "main_crouch";
    public static string Jump = "main_jump";     
}

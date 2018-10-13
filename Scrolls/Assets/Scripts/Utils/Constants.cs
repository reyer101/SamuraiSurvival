using UnityEngine;

// Constants
public class Constants : MonoBehaviour {
    // Audio

    // Prefixes
    private static string PREFIX_ANIM = "AnimationControllers/";
    private static string PREFIX_SPRITE = "Sprites/";

    // Player Animations 
    public static string ANIM_EMPTY = "";
    public static string ANIM_WALK = PREFIX_ANIM + "main_walk_0";
    public static string ANIM_JUMP = PREFIX_ANIM + "main_walk_1";
    public static string ANIM_BLOCK = PREFIX_ANIM + "main_walk_2";
    
    // Player Sprites
    public static string SPRITE_BLOCK = PREFIX_SPRITE + "block";
    public static string SPRITE_IDLE = PREFIX_SPRITE + "main_walk_0";
}

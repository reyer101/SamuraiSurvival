using UnityEngine;

// Constants
public class Constants : MonoBehaviour {
    
    // Prefixes
    private static string PREFIX_ANIM = "AnimationControllers/";
    private static string PREFIX_SPRITE = "Sprites/";
    private static string PREFIX_OBJECT = "Objects/";
    private static string PREFIX_AUDIO = "Audio/";

    // Player Animations 
    public static string ANIM_EMPTY = "";
    public static string ANIM_WALK = PREFIX_ANIM + "main_walk_0";
    public static string ANIM_WALKN = PREFIX_ANIM + "main_walk_no_sword_0";
    public static string ANIM_ATTACK1 = PREFIX_ANIM + "attack_1_0";
    public static string ANIM_ATTACK2 = PREFIX_ANIM + "attack_2_0";
    public static string ANIM_THROW = PREFIX_ANIM + "throw_0";

    // Player Sprites
    public static string SPRITE_BLOCK = PREFIX_SPRITE + "block";
    public static string SPRITE_JUMP = PREFIX_SPRITE + "jump_main";
    public static string SPRITE_JUMPN = PREFIX_SPRITE + "jump_no_sword";
    public static string SPRITE_IDLE = PREFIX_SPRITE + "main_walk_1";
    public static string SPRITE_VULNERABLE = PREFIX_SPRITE + "vulnerable";
    
    // Game Objects
    public static string OBJECT_SWORD = PREFIX_OBJECT + "sword";
    
    // Audio 
    public static string CLIP_SWING = PREFIX_AUDIO + "sword_slash{0}";
    public static string CLIP_BLOCK = PREFIX_AUDIO + "sword_block{0}";
    public static string CLIP_IMPACT = PREFIX_AUDIO + "sword_impact";
    
    // Tags
    public static string TAG_SWORD = "Sword";
    public static string TAG_SHADOW = "Shadow";
}

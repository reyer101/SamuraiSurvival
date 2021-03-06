﻿using UnityEngine;

// Constants
public class Constants {
    
    // Prefixes
    private static readonly string PREFIX_ANIM = "AnimationControllers/";
    private static readonly string PREFIX_SPRITE = "Sprites/";
    private static readonly string PREFIX_OBJECT = "Objects/";
    private static readonly string PREFIX_AUDIO = "Audio/";

    // Animations 
    public static readonly string ANIM_EMPTY = "";
    public static readonly string ANIM_WALK = PREFIX_ANIM + "main_walk_0";
    public static readonly string ANIM_WALKN = PREFIX_ANIM + "main_walk_no_sword_0";
    public static readonly string ANIM_ATTACK1 = PREFIX_ANIM + "attack_1_0";
    public static readonly string ANIM_ATTACK2 = PREFIX_ANIM + "attack_2_0";
    public static readonly string ANIM_THROW = PREFIX_ANIM + "throw_0";
    public static readonly string ANIM_SHADOW_PULSE = "Pulse";
    public static readonly string ANIM_SHADOW_FADE = "Fade";

    // Player Sprites
    public static readonly string SPRITE_BLOCK = PREFIX_SPRITE + "block";
    public static readonly string SPRITE_JUMP = PREFIX_SPRITE + "jump_main";
    public static readonly string SPRITE_JUMPN = PREFIX_SPRITE + "jump_no_sword";
    public static readonly string SPRITE_IDLE = PREFIX_SPRITE + "main_walk_1";
    public static readonly string SPRITE_VULNERABLE = PREFIX_SPRITE + "vulnerable";
    
    // Game Objects
    public static readonly string OBJECT_SWORD = PREFIX_OBJECT + "sword";
    public static readonly string OBJECT_HEALTHBAR = "HealthBar";
    public static readonly string OBJECT_HEALTHFOREGROUND = "Foreground";
	public static readonly string OBJECT_SHADOW = "Shadows/Shadow{0}";
    
    // Audio 
    public static readonly string CLIP_SWING = PREFIX_AUDIO + "sword_slash{0}";
    public static readonly string CLIP_BLOCK = PREFIX_AUDIO + "sword_block{0}";
    public static readonly string CLIP_IMPACT = PREFIX_AUDIO + "sword_impact";
    
    // Tags
    public static readonly string TAG_SWORD = "Sword";
    public static readonly string TAG_SHADOW = "Shadow";
	public static readonly string TAG_MENU = "Menu";

	// Keys
	public static readonly string KEY_DIFFICULTY = "keyDifficulty";

	// Difficulties
	public static readonly string EASY = "Easy";
	public static readonly string MED = "Med";
	public static readonly string HARD = "Hard";

	public enum Move {
		MOVE_ATTACK,
		MOVE_BLOCK,
		MOVE_POWERUP
	}
}

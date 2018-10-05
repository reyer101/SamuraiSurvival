using System.Collections;
using System.Collections.Generic;
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

    // Boss Animations    
    public static string BossPrefix = AnimationPrefix + "Kitty/";
    public static string Transform = BossPrefix + "kitty_transform";
    public static string Idle = BossPrefix + "kitty_idle";
    public static string Return = BossPrefix + "kitty_return";

    // Player Prefs
    public static string CheckpointX = "checkpointx";
    public static string CheckpointY = "checkpointy";
    public static string Scene = "scene";

    // Dialogue
    public static string Player = "[Player Name]";
    public static Dictionary<int, string[]> Dialogue = new Dictionary<int, string[]>()
    {
        {1, new string[] { "Headmistress Llewelyn", "Oh! There you are, [Player Name]. We'd been worried about you." } },
        {2, new string[] { "[Player Name]", "Sorry... I got a little lost." } },
        {3, new string[] { "Headmistress Llewelyn", "That's quite all right. Now, You better run ahead if you don't want to be late for class" } },
        {4, new string[] { "Headmistress Llewelyn", "Remember, you can move forward or back with the left analog stick." } },
        {5, new string[] { "Headmistress Llewelyn", "Oh, there seems to be a desk in the way here. You can jump over it with A." } },
        {6, new string[] { "Headmistress Llewelyn", "Looks like this space is a little cramped. You can crouch and crawl under it with B. " } },
        {7, new string[] { "Professor Burnfront", "Hey, [Player Name]! I'm glad you made it. I see you found your spell scroll. Let's see what it says." } },
        {8, new string[] { "Fire Scroll", "The curious wizard or witch will take heed, that fire is a dangerous friend to wield indeed. " } },
        {9, new string[] { "Fire Scroll", "But if you so must, in this flame you can trust, to rend your enemies to nothing but dust." } },
        {10, new string[] { "Professor Burnfront", "Well, kiddo, let's test your new spell out. Make sure to aim at the torch and hit the right trigger to cast." } },
        {11, new string[] { "Professor Burnfront", "You did a great job with the torch. Now let's see how you do with a more lively target." } },
        {12, new string[] { "Professor Burnfront", "Excellent, [Player Name]! I know you'll only improve from here." } },
        {13, new string[] { "Professor Burnfront", "For now, you should head over to your next class. Good luck!" } },
        {14, new string[] { "Professor Corvid", "Hello, [Player Name]. Go ahead and collect your scroll and we may begin the lesson." } },
        {15, new string[] { "Levitation Scroll", "If one lacks strength, this spell could be kind, for all that it takes is the power of the mind. " } },
        {16, new string[] { "Levitation Scroll", "Large or small, heavy or light, your magic will lift any object in sight." } },
        {17, new string[] { "Professor Corvid", "Alright. How about you try it?" } },
        {18, new string[] { "Professor Corvid", "Activate the spell with your right trigger and move the rock around with the right analog stick." } },
        {19, new string[] { "Professor Corvid", "Ah. This might look tricky, but all you must do is press Y to toggle between the different targets." } },
        {20, new string[] { "Professor Corvid", "Oh, splendid! You figured that one out without my instruction." } },
        {21, new string[] { "Professor Corvid", "Heavy objects like that can subdue nearly any creature." } },
        {22, new string[] { "Professor Corvid", "Hello, Headmistress! What could I help you with?" } },
        {23, new string[] { "Headmistress Llewelyn", "You two haven’t seen Mr. Snickerwhiskers anywhere have you? He seems to have run off..." } },
        {24, new string[] { "Professor Corvid", "No, I have not. But perhaps, our new student here could help you out with that." } },
        {25, new string[] { "[Player Name]", "I would be happy to find your cat for you, Headmistress." } },
        {26, new string[] { "Headmistress Llewelyn", "Thank you, [Player Name]. But please be careful out there and remember what you've learned. " } },
        {27, new string[] { "[Player Name]", "I will!" } },
        {28, new string[] {  "[Player Name]", "Mr. Snickerwhiskers has to be out here somewhere." } },
        {29, new string[] { "???", "Meow… meowww." } },
        {30, new string[] { "[Player Name]", "Oh. Mr. Snickerwhiskers must be inside!" } },
        {31, new string[] { "[Player Name]", "Mr. Snickerwhiskers? Where are you?" } },
        {32, new string[] { "[Player Name]", "C’mon, Mr. Snickerwhiskers. It’s time to go home." } },
        {33, new string[] { "Headmistress Llewelyn", "Oh! You found him. Thank you, [Player Name]. I’m so glad you found him." } },
        {34, new string[] { "Headmistress Llewelyn", "You are going to be an amazing sorcerer!" } },
        {35, new string[] { "", "The End" } }
    };
}

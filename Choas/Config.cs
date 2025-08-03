using RedRightHand.CustomPlugin;

namespace Choas;

public class Config : CustomPluginConfig
{
    // Enable/disable all custom keybinds. Probably leave this off if another plugin uses SSS, I was mostly messing around with stuff so this could break things
    public bool EnableCustomKeybinds { get; set; } = false;
    
    // Old features
    public bool EnableOldMechanics { get; set; } = false;
    
    // Pink candy events, including a command that changes it for one round
    public bool EnablePinkCandyEvents { get; set; } = false;
    // The % chance for a pink candy to be picked up, from 0 to 100
    public float PinkCandyChance { get; set; } = -1f;
    
    // The path to the folder where sound files are kept. Make sure to add a slash at the end
    public string AudioClipFolderPath { get; set; } = "";
}
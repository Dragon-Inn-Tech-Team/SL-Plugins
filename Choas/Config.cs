using RedRightHand.CustomPlugin;

namespace Choas;

public class Config : CustomPluginConfig
{
    // Enable/disable all custom keybinds
    public bool EnableCustomKeybinds { get; set; } = false;
    
    // Old features
    public bool EnableOldMechanics { get; set; } = false;
    
    // Pink candy events, including a command that changes it for one round
    public bool EnablePinkCandyEvents { get; set; } = false;
    // The % chance for a pink candy to be picked up, from 0 to 100
    public float PinkCandyChance { get; set; } = -1f;
}
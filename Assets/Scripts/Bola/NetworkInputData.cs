using Fusion;
using UnityEngine;

/// <summary>
/// So this is tricky, this byte buttons have slot for 8 buttons [0000][0000] i guess.
/// Each variabel assign a bool [0,1] to a slot in a byte.
/// MOUSEBUTTON1 is 0x01
/// 
/// ==The conversion is HEX to BINARY==
/// 
/// so it will take slot 1. (1 if button is pressed, 0 otherwise)
/// 
/// SHIFT is 0x04 => 0100, take slot 3
/// IF "SHIFT" TAKE LOWER, IT WILL ALTER PRECEEDING SLOTS
/// Say SHIFT 0x03 => 0011, it will take slot 1 AND 2. So not good
/// 
/// Make sure each button assigned to a new slot in the byte variable
/// </summary>
enum ExtraButtonsEnum
{
    WALL = 0,
}
public struct NetworkInputData : INetworkInput
{
    public const byte MOUSEBUTTON1 = 0x01;
    public const byte MOUSEBUTTON2 = 0x02;

    public const byte SHIFT = 0x04;
    public const byte SPACE = 0x08;

    public byte buttons;
    public Vector3 direction;

    // Docs network input
    public NetworkButtons extraButtons;
}
using System.Numerics;
using Robust.Shared.GameStates;

namespace Content.Shared._ES.Viewcone.Components;

/// <summary>
/// ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡠⠀⠀⠀⡀⢄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
/// ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢰⣴⡿⣟⣿⣻⣦⠆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
/// ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣲⣯⢿⡽⣞⣷⣻⡞⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
/// ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡈⢿⣽⣯⢿⡽⣞⣷⡻⢥⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
/// ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
/// ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠰⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
/// ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠐⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢃⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
/// ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣣⣀⣀⡀⠀⠀⠀⠀⠀⠀⢀⣀⣀⣬⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
/// ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣌⣿⡽⣯⣟⣿⣻⢟⣿⡻⣟⣯⢿⡽⣯⡡⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
/// ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠰⣽⣷⣻⢷⣻⢾⣽⣻⢾⣽⣻⣞⣯⣟⡷⣧⡆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
/// ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢳⣟⡾⣽⢯⡿⣽⢾⣽⣻⣞⣷⣻⢾⣭⣟⡷⡞⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
/// ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣘⠉⠙⠙⠯⠿⢽⣯⣟⣾⣳⣟⣾⡽⠻⠾⠙⠋⠉⢁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
/// ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠆⠀⠀⠀⠀⠀⠀⠀⠀⠀
/// ⠀⠀⠀⠀⢀⣤⣤⡶⣶⣶⡆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢰⣶⣶⢶⣤⣠⡀⠀⠀⠀
/// ⠀⠀⠀⣰⣟⡷⣯⣟⣷⡻⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⢷⣯⣟⡾⣽⣳⣆⠀⠀
/// ⠀⠀⢀⡿⣾⣽⣳⣟⡾⢳⣟⣿⣲⢦⣤⣄⣀⣀⠀⠀⠀⠀⠀⠀⢀⣀⣀⣤⣴⢶⡿⣽⡛⣾⣽⢻⣷⣻⢾⡄⠀
/// ⠀⠀⢸⣟⡷⣯⢷⣯⣏⡿⣽⢾⣽⣻⢾⡽⣯⣟⡿⣻⣟⢿⣻⣟⡿⣯⣟⡷⣯⢿⣽⣳⢿⣹⣞⡿⡾⣽⣻⣄⠀
/// ⠀⠀⣿⢾⡽⣯⣟⡾⣼⡽⣯⣟⡾⣽⢯⣟⡷⣯⣟⡷⣯⣟⡷⣯⣟⡷⣯⢿⣽⣻⢾⡽⣯⢧⢯⡿⣽⣳⣟⣎⠀
/// ⠀⢰⣯⢿⣽⣳⢯⡷⣯⣟⡷⣯⢿⣽⣻⢾⣽⣳⢯⣟⡷⣯⣟⡷⣯⢿⣽⣻⢾⡽⣯⢿⣽⣻⢾⣽⣳⣟⡾⣽⠇
/// ⠀⣼⣞⡿⡾⣽⢯⣟⡷⣯⢿⣽⣻⢾⣽⣻⢾⣽⣻⢾⣽⣳⢯⡿⣽⣻⢾⡽⣯⢿⣽⣻⢾⣽⣻⣞⡷⣯⢿⣽⣣
/// ⢠⡾⣽⣻⡽⣯⣟⡾⣽⢯⣟⡾⣽⣻⢾⣽⣻⢾⣽⣻⢾⡽⣯⣟⡷⣯⢿⡽⣯⣟⡾⣽⣻⣞⡷⣯⢿⣽⣻⣞⣷
/// ⠀⠻⣷⣯⣟⡷⣯⢿⣽⣻⢾⣽⣳⢯⣟⡾⣽⣻⢾⡽⣯⣟⡷⣯⢿⡽⣯⣟⡷⣯⣟⣷⣻⢾⡽⣯⣟⣾⣳⢿⡞
/// ⠀⠀⠈⠓⠛⠙⠋⠛⠚⠙⠛⠚⠋⠛⠚⠛⠓⠛⠋⠛⠓⠋⠛⠙⠋⠛⠓⠋⠛⠓⠛⠚⠙⠋⠛⠓⠛⠚⠛⠉⠀
///           THE CONE MAN APPROACHES
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ESViewconeComponent : Component
{
    /// <summary>
    ///     Base cone angle, without any modifications (from equipment or otherwise).
    /// </summary>
    /// <remarks>
    ///     You probably don't want to refer to this directly if you're using it for actual calculations.
    ///     Instead, use <see cref="ESViewconeAngleSystem.GetModifiedViewconeAngle"/>
    /// </remarks>
    [DataField, AutoNetworkedField]
    public float BaseConeAngle = 250f;

    [DataField, AutoNetworkedField]
    public float ConeFeather = 10f;

    [DataField, AutoNetworkedField]
    public float ConeIgnoreRadius = 0.5f;

    // Funky, specifies cone radius when blind (e.g. inside a container)
    [DataField, AutoNetworkedField]
    public float ConeIgnoreRadiusBlind = 0f;

    [DataField, AutoNetworkedField]
    public float ConeIgnoreFeather = 0.08f;

    // Clientside, used for lerping view angle
    // and keeping it consistent across all overlays
    public Angle ViewAngle;
    public Angle? DesiredViewAngle = null;
    public Angle LastMouseRotationAngle;
    public Vector2 LastWorldPos;
    public Angle LastWorldRotationAngle;

    // Funky start
    public bool IsBlind = false;

    // Keeping this negative ensures no items are shown
    // when blind
    public readonly float BaseConeAngleBlind = -10f;
    public float CurrentConeAngle = 250f;
    public float DesiredConeAngle = 250f;
    // Funky end
}

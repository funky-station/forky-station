namespace Content.Server._Funkystation.Unions;

/// <summary>
/// Given to department heads under the Captain, including the Commandant. These jobs should not be able to end up in union positions.
/// We should probably use this more instead of CommandStaff because making Commandant CommandStaff isn't really accurate.
/// </summary>
[RegisterComponent]
public sealed partial class DepartmentHeadComponent : Component
{

}

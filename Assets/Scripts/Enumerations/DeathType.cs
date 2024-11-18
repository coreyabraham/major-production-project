/// <summary>
/// An Enumeration that dictates what Type of Death is being currently used to any Script that reads it. <br/>
/// (Used by: `PlrDeathState.cs`)
/// </summary>
public enum DeathType
{
    /// <summary>
    /// The Default Death Type, this will not activate any `PriorDeath` Action!
    /// </summary>
    Default = 0,

    /// <summary>
    /// This State is used when being killed by a Crow, this'll delay the Death Screen a tad so that the Crow has a chance to do their animation work!
    /// Used in: Level_Alleyway_Changes
    /// </summary>
    Prayed,

    /// <summary>
    /// This State is used whenever being burnt alive.
    /// Used in: Level_Kitchen
    /// </summary>
    Burned,
    
    /// <summary>
    /// This state is used whenver drowning in water.
    /// Used in Level_Kitchen
    /// </summary>
    Drowned
}
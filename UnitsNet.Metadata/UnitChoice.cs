namespace UnitsNet.Metadata;

/// <summary>
/// A unit that a quantity property can be shown in.
/// </summary>
public sealed class UnitChoice
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnitChoice"/> class.
    /// </summary>
    /// <param name="id">The unit name qualified by its quantity name</param>
    /// <param name="name">The unit name</param>
    /// <param name="symbol">The unit symbol</param>
    public UnitChoice(string id, string name, string symbol)
    {
        Id = id;
        Name = name;
        Symbol = symbol;
    }

    /// <summary>
    /// Gets the unit name qualified by its quantity name.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the unit name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the unit symbol.
    /// </summary>
    public string Symbol { get; }
}

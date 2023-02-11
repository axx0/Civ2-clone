using Civ;
using JetBrains.Annotations;

namespace TOT;

/// <summary>
/// This class is dynamically instantiated when ToT files are detected
/// </summary>
[UsedImplicitly]
public class TestOfTimeInterface : Civ2Interface
{
    public override string Title => "Test of Time";
}
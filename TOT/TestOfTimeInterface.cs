using Civ;
using JetBrains.Annotations;

namespace TOT;

/// <summary>
/// This class is dynamically instantiated when ToT files are detected
/// </summary>
[UsedImplicitly]
public class TestOfTimeInterface : CivInterface
{
    public override string Title => "Test of Time";
}
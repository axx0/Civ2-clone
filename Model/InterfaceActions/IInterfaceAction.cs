namespace Model;

public interface IInterfaceAction
{
    string Name { get; }
    EventType ActionType { get; }
    MenuElements? MenuElement { get; }
    OpenFileInfo? FileInfo { get; }
}
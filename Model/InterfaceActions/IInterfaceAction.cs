namespace Model;

public interface IInterfaceAction
{
    EventType ActionType { get; }
    MenuElements? MenuElement { get; }
    OpenFileInfo? FileInfo { get; }
}
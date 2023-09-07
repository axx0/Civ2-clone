namespace Model.InterfaceActions;

public interface IInterfaceAction
{
    string Name { get; }
    EventType ActionType { get; }
}
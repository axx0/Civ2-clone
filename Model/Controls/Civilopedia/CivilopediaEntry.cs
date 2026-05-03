namespace Model.Controls.Civilopedia;

public class CivilopediaEntry(CivilopediaInfoType infoType, CivilopediaWindowType windowType, int id = 0)
{
    public CivilopediaInfoType InfoType { get; set; } = infoType;
    public CivilopediaWindowType WindowType { get; set; } = windowType;
    public int Id { get; set; } = id;

    public CivilopediaEntry Clone()
    {
        return new CivilopediaEntry(InfoType, WindowType, Id);
    }
}

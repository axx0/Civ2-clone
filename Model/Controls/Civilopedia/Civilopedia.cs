namespace Model.Controls;

public class Civilopedia
{
    public CivilopediaInfoType InfoType { get; set; }
    public CivilopediaWindowType WindowType { get; set; }
    public int Id { get; set; }

    public Civilopedia(CivilopediaInfoType infoType, CivilopediaWindowType windowType, int id = 0)
    {
        InfoType = infoType;
        WindowType = windowType;
        Id = id;
    }

    public Civilopedia Clone()
    {
        return new Civilopedia(InfoType, WindowType, Id);
    }
}

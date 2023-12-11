namespace Model.Images;

public record BinaryStorage : IImageSource
{
    public ImageStorage Type => ImageStorage.Binary;
    public string GetKey(int ownerId = -1)
    {
        return Key;
    }

    public string Key => $"Binary-{FileName}-{DataStart}";
    
    public string FileName { get; set; }
    public int DataStart { get; set; }
    
    public int Length { get; set; }
}
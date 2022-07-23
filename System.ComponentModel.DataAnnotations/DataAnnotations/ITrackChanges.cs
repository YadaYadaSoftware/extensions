namespace System.ComponentModel.DataAnnotations;

public interface ITrackChanges
{
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}
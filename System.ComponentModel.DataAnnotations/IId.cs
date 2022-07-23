namespace System.ComponentModel.DataAnnotations;

//todo: Move to Entity.Library
public interface IId<T>
{
    public T Id { get; set; }
}

public interface IId : IId<Guid>
{
}
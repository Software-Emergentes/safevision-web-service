namespace SafeVisionPlatform.Shared.Domain.Repositories;

public interface IUnitOfWork
{
    Task CompleteAsync();
}
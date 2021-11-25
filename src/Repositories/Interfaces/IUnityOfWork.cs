namespace SACA.Repositories.Interfaces
{
    public interface IUnityOfWork
    {
        void SaveChanges();
        Task SaveChangesAsync();
    }
}

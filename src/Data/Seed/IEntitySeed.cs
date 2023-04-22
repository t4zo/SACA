namespace SACA.Data.Seed
{
    public interface IEntitySeed
    {
        Task LoadAsync(LoadAsyncOptions loadAsyncOptions = null);
    }
}
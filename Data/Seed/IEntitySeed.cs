using System.Threading.Tasks;

namespace SACA.Data.Seed
{
    public interface IEntitySeed
    {
        Task LoadAsync();
    }
}
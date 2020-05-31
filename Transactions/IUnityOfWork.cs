using System.Threading.Tasks;

namespace SACA.Transactions
{
    public interface IUnityOfWork
    {
        public void Commit();
        public Task CommitAsync();
        public void Rollback();
    }
}

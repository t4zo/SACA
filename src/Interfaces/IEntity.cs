namespace SACA.Entities
{
    public interface IBaseEntity<TKey> where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }
    }

    public interface IEntity : IBaseEntity<int>
    {
    }
}
using System;

namespace SACA.Models
{
    public abstract class BaseEntity<TKey> where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }
    }

    public abstract class BaseEntity : BaseEntity<int>
    {
    }
}
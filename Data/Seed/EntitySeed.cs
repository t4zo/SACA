using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SACA.Models;

namespace SACA.Data.Seed
{
    public class EntitySeed<TEntity> : IEntitySeed where TEntity : BaseEntity
    {
        protected readonly ApplicationDbContext _context;

        protected EntitySeed(ApplicationDbContext context, string ressourceName)
        {
            _context = context;
            RessourceName = ressourceName;
        }

        protected string RessourceName { get; set; }

        public virtual async Task LoadAsync()
        {
            var dbSet = _context.Set<TEntity>();

            if (!dbSet.Any())
            {
                var assembly = Assembly.GetExecutingAssembly();

                await using var stream = assembly.GetManifestResourceStream(RessourceName);
                using var reader =
                    new StreamReader(stream ?? throw new ExternalException(nameof(TEntity)), Encoding.UTF8);

                var json = await reader.ReadToEndAsync();
                var entities = JsonSerializer.Deserialize<List<TEntity>>(json);

                if (entities is not null)
                    foreach (var entity in entities)
                        await dbSet.AddAsync(entity);

                await _context.SaveChangesAsync();
            }
        }
    }
}
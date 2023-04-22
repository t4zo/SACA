using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace SACA.Data.Seed
{
    public class LoadAsyncOptions
    {
        public bool UploadImage { get; set; }
    }
    
    public class EntitySeed<TEntity> : IEntitySeed where TEntity : class
    {
        protected readonly ApplicationDbContext _context;

        protected string RessourceName { get; set; }

        protected EntitySeed(ApplicationDbContext context, string ressourceName)
        {
            _context = context;
            RessourceName = ressourceName;
        }

        public virtual async Task LoadAsync(LoadAsyncOptions loadAsyncOptions = null)
        {
            var dbSet = _context.Set<TEntity>();

            if (!dbSet.Any())
            {
                var assembly = Assembly.GetExecutingAssembly();

                await using var stream = assembly.GetManifestResourceStream(RessourceName);
                using var reader = new StreamReader(stream ?? throw new ExternalException(nameof(TEntity)), Encoding.UTF8);

                var json = await reader.ReadToEndAsync();
                var entities = JsonSerializer.Deserialize<List<TEntity>>(json);

                if (entities is not null)
                {
                    foreach (var entity in entities)
                    {
                        await dbSet.AddAsync(entity);
                    }
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
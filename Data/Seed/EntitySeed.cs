using SACA.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SACA.Data.Seed
{
    public class EntitySeed<TEntity> : IEntitySeed where TEntity : BaseEntity
    {
        protected readonly ApplicationDbContext _context;
        protected string RessourceName { get; set; }

        protected EntitySeed(ApplicationDbContext context, string ressourceName)
        {
            _context = context;
            RessourceName = ressourceName;
        }

        public virtual async Task LoadAsync()
        {
            var dbSet = _context.Set<TEntity>();

            if (!dbSet.Any())
            {
                var assembly = Assembly.GetExecutingAssembly();

                using var stream = assembly.GetManifestResourceStream(RessourceName);
                using var reader = new StreamReader(stream, Encoding.UTF8);

                string json = await reader.ReadToEndAsync();
                List<TEntity> entities = JsonConvert.DeserializeObject<List<TEntity>>(json);

                foreach (var entity in entities)
                {
                    await dbSet.AddAsync(entity);
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}

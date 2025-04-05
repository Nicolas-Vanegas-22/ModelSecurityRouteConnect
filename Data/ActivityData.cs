using Microsoft.Extensions.Logging;
using Entity.Model;
using Entity.Context;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gesti�n de la entidad Rol en la base de datos.
    /// </summary>
    public class ActivityData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        ///<summary>
        ///Constructor que recibe el contexto de base de datos.
        ///</summary>
        ///<param name="=context">Instancia de <see cref="ApplicationDbContext"/> para la conexi�n con la base de datos.</param>
        public ActivityData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        ///<summary>
        ///Obtiene todos los roles almacenados en la base de datos.
        ///</summary>
        ///<returns>Lista de roles.</returns>
        public async Task<IEnumerable<Activity>> GetAllAsync()
        {
            return await _context.Set<Activity>().ToListAsync();
        }

        public async Task<Activity?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Activity>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener actividad con ID {ActivityId}", id);
                throw; //Re-lanza la excepci�n para que sea manejada en capas superiores
            }
        }

        ///<summary>
        ///Crea un nuevo rol en la base de datos.
        ///</summary>
        ///<param name="activity">Instancia del rol a crear.</param>
        ///<returns>El rol creado.</returns>
        public async Task<Activity> CreateAsync(Activity activity)
        {
            try
            {
                await _context.Set<Activity>().AddAsync(activity);
                await _context.SaveChangesAsync();
                return activity;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear actividad: {ex.Message}");
                throw;
            }
        }

        ///<summary>
        ///Actualiza un rol existente en la base de datos.
        ///</summary>
        ///<param name="activity">Objeto con la informaci�n actualizada.</param>
        ///<returns>True si la operaci�n fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Activity activity)
        {
            try
            {
                _context.Set<Activity>().Update(activity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar actividad: {ex.Message}");
                return false;
            }
        }

        ///<summary>
        ///Elimina un rol de la base de datos.
        ///</summary>
        ///<param name="id">Identificador �nico del rol a eliminar.</param>
        ///<returns>True si la operaci�n fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var activity = await _context.Set<Activity>().FindAsync(id);
                if (activity == null)
                    return false;

                _context.Set<Activity>().Remove(activity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar actividad: {ex.Message}");
                return false;
            }
        }
    }
}
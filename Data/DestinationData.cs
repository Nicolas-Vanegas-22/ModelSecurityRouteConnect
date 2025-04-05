using Microsoft.Extensions.Logging;
using Entity.Model;
using Entity.Context;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gestión de la entidad Rol en la base de datos.
    /// </summary>
    public class DestinationData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        ///<summary>
        ///Constructor que recibe el contexto de base de datos.
        ///</summary>
        ///<param name="=context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        public DestinationData(ApplicationDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        ///<summary>
        ///Obtiene todos los roles almacenados en la base de datos.
        ///</summary>
        ///<returns>Lista de roles.</returns>
        public async Task<IEnumerable<Destination>> GetAllAsync()
        {
            return await _context.Set<Destination>().ToListAsync();
        }

        public async Task<Destination?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Destination>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener destino con ID {DestinationId}", id);
                throw; //Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        ///<summary>
        ///Crea un nuevo rol en la base de datos.
        ///</summary>
        ///<param name="destination">Instancia del rol a crear.</param>
        ///<returns>El rol creado.</returns>
        public async Task<Destination> CreateAsync(Destination destination)
        {
            try
            {
                await _context.Set<Destination>().AddAsync(destination);
                await _context.SaveChangesAsync();
                return destination;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear pago: {ex.Message}");
                throw;
            }
        }

        ///<summary>
        ///Actualiza un rol existente en la base de datos.
        ///</summary>
        ///<param name="destination">Objeto con la información actualizada.</param>
        ///<returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Destination destination)
        {
            try
            {
                _context.Set<Destination>().Update(destination);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar destino: {ex.Message}");
                return false;
            }
        }

        ///<summary>
        ///Elimina un rol de la base de datos.
        ///</summary>
        ///<param name="id">Identificador único del rol a eliminar.</param>
        ///<returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var destination = await _context.Set<Destination>().FindAsync(id);
                if (destination == null)
                    return false;

                _context.Set<Destination>().Remove(destination);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar destino: {ex.Message}");
                return false;
            }
        }
    }
}
using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    public class RolPermission
    {
        private readonly RolPermissionData _rolPermissionData;
        private readonly ILogger _logger;

        public RolPermission(RolPermissionData rolPermissionData, ILogger logger)
        {
            _rolPermissionData = rolPermissionData;
            _logger = logger;
        }

        // Obtener todos los permisos
        public async Task<IEnumerable<RolPermissionDTO>> GetAllAsync()
        {
            try
            {
                var permissions = await _rolPermissionData.GetAllAsync();
                return permissions.Select(p => new RolPermissionDTO
                {
                    RolPermissionId = p.RolPermissionId,
                    Name = p.Name
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los permisos de rol");
                throw new ExternalServiceException("Base de datos", "Error al recuperar los permisos", ex);
            }
        }

        // Obtener permiso por ID
        public async Task<RolPermissionDTO> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID inválido: {RolPermissionId}", id);
                throw new ValidationException("id", "El ID del permiso debe ser mayor que cero");
            }

            try
            {
                var permission = await _rolPermissionData.GetByIdAsync(id);
                if (permission == null)
                {
                    _logger.LogInformation("Permiso no encontrado: {RolPermissionId}", id);
                    throw new EntityNotFoundException("RolPermission", id);
                }

                return new RolPermissionDTO
                {
                    RolPermissionId = permission.RolPermissionId,
                    Name = permission.Name
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el permiso con ID: {RolPermissionId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el permiso con ID {id}", ex);
            }
        }

        // Crear nuevo permiso
        public async Task<RolPermissionDTO> CreateAsync(RolPermissionDTO dto)
        {
            try
            {
                Validate(dto);

                var entity = new Entity.Model.RolPermission
                {
                    Name = dto.Name
                };

                var created = await _rolPermissionData.CreateAsync(entity);

                return new RolPermissionDTO
                {
                    RolPermissionId = created.RolPermissionId,
                    Name = created.Name
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo permiso: {Name}", dto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el permiso", ex);
            }
        }

        // Validación
        private void Validate(RolPermissionDTO dto)
        {
            if (dto == null)
                throw new ValidationException("El objeto permiso no puede ser nulo");

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                _logger.LogWarning("Intento de crear/actualizar permiso con Name vacío");
                throw new ValidationException("Name", "El Name del permiso es obligatorio");
            }
        }
    }
}


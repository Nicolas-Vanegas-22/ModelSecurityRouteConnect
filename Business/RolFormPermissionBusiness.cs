using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    public class RolFormPermission
    {
        private readonly RolFormPermissionData _rolFormPermissionData;
        private readonly ILogger _logger;

        public RolFormPermission(RolFormPermissionData rolFormPermissionData, ILogger logger)
        {
            _rolFormPermissionData = rolFormPermissionData;
            _logger = logger;
        }

        // Obtener todos los permisos de formularios
        public async Task<IEnumerable<RolFormPermissionDTO>> GetAllAsync()
        {
            try
            {
                var list = await _rolFormPermissionData.GetAllAsync();
                return list.Select(x => new RolFormPermissionDTO
                {
                    RolFormPermissionId = x.RolFormPermissionId,
                    Name = x.Name
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los permisos de formularios");
                throw new ExternalServiceException("Base de datos", "Error al recuperar los permisos", ex);
            }
        }

        // Obtener uno por ID
        public async Task<RolFormPermissionDTO> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID inválido: {RolFormPermissionId}", id);
                throw new ValidationException("id", "El ID debe ser mayor que cero");
            }

            try
            {
                var formPermission = await _rolFormPermissionData.GetByIdAsync(id);
                if (formPermission == null)
                {
                    _logger.LogInformation("Permiso no encontrado: {RolFormPermissionId}", id);
                    throw new EntityNotFoundException("RolFormPermission", id);
                }

                return new RolFormPermissionDTO
                {
                    RolFormPermissionId = formPermission.RolFormPermissionId,
                    Name = formPermission.Name
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener permiso con ID: {RolFormPermissionId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el permiso con ID {id}", ex);
            }
        }

        // Crear nuevo permiso de formulario
        public async Task<RolFormPermissionDTO> CreateAsync(RolFormPermissionDTO dto)
        {
            try
            {
                Validate(dto);

                var entity = new Entity.Model.RolFormPermission
                {
                    Name = dto.Name
                };

                var created = await _rolFormPermissionData.CreateAsync(entity);

                return new RolFormPermissionDTO
                {
                    RolFormPermissionId = created.RolFormPermissionId,
                    Name = created.Name
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear permiso de formulario: {Name}", dto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el permiso", ex);
            }
        }

        // Validación
        private void Validate(RolFormPermissionDTO dto)
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

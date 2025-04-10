using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los permission del sistema.
    /// </summary>

    public class PermissionBusiness
    {
        private readonly PermissionData _permissionData;
        private readonly ILogger _logger;

        public PermissionBusiness(PermissionData permissionData, ILogger logger)
        {
            _permissionData = permissionData;
            _logger = logger;
        }

        // Método para obtener todos los Permission como DTOs
        public async Task<IEnumerable<PermissionDTO>> GetAllPermissionAsync()
        {
            try
            {
                var permissions = await _permissionData.GetAllAsync();
                var permissionDTOs = new List<PermissionDTO>();

                foreach (var permission in permissions)
                {
                    permissionDTOs.Add(new PermissionDTO
                    {
                        permissionId = permission.PermissionId,
                        PermissionName = permission.PermissionName,
                        Description = permission.Description
                    });
                }

                return permissionDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los permission");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de permissions", ex);
            }
        }

        // Método para obtener un permissions por ID como DTO
        public async Task<PermissionDTO> GetPermissionByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un permission con ID inválido: {personId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del permission debe ser mayor que cero");
            }

            try
            {
                var permission = await _permissionData.GetByIdAsync(id);
                if (permission == null)
                {
                    _logger.LogInformation("No se encontró ningún Permission con ID: {PermissionId}", id);
                    throw new EntityNotFoundException("Permission", id);
                }

                return new PermissionDTO
                {
                    permissionId = permission.PermissionId,
                    PermissionName = permission.PermissionName,
                    Description = permission.Description
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el Permission con ID: {PermissionId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el permission con ID {id}", ex);
            }
        }

        // Método para crear un Permission desde un DTO
        public async Task<PermissionDTO> CreatePermissionAsync(PermissionDTO PermissionDTO)
        {
            try
            {
                ValidatPermission(PermissionDTO);

                var permission = new Permission
                {
                    PermissionId = PermissionDTO.permissionId,
                    PermissionName = PermissionDTO.PermissionName,
                    Description = PermissionDTO.Description
                };

                var createdPermission = await _permissionData.CreateAsync(permission);

                return new PermissionDTO
                {
                    permissionId = createdPermission.PermissionId,
                    PermissionName = createdPermission.PermissionName,
                    Description = createdPermission.Description
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo Permission: {PermissionNombre}", PermissionDTO?.PermissionName ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el Permission", ex);
            }
        }

        private static object GetPermissionCreado() => GetPermissionCreado();

        // Método para validar el Permissions
        private void ValidatPermission(PermissionDTO PermissionDto)
        {
            if (PermissionDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto Permission no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace((string?)PermissionDto.PermissionName))
            {
                _logger.LogWarning("Se intentó crear/actualizar un Person con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del permission es obligatorio");
            }
        }
    }
}
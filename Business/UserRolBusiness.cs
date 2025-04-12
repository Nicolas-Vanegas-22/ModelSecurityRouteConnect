using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    public class UserRol
    {
        private readonly UserRolData _userRolData;
        private readonly ILogger _logger;

        public UserRol(UserRolData userRolData, ILogger logger)
        {
            _userRolData = userRolData;
            _logger = logger;
        }

        // Obtener todos los roles
        public async Task<IEnumerable<UserRolDTO>> GetAllAsync()
        {
            try
            {
                var roles = await _userRolData.GetAllAsync();
                var dtoList = roles.Select(r => new UserRolDTO
                {
                    UserRolId = r.UserRolId,
                    Name = r.Name
                });

                return dtoList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los roles de usuario");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de roles", ex);
            }
        }

        // Obtener rol por ID
        public async Task<UserRolDTO> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("ID inválido: {UserRolId}", id);
                throw new ValidationException("id", "El ID del rol debe ser mayor que cero");
            }

            try
            {
                var rol = await _userRolData.GetByIdAsync(id);
                if (rol == null)
                {
                    _logger.LogInformation("Rol no encontrado: {UserRolId}", id);
                    throw new EntityNotFoundException("UserRol", id);
                }

                return new UserRolDTO
                {
                    UserRolId = rol.UserRolId,
                    Name = rol.Name
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rol con ID: {UserRolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el rol con ID {id}", ex);
            }
        }

        // Crear nuevo rol
        public async Task<UserRolDTO> CreateAsync(UserRolDTO dto)
        {
            try
            {
                Validate(dto);

                var entity = new Entity.Model.UserRol
                {
                    Name = dto.Name
                };

                var created = await _userRolData.CreateAsync(entity);

                return new UserRolDTO
                {
                    UserRolId = created.UserRolId,
                    Name = created.Name
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo rol: {Name}", dto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el rol", ex);
            }
        }

        // Validación
        private void Validate(UserRolDTO dto)
        {
            if (dto == null)
                throw new ValidationException("El objeto rol no puede ser nulo");

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                _logger.LogWarning("Intento de crear/actualizar rol con Name vacío");
                throw new ValidationException("Name", "El Name del rol es obligatorio");
            }
        }
    }
}


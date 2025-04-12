using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Server;
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

        // Método para obtener todos los roles de usuario como DTOs
        public async Task<IEnumerable<UserRolDTO>> GetAllUserRolAsync()
        {
            try
            {
                var userRolList = await _userRolData.GetAllAsync();
                var userRolDTOList = new List<UserRolDTO>();

                foreach (var userRol in userRolList)
                {
                    userRolDTOList.Add(new UserRolDTO
                    {
                        UserRolId = userRol.UserRolId,
                        Name = userRol.Name
                    });
                }

                return userRolDTOList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los roles de usuario");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de roles de usuario", ex);
            }
        }

        // Método para obtener un rol de usuario por ID como DTO
        public async Task<UserRolDTO> GetUserRolByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un rol de usuario con ID inválido: {UserRolBusinessId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del rol debe ser mayor que cero");
            }

            try
            {
                var userRol = await _userRolData.GetByIdAsync(id);
                if (userRol == null)
                {
                    _logger.LogInformation("No se encontró ningún rol con ID: {UserRolId}", id);
                    throw new EntityNotFoundException("UserRol", id);
                }

                return new UserRolDTO
                {
                    UserRolId = userRol.UserRolId,
                    Name = userRol.Name,
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rol de usuario con ID: {UserRolBusinessId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el rol con ID {id}", ex);
            }
        }

        // Método para crear un rol de usuario desde un DTO
        public async Task<UserRolDTO> CreateUserRolAsync(UserRolDTO userRolDto)
        {
            try
            {
                ValidateUserRol(userRolDto);

                var userRol = new UserRolDTO
                {
                    Name = userRolDto.Name,
                };

                var userRolCreado = await _userRolData.CreateAsync(userRol);

                return new UserRolDTO
                {
                    UserRolId = userRolCreado.UserRolId,
                    Name = userRolCreado.Name,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo rol de usuario: {Name}", userRolDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el rol", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateUserRol(UserRolDTO userRolDto)
        {
            if (userRolDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto rol no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(userRolDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un rol con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del rol es obligatorio");
            }
        }
    }
}


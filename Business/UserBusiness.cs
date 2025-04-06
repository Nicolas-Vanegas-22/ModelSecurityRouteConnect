using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
        /// <summary>
    /// Clase de negocio encargada de la lógica relacionada con los roles del sistema.
    /// </summary>

    public class UserBusiness
    {
        private readonly UserData _userData;
        private readonly ILogger _logger;

        public UserBusiness(UserData userData, ILogger logger)
        {
            _userData = userData;
            _logger = logger;
        }

        // Método para obtener todos los Users como DTOs
        public async Task<IEnumerable<UserDTO>> GetAllUserAsync()
        {
            try
            {
                var users = await _userData.GetAllAsync();
                var userDTO = new List<UserDTO>();

                foreach (var user in users)
                {
                    userDTO.Add(new UserDTO
                    {
                        UserId = user.UserId,
                        Username = user.Username,
                        Description = user.Description
                    });
                }
                
                return userDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los users");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de users", ex);
            }
        }

        // Método para obtener un User por ID como DTO
        public async Task<UserDTO> GetUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un rol con ID inválido: {RolId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del rol debe ser mayor que cero");
            }

            try
            {
                var user = await _userData.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogInformation("No se encontró ningún User con ID: {RolId}", id);
                    throw new EntityNotFoundException("User", id);
                }

                return new UserDTO
                {
                    UserId = user.userId,
                    Username = user.Username,
                    Description = user.Description
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rol con ID: {RolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el rol con ID {id}", ex);
            }
        }

        // Método para crear un User desde un DTO
        public async Task<UserDTO> CreateRolAsync(UserDTO UserDto)
        {
            try
            {
                ValidateUser(UserDto);

                var user = new User
                {
                    Username = UserDto.Username,
                    Description = UserDto.Description
                };

                var userCreado = await _userData.CreateAsync(user);

                return new UserDTO
                {
                    UserId = userCreado.UserId,
                    Username = userCreado.Username,
                    Description = userCreado.Description
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo user: {RolNombre}", UserDto?.Username ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el User", ex);
            }
        }

        // Método para validar el User
        private void ValidateUser(UserDTO UserDto)
        {
            if (UserDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto User no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(UserDto.Username))
            {
                _logger.LogWarning("Se intentó crear/actualizar un User con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del User es obligatorio");
            }
        }
    }
}
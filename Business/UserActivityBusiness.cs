using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Server;
using Utilities.Exceptions;

namespace Business
{
    public class UserActivityBusiness
    {
        private readonly UserActivityBusinessData _userActivityBusinessData;
        private readonly ILogger _logger;

        public UserActivityBusiness(UserActivityBusinessData userActivityBusinessData, ILogger logger)
        {
            _userActivityBusinessData = userActivityBusinessData;
            _logger = logger;
        }

        // Método para obtener todas las actividades de usuario como DTOs
        public async Task<IEnumerable<UserActivityBusinessDTO>> GetAllUserActivityBusinessAsync()
        {
            try
            {
                var userActivityBusinessList = await _userActivityBusinessData.GetAllAsync();
                var userActivityBusinessDTOList = new List<UserActivityBusinessDTO>();

                foreach (var userActivityBusiness in userActivityBusinessList)
                {
                    userActivityBusinessDTOList.Add(new UserActivityBusinessDTO
                    {
                        UserActivityBusinessId = userActivityBusiness.UserActivityBusinessId,
                        Name = userActivityBusiness.Name
                    });
                }

                return userActivityBusinessDTOList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las actividades de usuario");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de actividades de usuario", ex);
            }
        }

        // Método para obtener una actividad de usuario por ID como DTO
        public async Task<UserActivityBusinessDTO> GetUserActivityBusinessByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una actividad con ID inválido: {UserActivityBusinessId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la actividad debe ser mayor que cero");
            }

            try
            {
                var userActivityBusiness = await _userActivityBusinessData.GetByIdAsync(id);
                if (userActivityBusiness == null)
                {
                    _logger.LogInformation("No se encontró ninguna actividad con ID: {UserActivityBusinessId}", id);
                    throw new EntityNotFoundException("UserActivityBusiness", id);
                }

                return new UserActivityBusinessDTO
                {
                    UserActivityBusinessId = userActivityBusiness.UserActivityBusinessId,
                    Name = userActivityBusiness.Name,
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la actividad de usuario con ID: {UserActivityBusinessId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la actividad con ID {id}", ex);
            }
        }

        // Método para crear una actividad de usuario desde un DTO
        public async Task<UserActivityBusinessDTO> CreateUserActivityBusinessAsync(UserActivityBusinessDTO userActivityBusinessDto)
        {
            try
            {
                ValidateUserActivityBusiness(userActivityBusinessDto);

                var userActivityBusiness = new UserActivityBusiness
                {
                    Name = userActivityBusinessDto.Name,
                };

                var userActivityBusinessCreado = await _userActivityBusinessData.CreateAsync(userActivityBusiness);

                return new UserActivityBusinessDTO
                {
                    UserActivityBusinessId = userActivityBusinessCreado.UserActivityBusinessId,
                    Name = userActivityBusinessCreado.Name,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva actividad de usuario: {Name}", userActivityBusinessDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear la actividad", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateUserActivityBusiness(UserActivityBusinessDTO userActivityBusinessDto)
        {
            if (userActivityBusinessDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto actividad no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(userActivityBusinessDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar una actividad con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name de la actividad es obligatorio");
            }
        }
    }
}

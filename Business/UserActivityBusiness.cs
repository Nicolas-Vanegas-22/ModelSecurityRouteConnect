using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    public class UserActivity
    {
        private readonly UserActivityData _userActivityData;
        private readonly ILogger _logger;

        public UserActivity(UserActivityData userActivityData, ILogger logger)
        {
            _userActivityData = userActivityData;
            _logger = logger;
        }

        // Obtener todas las actividades de usuario
        public async Task<IEnumerable<UserActivityDTO>> GetAllAsync()
        {
            try
            {
                var userActivities = await _userActivityData.GetAllAsync();
                var dtoList = new List<UserActivityDTO>();

                foreach (var activity in userActivities)
                {
                    dtoList.Add(new UserActivityDTO
                    {
                        UserActivityId = activity.UserActivityId,
                        Name = activity.Name
                    });
                }

                return dtoList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las actividades de usuario");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de actividades de usuario", ex);
            }
        }

        // Obtener una actividad por ID
        public async Task<UserActivityDTO> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una actividad con ID inválido: {UserActivityId}", id);
                throw new ValidationException("id", "El ID de la actividad debe ser mayor que cero");
            }

            try
            {
                var activity = await _userActivityData.GetByIdAsync(id);
                if (activity == null)
                {
                    _logger.LogInformation("No se encontró ninguna actividad con ID: {UserActivityId}", id);
                    throw new EntityNotFoundException("UserActivity", id);
                }

                return new UserActivityDTO
                {
                    UserActivityId = activity.UserActivityId,
                    Name = activity.Name
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la actividad de usuario con ID: {UserActivityId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la actividad con ID {id}", ex);
            }
        }

        // Crear nueva actividad
        public async Task<UserActivityDTO> CreateAsync(UserActivityDTO dto)
        {
            try
            {
                Validate(dto);

                var entity = new Entity.Model.UserActivity
                {
                    Name = dto.Name
                };

                var created = await _userActivityData.CreateAsync(entity);

                return new UserActivityDTO
                {
                    UserActivityId = created.UserActivityId,
                    Name = created.Name
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva actividad de usuario: {Name}", dto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear la actividad", ex);
            }
        }

        // Validar DTO
        private void Validate(UserActivityDTO dto)
        {
            if (dto == null)
            {
                throw new ValidationException("El objeto actividad no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar una actividad con Name vacío");
                throw new ValidationException("Name", "El Name de la actividad es obligatorio");
            }
        }
    }
}

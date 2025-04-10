using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{

    public class ActivityBusiness
    {
        private readonly ActivityData _activityData;
        private readonly ILogger _logger;

        public ActivityBusiness(ActivityData activityData, ILogger logger)
        {
            _activityData = activityData;
            _logger = logger;
        }

        // Método para obtener todas las actividades como DTOs
        public async Task<IEnumerable<ActivityDTO>> GetAllActivitysAsync()
        {
            try
            {
                var activitys = await _activityData.GetAllAsync();
                var activitysDTO = new List<ActivityDTO>();

                foreach (var activity in activitys)
                {
                    activitysDTO.Add(new ActivityDTO
                    {
                        ActivityId = activity.ActivityId,
                        Name = activity.Name,
                        Description = activity.Description,
                        Category = activity.Category,
                        Price = activity.Price
                    });
                }

                return activitysDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las actividades");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de actividades", ex);
            }
        }

        // Método para obtener una actividad por ID como DTO
        public async Task<ActivityDTO> GetActivityByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una catividad con ID inválido: {ActivityId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de actividad debe ser mayor que cero");
            }

            try
            {
                var activity = await _activityData.GetByIdAsync(id);
                if (activity == null)
                {
                    _logger.LogInformation("No se encontró ningúna actividad con ID: {ActivityId}", id);
                    throw new EntityNotFoundException("Activity", id);
                }

                return new ActivityDTO
                {
                    ActivityId = activity.ActivityId,
                    Name = activity.Name,
                    Description = activity.Description,
                    Category = activity.Category,
                    Price = activity.Price
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la actividad con ID: {ActivityId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la actividad con ID {id}", ex);
            }
        }

        // Método para crear la actividad desde un DTO
        public async Task<ActivityDTO> CreateActivityAsync(ActivityDTO ActivityDto)
        {
            try
            {
                ValidateActivity(ActivityDto);

                var activity = new Activity
                {
                    Name = ActivityDto.Name,
                    Description = ActivityDto.Description,
                    Category = ActivityDto.Category,
                    Price = ActivityDto.Price
                };

                var activityCreado = await _activityData.CreateAsync(activity);

                return new ActivityDTO
                {
                    ActivityId = activityCreado.ActivityId,
                    Name = activityCreado.Name,
                    Description = activityCreado.Description,
                    Category = activityCreado.Category,
                    Price = activityCreado.Price
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva actividad: {Name}", ActivityDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el rol", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateActivity(ActivityDTO ActivityDto)
        {
            if (ActivityDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto actividad no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(ActivityDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar una actividad con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name de la actividad es obligatorio");
            }
        }
    }
}
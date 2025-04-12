using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Server;
using Utilities.Exceptions;

namespace Business
{
    public class RolPermissionBusiness
    {
        private readonly RolPermissionBusinessData _rolPermissionBusinessData;
        private readonly ILogger _logger;

        public RolPermissionBusiness(RolPermissionBusinessData rolPermissionBusinessData, ILogger logger)
        {
            _rolPermissionBusinessData = rolPermissionBusinessData;
            _logger = logger;
        }

        // Método para obtener todos los permisos de rol como DTOs
        public async Task<IEnumerable<RolPermissionBusinessDTO>> GetAllRolPermissionBusinessAsync()
        {
            try
            {
                var rolPermissionBusinessList = await _rolPermissionBusinessData.GetAllAsync();
                var rolPermissionBusinessDTOList = new List<RolPermissionBusinessDTO>();

                foreach (var rolPermissionBusiness in rolPermissionBusinessList)
                {
                    rolPermissionBusinessDTOList.Add(new RolPermissionBusinessDTO
                    {
                        RolPermissionBusinessId = rolPermissionBusiness.RolPermissionBusinessId,
                        Name = rolPermissionBusiness.Name
                    });
                }

                return rolPermissionBusinessDTOList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los permisos de rol");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de permisos", ex);
            }
        }

        // Método para obtener un permiso de rol por ID como DTO
        public async Task<RolPermissionBusinessDTO> GetRolPermissionBusinessByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un permiso con ID inválido: {RolPermissionBusinessId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del permiso debe ser mayor que cero");
            }

            try
            {
                var rolPermissionBusiness = await _rolPermissionBusinessData.GetByIdAsync(id);
                if (rolPermissionBusiness == null)
                {
                    _logger.LogInformation("No se encontró ningún permiso con ID: {RolPermissionBusinessId}", id);
                    throw new EntityNotFoundException("RolPermissionBusiness", id);
                }

                return new RolPermissionBusinessDTO
                {
                    RolPermissionBusinessId = rolPermissionBusiness.RolPermissionBusinessId,
                    Name = rolPermissionBusiness.Name,
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el permiso con ID: {RolPermissionBusinessId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el permiso con ID {id}", ex);
            }
        }

        // Método para crear un permiso de rol desde un DTO
        public async Task<RolPermissionBusinessDTO> CreateRolPermissionBusinessAsync(RolPermissionBusinessDTO rolPermissionBusinessDto)
        {
            try
            {
                ValidateRolPermissionBusiness(rolPermissionBusinessDto);

                var rolPermissionBusiness = new RolPermissionBusiness
                {
                    Name = rolPermissionBusinessDto.Name,
                };

                var rolPermissionBusinessCreado = await _rolPermissionBusinessData.CreateAsync(rolPermissionBusiness);

                return new RolPermissionBusinessDTO
                {
                    RolPermissionBusinessId = rolPermissionBusinessCreado.RolPermissionBusinessId,
                    Name = rolPermissionBusinessCreado.Name,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo permiso de rol: {Name}", rolPermissionBusinessDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el permiso", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateRolPermissionBusiness(RolPermissionBusinessDTO rolPermissionBusinessDto)
        {
            if (rolPermissionBusinessDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto permiso no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(rolPermissionBusinessDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un permiso con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del permiso es obligatorio");
            }
        }
    }
}

using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    /// <summary>
    /// Clase de negocio encargada de la l�gica relacionada con los roles del sistema.
    /// </summary>

    public class PaymentBusiness
    {
        private readonly PaymentData _PaymentData;
        private readonly ILogger _logger;

        public PaymentBusiness(PaymentData PaymentData, ILogger logger)
        {
            _PaymentData = PaymentData;
            _logger = logger;
        }

        // M�todo para obtener todos los Payment como DTOs
        public async Task<IEnumerable<PaymentDTO>> GetAllPaymentAsync()
        {
            try
            {
                var Payments = await _PaymentData.GetAllAsync();
                var PaymentsDTO = new List<PaymentDTO>();

                foreach (var payment in Payments)
                {
                    PaymentsDTO.Add(new PaymentDTO
                    {
                        PaymentId = payment.PaymentId,
                        PaymentMethod = payment.PaymentMethod,
                        Description = payment.Description
                    });
                }

                return PaymentsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los Payment");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de Payment", ex);
            }
        }

        // M�todo para obtener un User por ID como DTO
        public async Task<PaymentDTO> GetUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� obtener un Payment con ID inv�lido: {RolId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del Payment debe ser mayor que cero");
            }

            try
            {
                var payment = await _PaymentData.GetByIdAsync(id);
                if (payment == null)
                {
                    _logger.LogInformation("No se encontr� ning�n Payment con ID: {PaymentId}", id);
                    throw new EntityNotFoundException("User", id);
                }

                return new PaymentDTO
                {
                    PaymentId = payment.PaymentId,
                    PaymentMethod = payment.PaymentMethod,
                    PaymentAmount = payment.PaymentAmount
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el payment con ID: {PaymentId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el payment con ID {id}", ex);
            }
        }

        // M�todo para crear un payment desde un DTO
        public async Task<PaymentDTO> CreatePaymentAsync(PaymentDTO PaymentDto)
        {
            try
            {
                ValidatePayment(PaymentDto);

                var payment = new Payment
                {
                    PaymentId = PaymentDto.PaymentId,
                    PaymentMethod = PaymentDto.PaymentMethod,
                    Description = PaymentDto.Description
                };

                var paymentCreado = await _PaymentData.CreateAsync(payment);

                return new PaymentDTO
                {
                    PaymentId = paymentCreado.PaymentId,
                    PaymentMethod = paymentCreado.PaymentMethod,
                    Description = paymentCreado.Description
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo Payment: {RolNombre}", PaymentDto?.PaymentName ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el Payment", ex);
            }
        }

        // M�todo para validar el Payment
        private void ValidatePayment(PaymentDTO PaymentDto)
        {
            if (PaymentDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto Payment no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(PaymentDto.PaymentName))
            {
                _logger.LogWarning("Se intent� crear/actualizar un Payment con Name vac�o");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del Payment es obligatorio");
            }
        }
    }
}
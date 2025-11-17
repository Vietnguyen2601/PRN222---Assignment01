using System.Collections.Generic;
using System.Linq;
using EleVehicleDealer.DAL.Models;
using EleVehicleDealer.Domain.DTOs.Payments;

namespace EleVehicleDealer.BLL.Mappers
{
    public static class PaymentMapper
    {
        public static PaymentDto? ToPaymentDto(this Payment? payment)
        {
            if (payment == null) return null;

            return new PaymentDto
            {
                PaymentId = payment.PaymentId,
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status
            };
        }

        public static IEnumerable<PaymentDto> ToPaymentDtos(this IEnumerable<Payment>? payments) =>
            payments?.Select(p => p.ToPaymentDto()!).Where(dto => dto != null)! ?? Enumerable.Empty<PaymentDto>();
    }
}

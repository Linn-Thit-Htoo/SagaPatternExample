using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SagaPatternExample.OrderServiceApi.Features.Order.CreateOrder
{
    [Route("api/order")]
    [ApiController]
    public class CreateOrderEndpoint : ControllerBase
    {
        internal readonly ISender _sender;

        public CreateOrderEndpoint(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrderAsync([FromBody] CreateOrderCommand createOrderCommand, CancellationToken cs)
        {
            var result = await _sender.Send(createOrderCommand, cs);
            return Ok(result);
        }
    }
}

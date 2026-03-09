using Cart.API.Services;
using Contracts.Events;
using MassTransit;

namespace Cart.API.Consumers
{
    public class ProductPriceChangedConsumer : IConsumer<ProductPriceChangedIntegrationEvent>
    {
        private ChangePriceService _service;

        public ProductPriceChangedConsumer(ChangePriceService service)
        {
            _service = service;
        }

        public async Task Consume(ConsumeContext<ProductPriceChangedIntegrationEvent> context)
        {
            await _service.ChangePrice(context.Message.ProductId, context.Message.NewPrice);
        }
    }
}

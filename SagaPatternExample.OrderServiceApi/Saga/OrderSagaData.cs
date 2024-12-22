using MassTransit;

namespace SagaPatternExample.OrderServiceApi.Saga
{
    public class OrderSagaData : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; } = string.Empty;

        public Guid Invoice { get; set; }
        public bool IsOrderCompleted { get; set; }
    }
}

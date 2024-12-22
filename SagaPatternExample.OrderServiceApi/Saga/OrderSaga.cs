using MassTransit;
using SagaPatternExample.OrderServiceApi.Events;

namespace SagaPatternExample.OrderServiceApi.Saga
{
    public class OrderSaga : MassTransitStateMachine<OrderSagaData>
    {
        public State SubmitOrder { get; set; }
        public State ReduceStock { get; set; }

        public Event<OrderSubmittedEvent> OrderSubmitted { get; set; }
        public Event<StockReductionEvent> StockReduced { get; set; }

        public OrderSaga()
        {
            InstanceState(x => x.CurrentState);

            Event(() => OrderSubmitted, x => x.CorrelateById(o => o.Message.InvoiceNo));

            Initially(
                When(OrderSubmitted)
                    .Then(context =>
                    {
                        context.Saga.Invoice = context.Message.InvoiceNo;
                    })
                    .TransitionTo(ReduceStock)
                    .Publish(context => new OrderSubmittedEvent() {  })
            );
        }
    }
}

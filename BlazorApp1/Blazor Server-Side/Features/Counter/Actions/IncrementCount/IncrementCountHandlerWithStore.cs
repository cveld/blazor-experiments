using BlazorState;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// based on https://timewarpengineering.github.io/blazor-state/Sample.html
namespace BlazorServerSide.Features.Counter
{
    public partial class CounterState : State<CounterState>
    {
        // Scoped dependency
        public class IncrementCountHandlerWithStore : RequestHandler<IncrementCountAction, CounterState>
        {
            public IncrementCountHandlerWithStore(IStore aStore) : base(aStore) { }

            // Scoped property
            CounterState CounterState => Store.GetState<CounterState>();

            public override Task<CounterState> Handle(IncrementCountAction aIncrementCountAction, CancellationToken aCancellationToken)
            {
                CounterState.Count += aIncrementCountAction.Amount;
                return Task.FromResult(CounterState);
            }
        }
    }
}

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
        public class IncrementCountHandler : RequestHandler<IncrementCountAction, CounterState>
        {
            private readonly CounterState counterState;

            public IncrementCountHandler(IStore aStore, CounterState counterState): base(aStore) {
                this.counterState = counterState;
            }
            

            public override Task<CounterState> Handle(IncrementCountAction aIncrementCountAction, CancellationToken aCancellationToken)
            {
                counterState.Count += aIncrementCountAction.Amount;
                return Task.FromResult(counterState);
            }
        }
    }
}

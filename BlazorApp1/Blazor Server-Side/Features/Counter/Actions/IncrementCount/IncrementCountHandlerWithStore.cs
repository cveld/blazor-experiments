using BlazorServerSide.Features.Base;
using BlazorState;
using MediatR;
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
        internal class IncrementCountHandlerWithStore : BaseHandler<IncrementCountAction>
        {
            public IncrementCountHandlerWithStore(IStore aStore) : base(aStore) { }

            public override Task<Unit> Handle
            (
              IncrementCountAction aIncrementCounterAction,
              CancellationToken aCancellationToken
            )
            {
                CounterState.Count += aIncrementCounterAction.Amount;
                return Unit.Task;
            }
        }
    }
}

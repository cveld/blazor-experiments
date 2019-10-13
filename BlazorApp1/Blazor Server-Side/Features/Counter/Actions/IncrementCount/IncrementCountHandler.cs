﻿using BlazorServerSide.Features.Base;
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
        internal class IncrementCountHandler : BaseHandler<IncrementCountAction>
        {
            private readonly CounterState counterState;

            public IncrementCountHandler(IStore aStore, CounterState counterState): base(aStore) {
                this.counterState = counterState;
            }


            public override Task<Unit> Handle
            (
              IncrementCountAction aIncrementCountAction,
              CancellationToken aCancellationToken
            )
            {
                counterState.Count += aIncrementCountAction.Amount;
                return Unit.Task;
            }
        }
    }
}

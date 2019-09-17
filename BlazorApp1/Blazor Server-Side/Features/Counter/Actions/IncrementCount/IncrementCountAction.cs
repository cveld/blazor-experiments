using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// based on https://timewarpengineering.github.io/blazor-state/Sample.html
namespace BlazorServerSide.Features.Counter
{
    public class IncrementCountAction : IRequest<CounterState>
    {
        public int Amount { get; set; }
    }
}

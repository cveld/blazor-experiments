using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorState;

// based on https://timewarpengineering.github.io/blazor-state/Sample.html
namespace BlazorServerSide.Features.Counter
{
    public partial class CounterState : State<CounterState>
    {
        public int Count { get; private set; }
        protected override void Initialize() => Count = 3;
    }
}

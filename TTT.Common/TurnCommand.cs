using System;
using System.Collections.Generic;
using System.Text;

namespace TTT.Common
{
    public class TurnCommand
    {
        public Guid PlayerId { get; set; }

        public Cell Cell { get; set; }
    }
}

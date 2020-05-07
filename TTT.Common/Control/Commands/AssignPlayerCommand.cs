using System;
using System.Collections.Generic;
using System.Text;

namespace TTT.Common
{
    public class AssignPlayerCommand : ISubCommand
    {
        public Guid PlayerId { get; set; }
    }
}

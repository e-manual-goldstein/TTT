using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace TTT.Host
{
    public interface IUpdatingElement
    {
        bool ShouldUpdate { get; }

    }
}

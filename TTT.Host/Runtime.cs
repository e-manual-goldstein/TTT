using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTT
{
    public class Runtime
    {
        static GameMode _runMode;

        public static bool Debug { get => _runMode == GameMode.Debug; }
        public static bool Test { get => _runMode == GameMode.Test; }

        public static Random Randomiser = new Random(DateTime.Now.Millisecond);

        public static void SetMode(GameMode gameMode)
        {
            _runMode = gameMode;
        }
    }
}

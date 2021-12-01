using System;
using System.Collections.Generic;
using System.Text;

namespace Decoupling.Module
{
    public enum EnvType
    {
        Debug = 0,
        Local = 1,
        UnitTest = 2,
        Testing = 3,
        Development = 4,
        Staging = 5,
        PreRelease = 6,
        TestingPool = 7,
        Prod = 8,
        Live = 9,
    }
}

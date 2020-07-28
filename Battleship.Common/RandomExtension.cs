using System;

namespace Battleship.Common
{
    public static class RandomExtension
    {
        public static Boolean NextBoolean(this Random random)
        {
            return random.Next() > (Int32.MaxValue / 2);
            // Next() returns an int in the range [0..Int32.MaxValue]
        }
    }
}

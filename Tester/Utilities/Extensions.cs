﻿using System;
using System.Collections.Generic;

namespace ImpersonationTester
{
    static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
        {
            foreach( var i in ie )
            {
                action( i );
            }
        }
    }
}

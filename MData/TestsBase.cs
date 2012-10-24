using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace MData
{
    [DebuggerStepThrough]
    [ExcludeFromCodeCoverage]
    public abstract class TestsBase
    {
        protected void DoesThrow<T>(Action a) where T : Exception
        {
            try
            {
                a();
                Assert.Fail("An exception should have been raised.");
            }
            catch (T)
            {

            }
        }
    }
}
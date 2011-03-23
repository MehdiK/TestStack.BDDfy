using System;
using NUnit.Framework;

namespace Demos.NUnit
{
    public class SomeFailingTest
    {
        void GivenThisTestOrOneOfTheClassesItCallsToIsIncomplete()
        {
        }

        void WhenTheTestIsRun()
        {
            throw new ApplicationException("There was an error here");
        }

        void ThenItIsFlaggedAsIncomplete()
        {
            
        }

        [Test]
        public void Execute()
        {
            this.Bddify();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Constraints.Constraints;
using NUnit.Constraints;

public static class Error
{
    /// <summary>
    /// Verifies that executing the given Action delegate throws an exception
    /// of type T or an exception type that is a subclass of T
    /// </summary>
    public static T Expect<T>(Action action) where T : Exception
    {
        return (T)Expect(typeof(T), action);
    }
    /// <summary>
    /// Non-generic overload of Error.Expect&lt;T&gt;(Action).
    /// </summary>
    public static Exception Expect(Type exceptionType, Action action)
    {
        try
        {
            action();
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail(string.Format(
                "An exception of type {0} was expected, but no exception was thrown.",
                exceptionType.FullName));
        }
        catch (Exception ex)
        {
            TypeConstraint cons = new InstanceOfTypeConstraint(exceptionType);
            if (!cons.Matches(ex))
            {
                TextMessageWriter w = new TextMessageWriter();
                cons.WriteMessageTo(w);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail(w.ToString());
            }
            return ex;
        }
        return null; //making compiler happy.  this will never be reached.
    }

    public static T ExpectExact<T>(Action action) where T : Exception
    {
        return (T)ExpectExact(typeof(T), action);
    }

    public static Exception ExpectExact(Type exceptionType, Action action)
    {
        try
        {
            action();
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail(string.Format(
                "An exception of type {0} was expected, but no exception was thrown.",
                exceptionType.FullName));
        }
        catch (Exception ex)
        {
            TypeConstraint cons = new ExactTypeConstraint(exceptionType);
            if (!cons.Matches(ex))
            {
                TextMessageWriter w = new TextMessageWriter();
                cons.WriteMessageTo(w);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail(w.ToString());
            }
            return ex;
        }
        return null; //making compiler happy.  this will never be reached.
    }
}

// ****************************************************************
// Copyright 2009, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

// ****************************************************************
//              Generated by the NUnit Syntax Generator
//
// Command Line: GenSyntax.exe SyntaxElements.txt
// 
//                  DO NOT MODIFY THIS FILE DIRECTLY
// ****************************************************************

using System;
using System.Collections;
using System.ComponentModel;
using NUnit.Constraints.Constraints;
using AssertionException = Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException;
using InconclusiveException = Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Constraints;

/// <summary>
/// Delegate used by tests that execute code and
/// capture any thrown exception.
/// </summary>
public delegate void TestDelegate();

/// <summary>
/// The Assert class contains a collection of static methods that
/// implement the most common assertions used in NUnit.
/// </summary>
public class Verify
{
    /// <summary>
    /// We don't actually want any instances of this object, but some people
    /// like to inherit from it to add other static methods. Hence, the
    /// protected constructor disallows any instances of this object. 
    /// </summary>
    protected Verify() { }

    /// <summary>
    /// The Equals method throws an AssertionException. This is done 
    /// to make sure there is no mistake by calling this function.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static new bool Equals(object a, object b)
    {
        // TODO: This should probably be InvalidOperationException
        Assert.Fail("Assert.Equals should not be used for Assertions");
        return false;
    }

    /// <summary>
    /// override the default ReferenceEquals to throw an AssertionException. This 
    /// implementation makes sure there is no mistake in calling this function 
    /// as part of Assert. 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    public static new void ReferenceEquals(object a, object b)
    {
        Assert.Fail("Assert.ReferenceEquals should not be used for Assertions");
    }

    /// <summary>
    /// Helper for Assert.AreEqual(double expected, double actual, ...)
    /// allowing code generation to work consistently.
    /// </summary>
    /// <param name="expected">The expected value</param>
    /// <param name="actual">The actual value</param>
    /// <param name="delta">The maximum acceptable difference between the
    /// the expected and the actual</param>
    /// <param name="message">The message to display in case of failure</param>
    /// <param name="args">Array of objects to be used in formatting the message</param>
    protected static void AssertDoublesAreEqual(double expected, double actual, double delta, string message, object[] args)
    {
        if (double.IsNaN(expected) || double.IsInfinity(expected))
            Verify.That(actual, Is.EqualTo(expected), message, args);
        else
            Verify.That(actual, Is.EqualTo(expected).Within(delta), message, args);
    }

    /// <summary>
    /// Apply a constraint to an actual value, succeeding if the constraint
    /// is satisfied and throwing an assertion exception on failure.
    /// </summary>
    /// <param name="expression">A Constraint to be applied</param>
    /// <param name="actual">The actual value to test</param>
    static public void That(object actual, IResolveConstraint expression)
    {
        Verify.That(actual, expression, null, null);
    }

    /// <summary>
    /// Apply a constraint to an actual value, succeeding if the constraint
    /// is satisfied and throwing an assertion exception on failure.
    /// </summary>
    /// <param name="expression">A Constraint to be applied</param>
    /// <param name="actual">The actual value to test</param>
    /// <param name="message">The message that will be displayed on failure</param>
    static public void That(object actual, IResolveConstraint expression, string message)
    {
        Verify.That(actual, expression, message, null);
    }

    /// <summary>
    /// Apply a constraint to an actual value, succeeding if the constraint
    /// is satisfied and throwing an assertion exception on failure.
    /// </summary>
    /// <param name="expression">A Constraint expression to be applied</param>
    /// <param name="actual">The actual value to test</param>
    /// <param name="message">The message that will be displayed on failure</param>
    /// <param name="args">Arguments to be used in formatting the message</param>
    static public void That(object actual, IResolveConstraint expression, string message, params object[] args)
    {
        Constraint constraint = expression.Resolve();

        if (!constraint.Matches(actual))
        {
            MessageWriter writer = new TextMessageWriter(message, args);
            constraint.WriteMessageTo(writer);
            Assert.Fail(writer.ToString());
        }
    }

    /// <summary>
    /// Apply a constraint to an actual value, succeeding if the constraint
    /// is satisfied and throwing an assertion exception on failure.
    /// </summary>
    /// <param name="expr">A Constraint expression to be applied</param>
    /// <param name="del">An ActualValueDelegate returning the value to be tested</param>
    static public void That(ActualValueDelegate del, IResolveConstraint expr)
    {
        Verify.That(del, expr.Resolve(), null, null);
    }

    /// <summary>
    /// Apply a constraint to an actual value, succeeding if the constraint
    /// is satisfied and throwing an assertion exception on failure.
    /// </summary>
    /// <param name="expr">A Constraint expression to be applied</param>
    /// <param name="del">An ActualValueDelegate returning the value to be tested</param>
    /// <param name="message">The message that will be displayed on failure</param>
    static public void That(ActualValueDelegate del, IResolveConstraint expr, string message)
    {
        Verify.That(del, expr.Resolve(), message, null);
    }

    /// <summary>
    /// Apply a constraint to an actual value, succeeding if the constraint
    /// is satisfied and throwing an assertion exception on failure.
    /// </summary>
    /// <param name="del">An ActualValueDelegate returning the value to be tested</param>
    /// <param name="expr">A Constraint expression to be applied</param>
    /// <param name="message">The message that will be displayed on failure</param>
    /// <param name="args">Arguments to be used in formatting the message</param>
    static public void That(ActualValueDelegate del, IResolveConstraint expr, string message, params object[] args)
    {
        Constraint constraint = expr.Resolve();

        if (!constraint.Matches(del))
        {
            MessageWriter writer = new TextMessageWriter(message, args);
            constraint.WriteMessageTo(writer);
            Assert.Fail(writer.ToString());
        }
    }

    /// <summary>
    /// Apply a constraint to a referenced value, succeeding if the constraint
    /// is satisfied and throwing an assertion exception on failure.
    /// </summary>
    /// <param name="expression">A Constraint to be applied</param>
    /// <param name="actual">The actual value to test</param>
    static public void That<T>(ref T actual, IResolveConstraint expression)
    {
        Verify.That(ref actual, expression.Resolve(), null, null);
    }

    /// <summary>
    /// Apply a constraint to a referenced value, succeeding if the constraint
    /// is satisfied and throwing an assertion exception on failure.
    /// </summary>
    /// <param name="expression">A Constraint to be applied</param>
    /// <param name="actual">The actual value to test</param>
    /// <param name="message">The message that will be displayed on failure</param>
    static public void That<T>(ref T actual, IResolveConstraint expression, string message)
    {
        Verify.That(ref actual, expression.Resolve(), message, null);
    }

    /// <summary>
    /// Apply a constraint to a referenced value, succeeding if the constraint
    /// is satisfied and throwing an assertion exception on failure.
    /// </summary>
    /// <param name="expression">A Constraint to be applied</param>
    /// <param name="actual">The actual value to test</param>
    /// <param name="message">The message that will be displayed on failure</param>
    /// <param name="args">Arguments to be used in formatting the message</param>
    static public void That<T>(ref T actual, IResolveConstraint expression, string message, params object[] args)
    {
        Constraint constraint = expression.Resolve();

        if (!constraint.Matches(ref actual))
        {
            MessageWriter writer = new TextMessageWriter(message, args);
            constraint.WriteMessageTo(writer);
            Assert.Fail(writer.ToString());
        }
    }

    /// <summary>
    /// Asserts that a condition is true. If the condition is false the method throws
    /// an <see cref="AssertionException"/>.
    /// </summary> 
    /// <param name="condition">The evaluated condition</param>
    /// <param name="message">The message to display if the condition is false</param>
    /// <param name="args">Arguments to be used in formatting the message</param>
    static public void That(bool condition, string message, params object[] args)
    {
        Verify.That(condition, Is.True, message, args);
    }

    /// <summary>
    /// Asserts that a condition is true. If the condition is false the method throws
    /// an <see cref="AssertionException"/>.
    /// </summary>
    /// <param name="condition">The evaluated condition</param>
    /// <param name="message">The message to display if the condition is false</param>
    static public void That(bool condition, string message)
    {
        Verify.That(condition, Is.True, message, null);
    }

    /// <summary>
    /// Asserts that a condition is true. If the condition is false the method throws
    /// an <see cref="AssertionException"/>.
    /// </summary>
    /// <param name="condition">The evaluated condition</param>
    static public void That(bool condition)
    {
        Verify.That(condition, Is.True, null, null);
    }

    /// <summary>
    /// Asserts that the code represented by a delegate throws an exception
    /// that satisfies the constraint provided.
    /// </summary>
    /// <param name="code">A TestDelegate to be executed</param>
    /// <param name="constraint">A ThrowsConstraint used in the test</param>
    static public void That(TestDelegate code, IResolveConstraint constraint)
    {
        Verify.That((object)code, constraint);
    }

    /// <summary>
    /// Verifies that a delegate does not throw an exception
    /// </summary>
    /// <param name="code">A TestSnippet delegate</param>
    /// <param name="message">The message that will be displayed on failure</param>
    /// <param name="args">Arguments to be used in formatting the message</param>
    public static void DoesNotThrow(TestDelegate code, string message, params object[] args)
    {
        try
        {
            code();
        }
        catch (Exception ex)
        {
            TextMessageWriter writer = new TextMessageWriter(message, args);
            writer.WriteLine("Unexpected exception: {0}", ex.GetType());
            Assert.Fail(writer.ToString());
        }
    }

    /// <summary>
    /// Verifies that a delegate does not throw an exception.
    /// </summary>
    /// <param name="code">A TestSnippet delegate</param>
    /// <param name="message">The message that will be displayed on failure</param>
    public static void DoesNotThrow(TestDelegate code, string message)
    {
        DoesNotThrow(code, message, null);
    }

    /// <summary>
    /// Verifies that a delegate does not throw an exception.
    /// </summary>
    /// <param name="code">A TestSnippet delegate</param>
    public static void DoesNotThrow(TestDelegate code)
    {
        DoesNotThrow(code, string.Empty, null);
    }

}
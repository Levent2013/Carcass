using System;

namespace Carcass.Common.Utility
{
    /// <summary>
    /// Validation helper to check preconditions.
    /// </summary>
    public static class Throw
    {
        /// <summary>
        /// Verify argument is null
        /// </summary>
        /// <param name="arg">Argument to verify</param>
        /// <param name="argName">Argument name</param>
        public static void IfNullArgument(object arg, string argName)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(argName);
            }
        }

        /// <summary>Verify argument is null</summary>
        /// <param name="arg">Argument to verify</param>
        /// <param name="format">The format string.</param>
        /// <param name="args">Format string args</param>
        public static void IfNullArgument(object arg, string format, params object[] args)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(String.Format(format, args));
            }
        }

        /// <summary>
        /// Verify argument by custom check function
        /// </summary>
        /// <param name="argIsBad">Check function</param>
        public static void IfBadArgument(Func<bool> argIsBad)
        {
            if (argIsBad())
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Verify argument by custom check function
        /// </summary>
        /// <param name="argIsBad">Check function</param>
        /// <param name="message">Error message</param>
        public static void IfBadArgument(Func<bool> argIsBad, string message)
        {
            if (argIsBad())
            {
                throw new ArgumentException(message);
            }
        }

        /// <summary>
        /// Verify argument by custom check function
        /// </summary>
        /// <param name="argIsBad">Check function</param>
        /// <param name="format">The format string.</param>
        /// <param name="args">Format string args</param>
        public static void IfBadArgument(Func<bool> argIsBad, string format, params object[] args)
        {
            if (argIsBad())
            {
                throw new ArgumentException(String.Format(format, args));
            }
        }

        /// <summary>
        /// Throw <c>InvalidOperationException</c> by custom check function
        /// </summary>
        /// <param name="checkState">Check function</param>
        public static void IfInvalidOperation(Func<bool> checkState)
        {
            if (checkState())
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Throw <c>InvalidOperationException</c> by custom check function
        /// </summary>
        /// <param name="checkState">Check function</param>
        /// <param name="message">Error message</param>
        public static void IfInvalidOperation(Func<bool> checkState, string message)
        {
            if (checkState())
            {
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// Throw <c>InvalidOperationException</c> by custom check function
        /// </summary>
        /// <param name="checkState">Check function</param>
        /// <param name="format">The format string.</param>
        /// <param name="args">Format string args</param>
        public static void IfInvalidOperation(Func<bool> checkState, string format, params object[] args)
        {
            if (checkState())
            {
                throw new InvalidOperationException(String.Format(format, args));
            }
        }

        /// <summary>Throw <c>Exception</c> by custom check</summary>
        /// <param name="checkState">Check state</param>
        /// <param name="creator">The creator function</param>
        /// <param name="format">The format string.</param>
        /// <param name="args">Format string args</param>
        public static void IfTrue(bool checkState,  Func<string, Exception> creator, string format, params object[] args)
        {
            if (checkState)
            {
                throw creator(String.Format(format, args));
            }
        }
    }
}
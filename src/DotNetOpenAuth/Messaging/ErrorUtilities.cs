﻿//-----------------------------------------------------------------------
// <copyright file="ErrorUtilities.cs" company="Andrew Arnott">
//     Copyright (c) Andrew Arnott. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DotNetOpenAuth.Messaging {
	using System;
	using System.Diagnostics;
	using System.Globalization;

	/// <summary>
	/// A collection of error checking and reporting methods.
	/// </summary>
	internal class ErrorUtilities {
		/// <summary>
		/// Wraps an exception in a new <see cref="ProtocolException"/>.
		/// </summary>
		/// <param name="inner">The inner exception to wrap.</param>
		/// <param name="errorMessage">The error message for the outer exception.</param>
		/// <param name="args">The string formatting arguments, if any.</param>
		/// <returns>The newly constructed (unthrown) exception.</returns>
		internal static Exception Wrap(Exception inner, string errorMessage, params object[] args) {
			return new ProtocolException(string.Format(CultureInfo.CurrentCulture, errorMessage, args), inner);
		}

		/// <summary>
		/// Throws an internal error exception.
		/// </summary>
		/// <param name="errorMessage">The error message.</param>
		internal static void ThrowInternal(string errorMessage) {
			VerifyInternal(false, errorMessage);
		}

		/// <summary>
		/// Checks a condition and throws an internal error exception if it evaluates to false.
		/// </summary>
		/// <param name="condition">The condition to check.</param>
		/// <param name="errorMessage">The message to include in the exception, if created.</param>
		internal static void VerifyInternal(bool condition, string errorMessage) {
			if (!condition) {
				// Since internal errors are really bad, take this chance to
				// help the developer find the cause by breaking into the
				// debugger if one is attached.
				if (Debugger.IsAttached) {
					Debugger.Break();
				}

				throw new InternalErrorException(errorMessage);
			}
		}

		/// <summary>
		/// Checks a condition and throws an internal error exception if it evaluates to false.
		/// </summary>
		/// <param name="condition">The condition to check.</param>
		/// <param name="errorMessage">The message to include in the exception, if created.</param>
		/// <param name="args">The formatting arguments.</param>
		internal static void VerifyInternal(bool condition, string errorMessage, params object[] args) {
			if (!condition) {
				errorMessage = string.Format(CultureInfo.CurrentCulture, errorMessage, args);
				throw new InternalErrorException(errorMessage);
			}
		}

		/// <summary>
		/// Checks a condition and throws an <see cref="InvalidOperationException"/> if it evaluates to false.
		/// </summary>
		/// <param name="condition">The condition to check.</param>
		/// <param name="errorMessage">The message to include in the exception, if created.</param>
		internal static void VerifyOperation(bool condition, string errorMessage) {
			if (!condition) {
				throw new InvalidOperationException(errorMessage);
			}
		}

		/// <summary>
		/// Checks a condition and throws an <see cref="InvalidOperationException"/> if it evaluates to false.
		/// </summary>
		/// <param name="condition">The condition to check.</param>
		/// <param name="errorMessage">The message to include in the exception, if created.</param>
		/// <param name="args">The formatting arguments.</param>
		internal static void VerifyOperation(bool condition, string errorMessage, params object[] args) {
			if (!condition) {
				errorMessage = string.Format(CultureInfo.CurrentCulture, errorMessage, args);
				throw new InvalidOperationException(errorMessage);
			}
		}

		/// <summary>
		/// Throws a <see cref="ProtocolException"/> if some <paramref name="condition"/> evaluates to false.
		/// </summary>
		/// <param name="condition">True to do nothing; false to throw the exception.</param>
		/// <param name="faultedMessage">The message being processed that would be responsible for the exception if thrown.</param>
		/// <param name="errorMessage">The error message for the exception.</param>
		/// <param name="args">The string formatting arguments, if any.</param>
		internal static void VerifyProtocol(bool condition, IProtocolMessage faultedMessage, string errorMessage, params object[] args) {
			if (!condition) {
				throw new ProtocolException(string.Format(CultureInfo.CurrentCulture, errorMessage, args), faultedMessage);
			}
		}

		/// <summary>
		/// Throws a <see cref="ProtocolException"/> if some <paramref name="condition"/> evaluates to false.
		/// </summary>
		/// <param name="condition">True to do nothing; false to throw the exception.</param>
		/// <param name="message">The error message for the exception.</param>
		/// <param name="args">The string formatting arguments, if any.</param>
		internal static void VerifyProtocol(bool condition, string message, params object[] args) {
			if (!condition) {
				throw new ProtocolException(string.Format(
					CultureInfo.CurrentCulture,
					message,
					args));
			}
		}

		/// <summary>
		/// Throws a <see cref="ProtocolException"/>.
		/// </summary>
		/// <param name="message">The message to set in the exception.</param>
		/// <param name="args">The formatting arguments of the message.</param>
		/// <returns>
		/// An InternalErrorException, which may be "thrown" by the caller in order
		/// to satisfy C# rules to show that code will never be reached, but no value
		/// actually is ever returned because this method guarantees to throw.
		/// </returns>
		internal static Exception ThrowProtocol(string message, params object[] args) {
			VerifyProtocol(false, message, args);

			// we never reach here, but this allows callers to "throw" this method.
			return new InternalErrorException();
		}

		/// <summary>
		/// Verifies something about the argument supplied to a method.
		/// </summary>
		/// <param name="condition">The condition that must evaluate to true to avoid an exception.</param>
		/// <param name="message">The message to use in the exception if the condition is false.</param>
		/// <param name="args">The string formatting arguments, if any.</param>
		internal static void VerifyArgument(bool condition, string message, params object[] args) {
			if (!condition) {
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, message, args));
			}
		}

		/// <summary>
		/// Verifies that some given value is not null.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <param name="paramName">Name of the parameter, which will be used in the <see cref="ArgumentException"/>, if thrown.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
		internal static void VerifyArgumentNotNull(object value, string paramName) {
			if (Object.ReferenceEquals(value, null)) {
				throw new ArgumentNullException(paramName);
			}
		}

		/// <summary>
		/// Verifies that some string is not null and has non-zero length.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <param name="paramName">Name of the parameter, which will be used in the <see cref="ArgumentException"/>, if thrown.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="value"/> has zero length.</exception>
		internal static void VerifyNonZeroLength(string value, string paramName) {
			VerifyArgumentNotNull(value, paramName);
			if (value.Length == 0) {
				throw new ArgumentException(MessagingStrings.UnexpectedEmptyString, paramName);
			}
		}
	}
}
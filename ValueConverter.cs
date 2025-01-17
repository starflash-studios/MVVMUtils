﻿#region Copyright (C) 2017-2022  Cody Bock
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Data;

// ReSharper disable RedundantNullableFlowAttribute

#endregion

namespace MVVMUtils;

/// <summary>
/// A typed value converter with possible support for forwards, forwards (<see langword="null"/>), reverse, and reverse (<see langword="null"/>) conversions.
/// </summary>
/// <typeparam name="TFrom">The value to convert data from.</typeparam>
/// <typeparam name="TTo">The value to convert data into.</typeparam>
public abstract class ValueConverter<TFrom, TTo> : Reactive, IValueConverter {

	/// <summary>
	/// Indicates whether the <see cref="ValueConverter{TFrom, TTo}"/> supports forwards conversions (from type <typeparamref name="TFrom"/> to type <typeparamref name="TTo"/>).
	/// </summary>
	public virtual bool CanForward => true;

	/// <summary>
	/// Indicates whether the <see cref="ValueConverter{TFrom, TTo}"/> supports nullified forwards conversions (from <see langword="null"/> to type <typeparamref name="TTo"/>).
	/// </summary>
	public virtual bool CanForwardWhenNull => false;

	/// <summary>
	/// Indicates whether the <see cref="ValueConverter{TFrom, TTo}"/> supports reverse conversions (from type <typeparamref name="TTo"/> to type <typeparamref name="TFrom"/>).
	/// </summary>
	public abstract bool CanReverse { get; }

	/// <summary>
	/// Indicates whether the <see cref="ValueConverter{TFrom, TTo}"/> supports nullified reverse conversions (from <see langword="null"/> to type <typeparamref name="TFrom"/>).
	/// </summary>
	public virtual bool CanReverseWhenNull => false;

	/// <summary>
	/// Converts the given <paramref name="From"/> value to a new value of type <typeparamref name="TTo"/> or <see langword="null"/>.
	/// </summary>
	/// <param name="From">The original value to convert.</param>
	/// <param name="Parameter">Additional conversion parameters (may be <see langword="null"/>).</param>
	/// <param name="Culture">The current conversion culture.</param>
	/// <returns>A converted value of type <typeparamref name="TTo"/>, or <see langword="null"/>.</returns>
	[return: MaybeNull]
	public abstract TTo? Forward( [DisallowNull] TFrom From, object? Parameter = null, CultureInfo? Culture = null );

	/// <summary>
	/// Converts the a <see langword="null"/> 'from value' to a new value of type <typeparamref name="TTo"/> or <see langword="null"/>.
	/// </summary>
	/// <param name="Parameter">Additional conversion parameters (may be <see langword="null"/>).</param>
	/// <param name="Culture">The current conversion culture.</param>
	/// <returns>A new value of type <typeparamref name="TTo"/>, or <see langword="null"/>.</returns>
	[return: MaybeNull]
	public virtual TTo? ForwardWhenNull( object? Parameter = null, CultureInfo? Culture = null ) => default;

	/// <summary>
	/// Converts the given <paramref name="To"/> value to a new value of type <typeparamref name="TFrom"/> or <see langword="null"/>.
	/// </summary>
	/// <param name="To">The value to convert in the reverse direction.</param>
	/// <param name="Parameter">Additional conversion parameters (may be <see langword="null"/>).</param>
	/// <param name="Culture">The current conversion culture.</param>
	/// <returns>A converted value of type <typeparamref name="TFrom"/>, or <see langword="null"/>.</returns>
	[return: MaybeNull]
	public abstract TFrom? Reverse( [DisallowNull] TTo To, object? Parameter = null, CultureInfo? Culture = null );

	/// <summary>
	/// Converts the a <see langword="null"/> 'to value' to a new value of type <typeparamref name="TFrom"/> or <see langword="null"/>.
	/// </summary>
	/// <param name="Parameter">Additional conversion parameters (may be <see langword="null"/>).</param>
	/// <param name="Culture">The current conversion culture.</param>
	/// <returns>A new value of type <typeparamref name="TFrom"/>, or <see langword="null"/>.</returns>
	[return: MaybeNull]
	public virtual TFrom? ReverseWhenNull( object? Parameter = null, CultureInfo? Culture = null ) => default;

	/// <inheritdoc />
	public virtual object? Convert( object? Value, Type TargetType, object Parameter, CultureInfo Culture ) {
		switch ( Value ) {
			case TFrom TF:
				if ( CanForward ) {
					return Forward(TF, Parameter, Culture);
				}

				throw new NotSupportedException($"ValueConverter of type {GetType().Name} does not support forward conversions from type {nameof(TFrom)} to type {nameof(TTo)}.");
			case null:
				if ( CanForwardWhenNull ) {
					return ForwardWhenNull(Parameter, Culture);
				}

				throw new NotSupportedException($"ValueConverter of type {GetType().Name} does not support nullified forward conversions from type {nameof(TFrom)} (null) to type {nameof(TTo)}.");
			default:
				throw new NotSupportedException($"Value '{Value}' ({Value.GetType().Name}) is not of expected type for ValueConverter of type {GetType().Name} (Must be {typeof(TFrom)} or {typeof(TTo)}).");
		}
	}

	/// <inheritdoc />
	public object? ConvertBack( object? Value, Type TargetType, object Parameter, CultureInfo Culture ) {
		switch ( Value ) {
			case TTo TT:
				if ( CanReverse ) {
					return Reverse(TT, Parameter, Culture);
				}

				throw new NotSupportedException($"ValueConverter of type {GetType().Name} does not support backwards conversions from type {nameof(TTo)} to type {nameof(TFrom)}.");
			case null:
				if ( CanReverseWhenNull ) {
					return ReverseWhenNull(Parameter, Culture);
				}

				throw new NotSupportedException($"ValueConverter of type {GetType().Name} does not support nullified backwards conversions from type {nameof(TTo)} (null) to type {nameof(TFrom)}.");
			default:
				throw new NotSupportedException($"Value '{Value}' ({Value.GetType().Name}) is not of expected type for ValueConverter of type {GetType().Name} (Must be {typeof(TFrom)} or {typeof(TTo)}).");
		}
	}
}
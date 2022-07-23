// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    ///     DisplayAttribute is a general-purpose attribute to specify user-visible globalizable strings for types and members.
    ///     The string properties of this class can be used either as literals or as resource identifiers into a specified
    ///     <see cref="ResourceType" />
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DisplayExAttribute : Attribute
    {
        public string? PluralName
        {
            get
            {
                string returnValue = string.Empty;
                if (this.ResourceType.GetRuntimeProperty(nameof(PluralName)) is { } pluralName && pluralName.GetValue(null, null) is {} value) return value.ToString();
                if (string.IsNullOrEmpty(returnValue) && this.ResourceType.GetRuntimeProperty(nameof(DisplayAttribute.Name)) is { } nameProperty && nameProperty.GetValue(null, null) is {} singleValue) return singleValue.ToString();
                return string.Empty;
            }
        }

        public Type ResourceType { get; set; }
    }
}

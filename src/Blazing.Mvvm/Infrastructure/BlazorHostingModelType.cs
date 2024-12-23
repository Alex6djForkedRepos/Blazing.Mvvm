﻿namespace Blazing.Mvvm;

/// <summary>
/// The ASP.NET Core Blazor hosing models
/// </summary>
public enum BlazorHostingModelType
{
    /// <summary>
    /// No hosting model is specified
    /// </summary>
    NotSpecified,

    /// <summary>
    /// Blazor Server
    /// </summary>
    Server,

    /// <summary>
    /// Blazor WebAssembly
    /// </summary>
    WebAssembly,

    /// <summary>
    /// Blazor Hybrid
    /// </summary>
    Hybrid,

    /// <summary>
    /// Blazor Hybrid with .NET MAUI
    /// </summary>
    HybridMaui,

    /// <summary>
    /// Interactive WebApp
    /// </summary>
    WebApp
}

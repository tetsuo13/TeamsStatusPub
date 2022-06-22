﻿namespace TeamsStatusPub.Services;

/// <summary>
/// Application info.
/// </summary>
public interface IAppInfo
{
    /// <summary>
    /// Gets the name of the application.
    /// </summary>
    string ApplicationName { get; }

    /// <summary>
    /// Gets the absolute file path to the application binary.
    /// </summary>
    string ApplicationPath { get; }

    /// <summary>
    /// Gets the copyright notice.
    /// </summary>
    string Copyright { get; }

    /// <summary>
    /// Gets the version string.
    /// </summary>
    string Version { get; }

    /// <summary>
    /// Gets the application web site.
    /// </summary>
    string WebsiteUrl { get; }
}

﻿using System.Text;

namespace ComputingProject.Client.Services;

public static class RandomIDGenerator
{
    /// <summary>
    /// Generates a random ID 11 characters long.
    /// Code from stack overflow: https://stackoverflow.com/a/44960751
    /// </summary>
    /// <returns>Random string</returns>
    public static string GenerateRandomID()
    {
        StringBuilder builder = new StringBuilder();
        Enumerable
            .Range(65, 26)
            .Select(e => ((char)e).ToString())
            .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
            .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
            .OrderBy(e => Guid.NewGuid())
            .Take(11)
            .ToList().ForEach(e => builder.Append(e));
        string id = builder.ToString();
        return id;
    }
}
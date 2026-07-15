using System;

namespace task07;

[AttributeUsage(AttributeTargets.Class)]
public class VersionAttribute : Attribute
{
    public int Major { get; }
    public int Minor { get; }

    public VersionAttribute(int majorVersion, int minorVersion)
    {
        Major = majorVersion;
        Minor = minorVersion;
    }
}
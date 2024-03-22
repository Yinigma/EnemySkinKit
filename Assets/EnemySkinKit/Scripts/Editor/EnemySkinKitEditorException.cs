using System;

namespace AntlerShed.EnemySkinKit
{
    internal class InvalidPluginNameException : Exception 
    {
        internal InvalidPluginNameException() : base("The given mod name created an invalid plugin name. The mod name must contain at least one alphabetical character."){}
    }

    internal class InvalidAssemblyNameException : Exception
    {
        internal InvalidAssemblyNameException() : base("The given mod name is an invalid assembly name. The mod name must have at least one alphabetical character.") { }
    }

    internal class InvalidNamespaceException : Exception
    {
        internal InvalidNamespaceException() : base("The given mod name and author created an invalid namespace. The mod name and author must each contain at least one aphabetical character in order to generate a valid namespace identifier.") { }
    }

    internal class InvalidModNameException : Exception
    {
        internal InvalidModNameException() : base("Given mod name was empty or only contained whitespace.") { }
    }

    internal class InvalidAuthorException : Exception
    {
        internal InvalidAuthorException() : base("Given author was empty or only contained whitespace.") { }
    }

    internal class InvalidModGUIDException : Exception
    {
        internal InvalidModGUIDException() : base("Mod GUID and either the author or the mod name were not specified or only contain whitespace.") { }
    }

    internal class InvalidIconException : Exception
    {
        internal InvalidIconException() : base("Icon must be a png file of size 256 x 256.") { }
    }
}

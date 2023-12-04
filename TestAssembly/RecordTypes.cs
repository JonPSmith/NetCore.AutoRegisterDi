// Copyright (c) 2023 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace TestAssembly;

public record RecordNoInterface(string FirstName, string LastName)
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

public record RecordWithInterface(string FirstName, string LastName) : IRecordWithInterface
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

public interface IRecordWithInterface
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
// Copyright (c) 2018 Inventory Innovations, Inc. - build by Jon P Smith (GitHub JonPSmith)
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Runtime.InteropServices;

namespace TestAssembly
{
    public class MyService : IMyService, ClassWithNestedService.INestedMyService
    {
        public string IntToString(int num)
        {
            return num.ToString();
        }
    }  
}

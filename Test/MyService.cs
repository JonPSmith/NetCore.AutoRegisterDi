// Copyright (c) 2018 Inventory Innovations, Inc. - build by Jon P Smith (GitHub JonPSmith)
// Licensed under MIT licence. See License.txt in the project root for license information.

namespace Test
{
    public class MyService : IMyService, TestAutoRegisterDi.INestedMyService
    {
        public int MyInt { get; set; }
    }  
}

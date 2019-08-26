// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace TestAssembly
{
    public class ClassWithNestedService
    {
        public interface INestedMyService
        {
            string IntToString(int num);
        }

        public class NestedMyService : INestedMyService
        {
            public string IntToString(int num)
            {
                return num.ToString();
            }
        }

    }
}
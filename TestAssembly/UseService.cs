// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

namespace TestAssembly
{
    public class UseService
    {
        private readonly IMyService _service;

        public UseService(IMyService service)
        {
            _service = service;
        }

        public string CallService()
        {
            int i = 1; //... some calculation
            return _service.IntToString(i);
        }
    }
}
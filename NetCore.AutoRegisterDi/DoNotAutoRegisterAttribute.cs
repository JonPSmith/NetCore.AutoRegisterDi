// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.
// Code added/updated by Fedor Zhekov, GitHub: @ZFi88

using System;

namespace NetCore.AutoRegisterDi
{
    /// <summary>
    /// Attribute for marking classes which no need to register in container 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DoNotAutoRegisterAttribute : Attribute
    {}
}
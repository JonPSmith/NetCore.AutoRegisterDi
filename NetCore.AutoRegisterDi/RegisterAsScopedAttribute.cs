// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.
// Code added/updated by Fedor Zhekov, GitHub: @ZFi88

using System;
using Microsoft.Extensions.DependencyInjection;
using NetCore.AutoRegisterDi.PublicButHidden;

namespace NetCore.AutoRegisterDi
{
    /// <summary>
    /// Attribute for marking classes which need to register with Scope lifetime
    /// </summary>
    public class RegisterAsScopedAttribute : RegisterWithLifetimeAttribute
    {

        /// <summary>
        /// ctor to set the ServiceLifetime
        /// </summary>
        public RegisterAsScopedAttribute() : base(ServiceLifetime.Scoped)
        { }
    }
}
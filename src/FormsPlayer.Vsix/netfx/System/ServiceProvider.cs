#region BSD License
/* 
Copyright (c) 2010, NETFx
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.

* Neither the name of Clarius Consulting nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion
namespace System
{
    using System.Globalization;

    /// <summary>
    /// Defines extension methods related to <see cref="IServiceProvider"/>.
    /// </summary>
    static partial class ServiceProviderExtensions
    {
        /// <summary>
        /// Gets type-based services from the  service provider.
        /// </summary>
        /// <nuget id="netfx-System.ServiceProvider" />
        /// <typeparam name="T">The type of the service to get.</typeparam>
        /// <param name="provider" this="true">The service provider.</param>
        /// <returns>The requested service, or a <see langword="null"/> reference if the service could not be located.</returns>
        public static T TryGetService<T>(this IServiceProvider provider)
        {
            return (T)provider.GetService(typeof(T));
        }

        /// <summary>
        /// Gets type-based services from the  service provider.
        /// </summary>
        /// <nuget id="netfx-System.ServiceProvider" />
        /// <typeparam name="T">The type of the service to get.</typeparam>
        /// <param name="provider" this="true">The service provider.</param>
        /// <returns>The requested service, or throws an <see cref="InvalidOperationException"/> 
        /// if the service was not found.</returns>
        public static T GetService<T>(this IServiceProvider provider)
        {
            var service = (T)provider.GetService(typeof(T));

            if (service == null)
                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture,
                    "Required service '{0}' not found.",
                    typeof(T)));

            return service;
        }

        /// <summary>
        /// Gets type-based services from the service provider.
        /// </summary>
        /// <nuget id="netfx-System.ServiceProvider" />
        /// <typeparam name="TRegistration">The type of the registration of the service.</typeparam>
        /// <typeparam name="TService">The type of the service to get.</typeparam>
        /// <param name="provider" this="true">The service provider.</param>
        /// <returns>The requested service, or a <see langword="null"/> reference if the service could not be located.</returns>
        public static TService TryGetService<TRegistration, TService>(this IServiceProvider provider)
        {
            return (TService)provider.GetService(typeof(TRegistration));
        }

        /// <summary>
        /// Gets type-based services from the service provider.
        /// </summary>
        /// <nuget id="netfx-System.ServiceProvider" />
        /// <typeparam name="TRegistration">The type of the registration of the service.</typeparam>
        /// <typeparam name="TService">The type of the service to get.</typeparam>
        /// <param name="provider" this="true">The service provider.</param>
        /// <returns>The requested service, or throws an <see cref="InvalidOperationException"/> 
        /// if the service was not found.</returns>
        public static TService GetService<TRegistration, TService>(this IServiceProvider provider)
        {
            var service = (TService)provider.GetService(typeof(TRegistration));

            if (service == null)
                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture,
                    "Required service '{0}' not found.",
                    typeof(TRegistration)));

            return service;
        }
    }
}
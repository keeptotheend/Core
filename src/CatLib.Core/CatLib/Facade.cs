﻿/*
 * This file is part of the CatLib package.
 *
 * (c) CatLib <support@catlib.io>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 *
 * Document: https://catlib.io/
 */

#pragma warning disable CA1000
#pragma warning disable S2743
#pragma warning disable S1118

using CatLib.Container;

namespace CatLib
{
    /// <summary>
    /// <see cref="Facade{TService}"/> is the abstract implemented by all facade classes.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <remarks>
    /// <code>public class FileSystem : Facade&gt;IFileSystem&lt;{ }</code>
    /// </remarks>
    public abstract class Facade<TService>
    {
        private static readonly string Service;
        private static TService that;
        private static IBindData binder;
        private static bool inited;
        private static bool released;

        /// <summary>
        /// Initializes static members of the <see cref="Facade{TService}"/> class.
        /// </summary>
#pragma warning disable S3963
        static Facade()
#pragma warning restore S3963
        {
            Service = App.Type2Service(typeof(TService));
            App.OnNewApplication += app =>
            {
                that = default;
                binder = null;
                inited = false;
                released = false;
            };
        }

        /// <inheritdoc cref="that"/>
        public static TService That => HasInstance ? that : Resolve();

        /// <summary>
        /// Gets a value indicating whether the resolved instance is exists in the facade.
        /// <para>If it is a non-static binding then return forever false.</para>
        /// </summary>
        internal static bool HasInstance => binder != null && binder.IsStatic && !released && that != null;

        /// <summary>
        /// Resolve the object instance.
        /// </summary>
        /// <param name="userParams">The user parameters.</param>
        /// <returns>The resolved object.</returns>
        internal static TService Make(params object[] userParams)
        {
            return HasInstance ? that : Resolve(userParams);
        }

        /// <inheritdoc cref="Make"/>
        private static TService Resolve(params object[] userParams)
        {
            released = false;

            if (!inited && (App.IsResolved(Service) || App.CanMake(Service)))
            {
                App.Watch<TService>(ServiceRebound);
                inited = true;
            }
            else if (binder != null && !binder.IsStatic)
            {
                // If it has been initialized, the binder has been initialized.
                // Then judging in advance can optimize performance without
                // going through a hash lookup.
                return Build(userParams);
            }

            var newBinder = App.GetBind(Service);
            if (newBinder == null || !newBinder.IsStatic)
            {
                binder = newBinder;
                return Build(userParams);
            }

            Rebind(newBinder);
            return that = Build(userParams);
        }

        /// <summary>
        /// When the resolved object is released.
        /// </summary>
        /// <param name="oldBinder">The old bind data with resolved object.</param>
        /// <param name="instance">The ignored parameter.</param>
        private static void OnRelease(IBindData oldBinder, object instance)
        {
            if (oldBinder != binder)
            {
                return;
            }

            that = default;
            released = true;
        }

        /// <summary>
        /// When the resolved object is rebound.
        /// </summary>
        /// <param name="newService">The new resolved object.</param>
        private static void ServiceRebound(TService newService)
        {
            var newBinder = App.GetBind(Service);
            Rebind(newBinder);
            that = (newBinder == null || !newBinder.IsStatic) ? default : newService;
        }

        /// <summary>
        /// Rebinding the bound data to given binder.
        /// </summary>
        /// <param name="newBinder">The new binder.</param>
        private static void Rebind(IBindData newBinder)
        {
            if (newBinder != null && binder != newBinder && newBinder.IsStatic)
            {
                newBinder.OnRelease(OnRelease);
            }

            binder = newBinder;
        }

        /// <summary>
        /// Resolve facade object from the container.
        /// </summary>
        /// <param name="userParams">The user parameters.</param>
        /// <returns>The resolved object.</returns>
        private static TService Build(params object[] userParams)
        {
            return (TService)App.Make(Service, userParams);
        }
    }
}

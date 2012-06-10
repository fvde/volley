using System;
using System.Collections.Generic;

namespace Torrero
{
    /// <summary>
    /// Implementiert IServiceProvider für die Anwendung. Dieser Typ wird durch die Eigenschaft App.Services
    /// ausgesetzt und kann für ContentManagers oder andere Typen verwendet werden, die auf einen IServiceProvider zugreifen müssen.
    /// </summary>
    public class AppServiceProvider : IServiceProvider
    {
        // Eine Zuordnung des Diensttyps zu den Diensten selbst
        private readonly Dictionary<Type, object> services = new Dictionary<Type, object>();

        /// <summary>
        /// Fügt einen neuen Dienst zum Dienstanbieter hinzu.
        /// </summary>
        /// <param name="serviceType">Der Typ des hinzuzufügenden Dienstes.</param>
        /// <param name="service">Das Dienstobjekt selbst.</param>
        public void AddService(Type serviceType, object service)
        {
            // Eingabe bestätigen
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");
            if (service == null)
                throw new ArgumentNullException("service");
            if (!serviceType.IsAssignableFrom(service.GetType()))
                throw new ArgumentException("service does not match the specified serviceType");

            // Dienst zum Wörterbuch hinzufügen
            services.Add(serviceType, service);
        }

        /// <summary>
        /// Holt einen Dienst vom Dienstanbieter.
        /// </summary>
        /// <param name="serviceType">Der Typ des abzurufenden Dienstes.</param>
        /// <returns>Das Dienstobjekt, das für den angegebenen Typ registriert ist.</returns>
        public object GetService(Type serviceType)
        {
            // Eingabe bestätigen
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            // Dienst vom Wörterbuch abrufen
            return services[serviceType];
        }

        /// <summary>
        /// Entfernt einen Dienst vom Dienstanbieter.
        /// </summary>
        /// <param name="serviceType">Der Typ des zu entfernenden Dienstes.</param>
        public void RemoveService(Type serviceType)
        {
            // Eingabe bestätigen
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            // Dienst aus dem Wörterbuch entfernen
            services.Remove(serviceType);
        }
    }
}

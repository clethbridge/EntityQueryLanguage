using EntityQueryLanguage.Components.Models;
using System;

namespace EntityQueryLanguage.Components.Services.Attributes
{
    public class EqlServiceAttribute: Attribute
    {
        /// <summary>
        /// The Type of interface that should be used within the DI Containerd.
        /// </summary>
        public Type Abstraction { get; set; }


        /// <summary>
        /// The lifetime of the service (e.g Singleton, Scoped, Transient)
        /// </summary>
        public EqlServiceType EqlServiceType { get; set; } 

        public EqlServiceAttribute(Type abstraction, EqlServiceType eqlServiceType = EqlServiceType.Scoped)
        {
            Abstraction = abstraction;
            EqlServiceType = eqlServiceType;
        }
    }
}

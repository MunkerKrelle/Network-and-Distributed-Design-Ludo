using ComponentPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network_Ludo.BuilderPattern
{
    internal class Director
    {
        private IBuilder builder;

        /// <summary>
        /// Konstruktør til at initialisere directoren med en builder.
        /// </summary>
        /// <param name="builder">Den builder, der skal bruges til at konstruere GameObject.</param>
        public Director(IBuilder builder)
        {
            this.builder = builder;
        }

        /// <summary>
        /// Metode til at konstruere et GameObject ved hjælp af den angivne builder.
        /// </summary>
        /// <returns>Det færdigbyggede GameObject.</returns>
        public GameObject Construct()
        {
            builder.BuildGameObject();

            return builder.GetResult();
        }
    }
}

using ComponentPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network_Ludo.BuilderPattern
{
    internal interface IBuilder
    {
        /// <summary>
        /// Metode til at bygge et GameObject med dets komponenter.
        /// </summary>
        void BuildGameObject();

        /// <summary>
        /// Metode til at få det færdigbyggede GameObject.
        /// </summary>
        /// <returns>Det færdigbyggede GameObject.</returns>
        GameObject GetResult();
    }
}

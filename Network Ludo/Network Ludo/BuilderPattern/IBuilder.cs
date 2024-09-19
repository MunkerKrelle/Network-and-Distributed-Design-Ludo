using ComponentPattern;

namespace BuilderPattern
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

using ComponentPattern;

namespace FactoryPattern
{
    /// <summary>
    /// Factory interface
    /// </summary>
    public abstract class Factory
    {
        public abstract GameObject Create();
    }
}

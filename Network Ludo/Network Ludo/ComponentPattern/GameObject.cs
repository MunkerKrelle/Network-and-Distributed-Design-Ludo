using myClientTCP;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ComponentPattern
{
    /// <summary>
    /// GameObject er en samling af components så de sammen danner et GameObject hvor alting kan blive kørt for sig
    /// </summary>
    public class GameObject : ICloneable
    {
        private List<Component> components = new List<Component>();

        public Transform Transform { get; private set; } = new Transform();
        public bool IsActive { get; set; }

        public string Tag { get; set; }

        /// <summary>
        /// Tilføje et component til et GameObject - spriteRender, Collider etc. 
        /// </summary>
        /// <typeparam name="T">Hvilken type component der skal tilføjes</typeparam>
        /// <param name="additionalParameters">Hvilke ekstra parameter der skal med</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public T AddComponent<T>(params object[] additionalParameters) where T : Component
        {
            Type componentType = typeof(T);
            try
            {
                // Opret en instans ved hjælp af den fundne konstruktør og leverede parametre
                object[] allParameters = new object[1 + additionalParameters.Length];
                allParameters[0] = this;
                Array.Copy(additionalParameters, 0, allParameters, 1, additionalParameters.Length);

                T component = (T)Activator.CreateInstance(componentType, allParameters);
                components.Add(component);
                return component;
            }
            catch (Exception e)
            {
                // Håndter tilfælde, hvor der ikke er en passende konstruktør
                throw new InvalidOperationException($"Klassen {componentType.Name} har ikke en " +
                    $"konstruktør, der matcher de leverede parametre. Exection {e}");

            }
        }

        /// <summary>
        /// Tilføj et component der allerde eksistere 
        /// </summary>
        /// <param name="component">Hvilket component der skal bruges</param>
        /// <returns></returns>
        public Component AddComponentWithExistingValues(Component component)
        {
            components.Add(component);
            return component;
        }

        /// <summary>
        /// Find et specifikt component fra et GameObject
        /// </summary>
        /// <typeparam name="T">Hvilken slags component den skal prøve at finde</typeparam>
        /// <returns></returns>
        public Component GetComponent<T>() where T : Component
        {
            return components.Find(x => x.GetType() == typeof(T));
        }

        public void Awake()
        {
            for (int i = 0; i < components.Count; i++)
            {
                components[i].Awake();
            }
        }

        public void Start()
        {
            for (int i = 0; i < components.Count; i++)
            {
                components[i].Start();
            }
        }



        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < components.Count; i++)
            {
                components[i].Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < components.Count; i++)
            {
                components[i].Draw(spriteBatch);
            }
        }

        public object Clone()
        {
            GameObject go = new GameObject();
            foreach (Component component in components)
            {
                Component newComponent = go.AddComponentWithExistingValues(component.Clone() as Component);
                newComponent.SetNewGameObject(go);
            }
            return go;

        }
    }
}

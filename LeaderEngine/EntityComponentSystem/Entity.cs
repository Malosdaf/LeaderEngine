﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace LeaderEngine
{
    public class Entity
    {
        public string Name;
        public readonly Transform Transform;

        private Entity _parent;
        public Entity Parent
        {
            get => _parent;
            set
            {
                //remove from old parent
                _parent?.Children.Remove(this);
                //add to new parent
                _parent = value;
                _parent?.Children.Add(this);

                if (value == null)
                    DataManager.CurrentScene.SceneRootEntities.Add(this);
                else
                    DataManager.CurrentScene.SceneRootEntities.Remove(this);
            }
        }

        public bool Active = true;

        internal List<Entity> Children { get; } = new List<Entity>();
        private List<Component> components { get; } = new List<Component>();

        internal List<Renderer> Renderers { get; } = new List<Renderer>();

        public Entity(string name)
        {
            Name = name;
            Transform = new Transform(this);

            DataManager.CurrentScene.SceneRootEntities.Add(this);
        }

        internal void RecursivelyUpdate()
        {
            if (!Active)
                return;

            components.ForEach(x => { if (x.Enabled) x.UpdateMethod?.Invoke(); });

            Children.ForEach(child => child.RecursivelyUpdate());
        }

        internal void RecursivelyRender()
        {
            if (!Active)
                return;

            Renderers.ForEach(x => x.Render());

            Children.ForEach(child => child.RecursivelyRender());
        }

        public void Destroy()
        {
            _parent?.Children.Remove(this);
            DataManager.CurrentScene.SceneRootEntities.Remove(this);

            while (Children.Count > 0)
                Children[0].Destroy();
        }

        #region ComponentGettersSetters
        public T GetComponent<T>() where T : Component
        {
            return (T)components.Find(c => typeof(T).IsAssignableFrom(c.GetType()));
        }
        public T[] GetComponents<T>() where T : Component
        {
            return components.FindAll(c => typeof(T).IsAssignableFrom(c.GetType())).Select(x => (T)x).ToArray();
        }
        public void AddComponent(Component component) //basic
        {
            components.Add(component);
            component.BaseEntity = this;
            component.StartMethod?.Invoke();
        }
        public T AddComponent<T>(params object[] args) where T : Component
        {
            T comp = (T)Activator.CreateInstance(typeof(T), args);
            AddComponent(comp);
            return comp;
        }
        private void RemoveComponentAt(int index) //basic
        {
            components[index].RemoveMethod?.Invoke();

            components.RemoveAt(index);
        }
        public void RemoveComponent(Component component)
        {
            RemoveComponentAt(components.IndexOf(component));
        }
        public void RemoveComponent<T>() where T : Component
        {
            RemoveComponentAt(components.FindIndex(c => typeof(T).IsAssignableFrom(c.GetType())));
        }
        #endregion
    }
}

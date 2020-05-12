using System;
using System.Collections.Generic;

namespace RasofiaGames.SimplyECS
{
	public class Entity
	{
		public delegate void ComponentHandler(Entity entity, EntityComponent componentInstance);

		public event Action<Entity> DestroyEvent;
		public event ComponentHandler AddedComponentEvent;
		public event ComponentHandler RemovedComponentEvent;

		private HashSet<EntityComponent> _components = new HashSet<EntityComponent>();
		private List<EntityComponent> _removingComponents = new List<EntityComponent>();

		public bool IsDestroyed
		{
			get; private set;
		}

		public Entity()
		{
			EntityTracker.Instance.Register(this);
		}

		public void Destroy()
		{
			if(IsDestroyed)
			{
				return;
			}

			IsDestroyed = true;

			DestroyEvent?.Invoke(this);

			foreach(EntityComponent component in _components)
			{
				RemoveEntityComponent(component, false);
			}

			_removingComponents.Clear();
			_removingComponents = null;

			_components.Clear();
			_components = null;

			AddedComponentEvent = null;
			RemovedComponentEvent = null;
		}

		public T AddEntityComponent<T>() where T : EntityComponent
		{
			return AddEntityComponent(typeof(T)) as T;
		}

		public void RemoveEntityComponent<T>() where T : EntityComponent
		{
			RemoveEntityComponent(GetEntityComponent<T>());
		}

		public bool HasEntityComponent<T>() where T : EntityComponent
		{
			return GetEntityComponent<T>() != null;
		}

		public T GetEntityComponent<T>() where T : EntityComponent
		{
			return GetEntityComponent(typeof(T)) as T;
		}

		public bool TryGetEntityComponent<T>(out T component) where T : EntityComponent
		{
			component = GetEntityComponent<T>();
			return component != null;
		}

		private EntityComponent AddEntityComponent(Type type)
		{
			if(IsDestroyed)
			{
				UnityEngine.Debug.LogWarning($"Can't add component {type.Name} to a destroyed entity!");
				return null;
			}

			EntityComponent component = Activator.CreateInstance(type) as EntityComponent;

			if(component != null)
			{
				_components.Add(component);
				component.Initialize(this);
				AddedComponentEvent?.Invoke(this, component);
			}

			return component;
		}

		private EntityComponent GetEntityComponent(Type type)
		{
			foreach(EntityComponent component in _components)
			{
				if(type.IsAssignableFrom(component.GetType()))
				{
					return component;
				}
			}
			return null;
		}

		private void RemoveEntityComponent(EntityComponent component, bool clean = true)
		{
			if(component != null)
			{
				if(_components.Contains(component) && !_removingComponents.Contains(component))
				{
					_removingComponents.Add(component);
					RemovedComponentEvent?.Invoke(this, component);
					component.Deinitialize();
				}

				if(clean)
				{
					for(int i = _removingComponents.Count; i >= 0; i--)
					{
						_components.Remove(_removingComponents[i]);
					}
				}
			}
		}
	}
}
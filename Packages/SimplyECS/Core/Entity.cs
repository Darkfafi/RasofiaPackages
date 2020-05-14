using System;
using System.Collections.Generic;

namespace RasofiaGames.SimplyECS
{
	public class Entity
	{
		public delegate void ComponentHandler(Entity entity, EntityComponent componentInstance);
		public delegate void ComponentEnabledHandler(Entity entity, EntityComponent componentInstance);

		public event Action<Entity> DestroyEvent;

		public event ComponentHandler EnabledStateComponentChangedEvent;
		public event ComponentHandler AddedComponentEvent;
		public event ComponentHandler RemovedComponentEvent;

		private HashSet<EntityComponent> _components = new HashSet<EntityComponent>();
		private List<EntityComponent> _disabledComponents = new List<EntityComponent>();
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

			_disabledComponents.Clear();
			_disabledComponents = null;

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
			RemoveEntityComponent(GetEntityComponent<T>(false));
		}

		public void RemoveEntityComponent(EntityComponent entityComponent)
		{
			RemoveEntityComponent(entityComponent, true);
		}

		public bool HasEntityComponent<T>(bool isEnabled = true) where T : EntityComponent
		{
			return GetEntityComponent<T>(isEnabled) != null;
		}

		public T GetEntityComponent<T>(bool isEnabled = true) where T : EntityComponent
		{
			return GetEntityComponent(typeof(T), isEnabled) as T;
		}

		public bool TryGetEntityComponent<T>(out T component, bool isEnabled = true) where T : EntityComponent
		{
			component = GetEntityComponent<T>(isEnabled);
			return component != null;
		}

		internal bool IsEntityComponentEnabled(EntityComponent entityComponent)
		{
			return !_disabledComponents.Contains(entityComponent);
		}

		internal void SetEntityComponentEnabledState(EntityComponent entityComponent, bool enabled)
		{
			if(!_components.Contains(entityComponent) || (_removingComponents.Contains(entityComponent) && enabled))
			{
				return;
			}

			if(IsEntityComponentEnabled(entityComponent) != enabled)
			{
				if(enabled)
				{
					_disabledComponents.Remove(entityComponent);
					entityComponent.Enabled();
				}
				else
				{
					_disabledComponents.Add(entityComponent);
					entityComponent.Disabled();
				}
				EnabledStateComponentChangedEvent?.Invoke(this, entityComponent);
			}
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
				component.Enabled();
				AddedComponentEvent?.Invoke(this, component);
			}

			return component;
		}

		private EntityComponent GetEntityComponent(Type type, bool isEnabled)
		{
			foreach(EntityComponent component in _components)
			{
				if(type.IsAssignableFrom(component.GetType()) && 
					!_removingComponents.Contains(component) &&
					(!isEnabled || (isEnabled && !_disabledComponents.Contains(component))))
				{
					return component;
				}
			}
			return null;
		}

		private void RemoveEntityComponent(EntityComponent component, bool clean)
		{
			if(component != null)
			{
				if(_components.Contains(component) && !_removingComponents.Contains(component))
				{
					_removingComponents.Add(component);
					SetEntityComponentEnabledState(component, false);

					RemovedComponentEvent?.Invoke(this, component);

					component.Deinitialize();
				}

				if(clean)
				{
					for(int i = _removingComponents.Count - 1; i >= 0; i--)
					{
						_components.Remove(_removingComponents[i]);
						_disabledComponents.Remove(_removingComponents[i]);
					}
				}
			}
		}
	}
}
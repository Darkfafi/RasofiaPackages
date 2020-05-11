using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RasofiaGames.SimplyECS
{
	public class EntityTracker : MonoBehaviour
	{
		public delegate void EntityComponentHandler(Entity entity, EntityComponent component);

		public event Action<Entity> EntityTrackedEvent;
		public event Action<Entity> EntityUntrackedEvent;

		public event EntityComponentHandler EntityAddedComponentEvent;
		public event EntityComponentHandler EntityRemovedComponentEvent;

		public static EntityTracker Instance
		{
			get
			{
				if(_instance == null)
				{
					_instance = new GameObject("<EntityTracker>").AddComponent<EntityTracker>();
				}

				return _instance;
			}
		}

		public static bool IsAvailable => _instance != null;

		private static EntityTracker _instance;

		private List<Entity> _entities = new List<Entity>();
		private List<Entity> _entitiesToAdd = new List<Entity>();
		private Coroutine _registerRoutine = null;

		protected void OnDestroy()
		{
			Clean();
		}

		public Entity[] GetAll()
		{
			return _entities.ToArray();
		}

		public void ForEach(Action<Entity> action)
		{
			for(int i = _entities.Count - 1; i >= 0; i--)
			{
				action(_entities[i]);
			}
		}

		public void Clean()
		{
			for(int i = _entities.Count - 1; i >= 0; i--)
			{
				_entities[i].Destroy();
			}
		}

		internal void Register(Entity entity)
		{
			_entitiesToAdd.Add(entity);
			if(_registerRoutine == null)
			{
				_registerRoutine = StartCoroutine(WaitToRegisterEntity());
			}
		}

		internal void Unregister(Entity entity)
		{
			if(_entities.Remove(entity))
			{
				entity.DestroyEvent -= OnDestroyEvent;
				entity.AddedComponentEvent -= OnComponentAddedEvent;
				entity.RemovedComponentEvent -= OnComponentRemovedEvent;
				EntityUntrackedEvent?.Invoke(entity);
			}
		}

		private void OnDestroyEvent(Entity destroyedEntity)
		{
			Unregister(destroyedEntity);
		}

		private void OnComponentAddedEvent(Entity entity, EntityComponent component)
		{
			EntityAddedComponentEvent?.Invoke(entity, component);
		}

		private void OnComponentRemovedEvent(Entity entity, EntityComponent component)
		{
			EntityRemovedComponentEvent?.Invoke(entity, component);
		}

		private IEnumerator WaitToRegisterEntity()
		{
			yield return new WaitForEndOfFrame();
			for(int i = 0, c = _entitiesToAdd.Count; i < c; i++)
			{
				Entity entity = _entitiesToAdd[i];
				if(!entity.IsDestroyed && !_entities.Contains(entity))
				{
					_entities.Add(entity);
					entity.DestroyEvent += OnDestroyEvent;
					entity.AddedComponentEvent += OnComponentAddedEvent;
					entity.RemovedComponentEvent += OnComponentRemovedEvent;
					EntityTrackedEvent?.Invoke(entity);
				}
			}
			_entitiesToAdd.Clear();
			_registerRoutine = null;
		}
	}
}
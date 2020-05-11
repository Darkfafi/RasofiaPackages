using System;

namespace RasofiaGames.SimplyECS
{
	public class EntityFilterModule
	{
		private Action<Entity> _applyLogicsAction;

		internal void InitModule(Action<Entity> applyLogicsAction)
		{
			_applyLogicsAction = applyLogicsAction;

			EntityTracker tracker = EntityTracker.Instance;
			tracker.EntityTrackedEvent += OnEntityTrackedEvent;
			tracker.EntityUntrackedEvent += OnEntityUntrackedEvent;
			tracker.EntityAddedComponentEvent += OnEntityAddedComponentEvent;
			tracker.EntityRemovedComponentEvent += OnEntityRemovedComponentEvent;
		}

		internal void DeInitModule()
		{
			if(EntityTracker.IsAvailable)
			{
				EntityTracker tracker = EntityTracker.Instance;
				tracker.EntityTrackedEvent -= OnEntityTrackedEvent;
				tracker.EntityUntrackedEvent -= OnEntityUntrackedEvent;
				tracker.EntityAddedComponentEvent -= OnEntityAddedComponentEvent;
				tracker.EntityRemovedComponentEvent -= OnEntityRemovedComponentEvent;
			}
		}

		protected virtual void OnGlobalEntityTracked(Entity entity)
		{

		}

		protected virtual void OnGlobalEntityUntracked(Entity entity)
		{

		}

		protected void ExecuteValidation(Entity entity)
		{
			_applyLogicsAction(entity);
		}

		private void OnEntityTrackedEvent(Entity entity)
		{
			ExecuteValidation(entity);
			OnGlobalEntityTracked(entity);
		}

		private void OnEntityUntrackedEvent(Entity entity)
		{
			ExecuteValidation(entity);
			OnGlobalEntityUntracked(entity);
		}

		private void OnEntityAddedComponentEvent(Entity entity, EntityComponent component)
		{
			ExecuteValidation(entity);
		}

		private void OnEntityRemovedComponentEvent(Entity entity, EntityComponent component)
		{
			ExecuteValidation(entity);
		}
	}
}
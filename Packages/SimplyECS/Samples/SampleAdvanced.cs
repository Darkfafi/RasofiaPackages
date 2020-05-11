using System;
using UnityEngine;

namespace RasofiaGames.SimplyECS.Sample
{
	public class SampleAdvanced : MonoBehaviour
	{
		private IEntityFilter<HealthFilterData> _entityFilter;
		private Character _character;
		private Character _character2;

		protected void Awake()
		{
			_entityFilter = EntityFilter<HealthFilterData>.Create(new TestModule(), OnTracked, OnUntracked);
			_character = new Character(50);
			_character2 = new Character(75);
		}

		private void OnTracked(HealthFilterData obj)
		{
			Debug.Log("Hello: " + obj.Health.HealthPoints);
		}

		private void OnUntracked(HealthFilterData obj)
		{
			Debug.Log("Bye: " + obj.Health.HealthPoints);
		}

		protected void Update()
		{
			if(Input.GetKeyDown(KeyCode.Space))
			{
				_entityFilter.ForEach(x => x.Health.Damage());
			}
		}

		protected void OnDestroy()
		{
			_entityFilter.Clean(null, null);
		}

		// This will make it so casting only occures when an entity is tracked or untracked by the EntityFilter, after that, all data is set and can be accessed directly.
		public struct HealthFilterData : IFilterData
		{
			public HealthComp Health
			{
				get; private set;
			}

			// This will only be executed once when the data is inserted in the filter instance
			public void InitData(Entity entity)
			{
				Health = entity.GetEntityComponent<HealthComp>();
			}

			// This will be executed when the validation check happens, which is when entities are modified or when custom module events are triggered.
			public bool IsValidEntity(Entity entity)
			{
				return entity.TryGetEntityComponent(out HealthComp healthComp) && healthComp.HealthPoints >= 50;
			}
		}

		// Entity
		public class Character : Entity
		{
			public HealthComp Health
			{
				get;
			}

			public Character(int health)
			{
				Health = AddEntityComponent<HealthComp>();
				Health.HealthPoints = health;
			}
		}


		// Component
		public class HealthComp : EntityComponent
		{
			public event Action<HealthComp> DamagedEvent;

			public int HealthPoints = 100;

			public void Damage()
			{
				HealthPoints -= 10;
				DamagedEvent?.Invoke(this);
			}
		}

		// Modified module to run Validation check on more than the default events
		public class TestModule : EntityFilterModule
		{
			protected override void OnGlobalEntityTracked(Entity entity)
			{
				if(entity.TryGetEntityComponent(out HealthComp healthComp))
				{
					healthComp.DamagedEvent += OnDamagedEvent;
				}
			}

			protected override void OnGlobalEntityUntracked(Entity entity)
			{
				if(entity.TryGetEntityComponent(out HealthComp healthComp))
				{
					healthComp.DamagedEvent -= OnDamagedEvent;
				}
			}

			private void OnDamagedEvent(HealthComp healthComp)
			{
				ExecuteValidation(healthComp.Parent);
			}
		}
	}
}
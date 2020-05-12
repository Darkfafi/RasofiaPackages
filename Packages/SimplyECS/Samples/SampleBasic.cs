using UnityEngine;

namespace RasofiaGames.SimplyECS.Sample
{
	public class SampleBasic : MonoBehaviour
	{
		IEntityFilter<CommercialFilterData> _entityFilter;
		Building _building1;
		Building _building2;

		protected void Awake()
		{
			_entityFilter = EntityFilter<CommercialFilterData>.Create(OnTracked, OnUntracked);
			_building1 = new Building(50);
			_building2 = new Building(75);
		}

		private void OnTracked(CommercialFilterData obj)
		{
			Debug.Log("Income Added: " + obj.Commercial.Income);
		}

		private void OnUntracked(CommercialFilterData obj)
		{
			Debug.Log("Income Removed: " + obj.Commercial.Income);
		}

		protected void Update()
		{
			if(Input.GetKeyDown(KeyCode.Space))
			{
				// You could also track total income on `OnTracked` and `OnUntracked`, but for the sake of showing the loop system we placed it here.
				int totalIncome = 0;
				_entityFilter.ForEach(x => totalIncome += x.Commercial.Income);
				Debug.Log("Total Income: " + totalIncome);
			}
		}

		protected void OnDestroy()
		{
			_entityFilter.Clean(null, null);
		}

		// This will make it so casting only occures when an entity is tracked or untracked by the EntityFilter, after that, all data is set and can be accessed directly.
		public struct CommercialFilterData : IFilterData
		{
			public CommercialComponent Commercial
			{
				get; private set;
			}

			// This will only be executed once when the data is inserted in the filter instance
			public void InitData(Entity entity)
			{
				Commercial = entity.GetEntityComponent<CommercialComponent>();
			}

			// This will be executed when the validation check happens, which is when entities are modified or when custom module events are triggered.
			public bool IsValidEntity(Entity entity)
			{
				return entity.HasEntityComponent<CommercialComponent>();
			}
		}

		// Entity
		public class Building : Entity
		{
			public Building(int income)
			{
				AddEntityComponent<CommercialComponent>().Income = income;
			}
		}


		// Component
		public class CommercialComponent : EntityComponent
		{
			public int Income = 100;
		}
	}
}
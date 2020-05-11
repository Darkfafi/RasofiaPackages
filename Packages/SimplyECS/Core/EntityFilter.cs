using System;
using System.Collections.Generic;

namespace RasofiaGames.SimplyECS
{
	public class EntityFilter<T> : EntityFilter<T, EntityFilterModule> where T : struct, IFilterData
	{
		public static IEntityFilter<T> Create(Action<T> trackedCallback, Action<T> untrackedCallback, bool callForCurrentEntries = true)
		{
			return Create(new EntityFilterModule(), trackedCallback, untrackedCallback);
		}

		protected EntityFilter(EntityFilterModule module)
			: base(module)
		{

		}
	}

	public class EntityFilter<T, U> : IEntityFilter<T> where T : struct, IFilterData where U : EntityFilterModule
	{
		private Action<T> _trackedCallbacks;
		private Action<T> _untrackedCallbacks;

		private static EntityFilter<T, U> _cachedEntityFilter = null;
		private static int _refCount = 0;

		public IFilterData Validator
		{
			get;
		} = new T();

		private Dictionary<Entity, T> _filterData;
		private U _entityFilterModule;

		public static IEntityFilter<T> Create(U entityFilterModule, Action<T> trackedCallback, Action<T> untrackedCallback, bool callForCurrentEntries = true)
		{
			if(_cachedEntityFilter == null)
			{
				_cachedEntityFilter = new EntityFilter<T, U>(entityFilterModule);
			}

			_refCount++;

			_cachedEntityFilter._trackedCallbacks += trackedCallback;
			_cachedEntityFilter._untrackedCallbacks += untrackedCallback;

			if(trackedCallback != null && callForCurrentEntries)
			{
				_cachedEntityFilter.ForEach(x => trackedCallback(x));
			}

			return _cachedEntityFilter;
		}

		protected EntityFilter(U entityFilterModule)
		{
			Validator = new T();
			_filterData = new Dictionary<Entity, T>();

			_entityFilterModule = entityFilterModule;
			_entityFilterModule.InitModule(ApplyTrackLogics);

			EntityTracker.Instance.ForEach(ApplyTrackLogics);
		}

		public T[] GetAllData()
		{
			T[] entries = new T[_filterData.Count];
			_filterData.Values.CopyTo(entries, 0);
			return entries;
		}

		public void ForEach(Action<T> action)
		{
			foreach(T v in GetAllData())
			{
				action(v);
			}
		}

		public void Clean(Action<T> trackedCallback, Action<T> untrackedCallback, bool callForCurrentEntries = true)
		{
			if(_cachedEntityFilter != null && _refCount > 0)
			{
				_trackedCallbacks -= trackedCallback;
				_untrackedCallbacks -= untrackedCallback;

				if(untrackedCallback != null)
				{
					ForEach(x => untrackedCallback(x));
				}

				if(--_refCount == 0)
				{
					_entityFilterModule.DeInitModule();

					_filterData.Clear();
					_cachedEntityFilter = null;
					_entityFilterModule = null;
				}
			}
		}

		private void Track(Entity e)
		{
			if(!_filterData.ContainsKey(e))
			{
				T data = new T();
				data.InitData(e);
				_filterData.Add(e, data);
				_trackedCallbacks?.Invoke(data);
			}
		}

		private void Untrack(Entity e)
		{
			if(_filterData.TryGetValue(e, out T data))
			{
				_filterData.Remove(e);
				_untrackedCallbacks?.Invoke(data);
			}
		}

		private void ApplyTrackLogics(Entity e)
		{
			if(e != null)
			{
				if(!e.IsDestroyed && Validator.IsValidEntity(e))
				{
					Track(e);
				}
				else
				{
					Untrack(e);
				}
			}
		}
	}

	public interface IFilterData
	{
		/// <summary>
		/// This will be used to extract data which will be used within the list
		/// </summary>
		/// <param name="entity"></param>
		void InitData(Entity entity);

		/// <summary>
		/// Filter Validation check. This will be triggered on Entity Event Changes and Component Event Changes to filter the list
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		bool IsValidEntity(Entity entity);
	}

	public interface IEntityFilter<T> : IEntityFilter where T : IFilterData
	{
		T[] GetAllData();

		void ForEach(Action<T> action);

		void Clean(Action<T> trackedCallback, Action<T> untrackedCallback, bool callForCurrentEntries = true);
	}

	public interface IEntityFilter
	{
		IFilterData Validator
		{
			get;
		}
	}
}
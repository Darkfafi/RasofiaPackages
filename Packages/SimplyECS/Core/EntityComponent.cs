namespace RasofiaGames.SimplyECS
{
	public abstract class EntityComponent
	{
		public Entity Parent
		{
			get; private set;
		}

		public bool IsValid => Parent != null;

		public bool IsEnabled
		{
			get
			{
				if(!IsValid)
				{
					return false;
				}

				return Parent.IsEntityComponentEnabled(this);
			}
			set
			{
				Parent.SetEntityComponentEnabledState(this, value);
			}
		}

		public void Destroy()
		{
			Parent.RemoveEntityComponent(this);
		}

		internal void Initialize(Entity parent)
		{
			Parent = parent;
			Init();
		}

		internal void Deinitialize()
		{
			DeInit();
			Parent = null;
		}

		internal void Enabled()
		{
			if(IsValid)
			{
				OnEnabled();
			}
		}

		internal void Disabled()
		{
			if(IsValid)
			{
				OnDisabled();
			}
		}

		protected virtual void OnEnabled()
		{

		}

		protected virtual void OnDisabled()
		{

		}

		protected virtual void Init()
		{

		}

		protected virtual void DeInit()
		{

		}
	}
}
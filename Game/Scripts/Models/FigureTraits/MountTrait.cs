using Fractural.Tasks;
using System;

public class MountTrait() : FigureTrait
{
	public class AbstractBuilder<TBuilder, TTrait> :
		AbstractBuilder<TBuilder, TTrait>.ICharacterOwnerStep,
		AbstractBuilder<TBuilder, TTrait>.IOnMountedStep,
		AbstractBuilder<TBuilder, TTrait>.IOnUnmountedStep
		where TBuilder : AbstractBuilder<TBuilder, TTrait>
		where TTrait : MountTrait, new()
	{
		protected readonly TTrait Obj = new();

		public interface ICharacterOwnerStep
		{
			TBuilder WithCharacterOwner(Character characterOwner);
		}

		public interface IOnMountedStep
		{
			IOnUnmountedStep WithOnMounted(Func<Figure, GDTask> onMounted);
		}

		public interface IOnUnmountedStep
		{
			TBuilder WithOnUnmounted(Func<Figure, GDTask> onUnmounted);
		}

		public TBuilder WithCharacterOwner(Character characterOwner)
		{
			Obj._characterOwner = characterOwner;
			return (TBuilder)this;
		}

		public IOnUnmountedStep WithOnMounted(Func<Figure, GDTask> onMounted)
		{
			Obj._onMounted = onMounted;
			return (TBuilder)this;
		}

		public TBuilder WithOnUnmounted(Func<Figure, GDTask> onUnmounted)
		{
			Obj._onUnmounted = onUnmounted;
			return (TBuilder)this;
		}

		public virtual TTrait Build()
		{
			return Obj;
		}
	}

	/// <summary>
	/// A concrete implementation of the AbstractBuilder. Required to actually use the builder,
	/// as abstract builders cannot be instantiated.
	/// </summary>
	public class MountBuilder : AbstractBuilder<MountBuilder, MountTrait>
	{
		internal MountBuilder() { }
	}

	/// <summary>
	/// A convenience method that returns an instance of SummonBuilder.
	/// </summary>
	/// <returns></returns>
	public static MountBuilder.ICharacterOwnerStep Builder()
	{
		return new MountBuilder();
	}

	private Figure _characterOwner;
	private Func<Figure, GDTask> _onMounted;
	private Func<Figure, GDTask> _onUnmounted;

	private bool _mounted = false;

	public override void Activate(Figure figure)
	{
		base.Activate(figure);

		// Allow entering the same hex to mount
		ScenarioCheckEvents.CanEnterHexWithFigureCheckEvent.Subscribe(figure, this,
			parameters => parameters.OtherFigure == figure && parameters.Figure == _characterOwner,
			parameters =>
			{
				parameters.SetCanEnter();
			}
		);

		// Control the mount
		ScenarioCheckEvents.IsSummonControlledCheckEvent.Subscribe(figure, this,
			parameters => parameters.Summon == figure && _mounted,
			parameters =>
			{
				parameters.SetIsControlled();
			}
		);

		// Follow the mount when it moves or being forcefully moved
		ScenarioEvents.MoveTogetherCheckEvent.Subscribe(figure, this,
			parameters => parameters.Performer == figure && _mounted,
			parameters =>
			{
				parameters.SetOtherFigure(_characterOwner);

				return GDTask.CompletedTask;
			}
		);

		// Trigger mounted effect
		ScenarioEvents.FigureEnteredHexEvent.Subscribe(figure, this,
			parameters => parameters.Figure == _characterOwner,
			async parameters =>
			{
				if(!_mounted && _onMounted != null && parameters.Hex == figure.Hex)
				{
					await _onMounted(_characterOwner);
					_mounted = true;
				}
				else if (_mounted && _onUnmounted != null && parameters.Hex != figure.Hex)
				{
					await _onUnmounted(_characterOwner);
					_mounted = false;
				}
			}
		);
	}

	public override void Deactivate(Figure figure)
	{
		base.Deactivate(figure);

		ScenarioCheckEvents.CanEnterHexWithFigureCheckEvent.Unsubscribe(figure, this);
		ScenarioCheckEvents.IsSummonControlledCheckEvent.Unsubscribe(figure, this);
		ScenarioEvents.MoveTogetherCheckEvent.Unsubscribe(figure, this);
	}
}
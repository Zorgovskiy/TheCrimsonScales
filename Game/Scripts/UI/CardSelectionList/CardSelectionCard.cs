﻿using System;
using Godot;
using GTweens.Builders;
using GTweens.Easings;
using GTweens.Tweens;
using GTweensGodot.Extensions;

[GlobalClass]
public partial class CardSelectionCard : Control
{
	[Export]
	private Control _container;
	[Export]
	private TextureRect _textureRect;
	[Export]
	private Label _initiativeLabel;
	[Export]
	private BetterButton _cardButton;
	[Export]
	private BetterButton _initiativeButton;
	[Export]
	private Control _initiativeIndicatorContainer;

	private GTween _tween;

	public AbilityCard AbilityCard { get; private set; }
	public int Index { get; private set; }

	public bool Selected { get; private set; }
	public bool InitiativeSelected { get; private set; }

	public event Action<CardSelectionCard> CardPressedEvent;
	public event Action<CardSelectionCard> InitiativePressedEvent;
	public event Action<CardSelectionCard> MouseEnteredEvent;
	public event Action<CardSelectionCard> MouseExitedEvent;
	//public event Action<CardSelectionCard> InitiativeMouseEnterEvent;

	public void Init(AbilityCard card, int index, float delay, bool canSelect, bool canPressInitiative)
	{
		AbilityCard = card;
		Index = index;

		_textureRect.Texture = card.Model.GetTexture();
		_initiativeLabel.Text = card.Model.Initiative.ToString();

		_initiativeIndicatorContainer.Scale = Vector2.Zero;

		_cardButton.SetEnabled(canSelect, canSelect);
		_initiativeButton.SetEnabled(canPressInitiative, canPressInitiative);

		UIHelper.SetCardMaterial(_textureRect, card.CardState);

		_cardButton.Pressed += OnCardPressed;
		_initiativeButton.Pressed += OnInitiativePressed;
		_cardButton.MouseEntered += OnMouseEntered;
		_cardButton.MouseExited += OnMouseExited;

		Position = new Vector2(-600f, Position.Y);
		_tween = GTweenSequenceBuilder.New()
			.AppendTime(delay + index * 0.03f)
			.Append(this.TweenPositionX(0f, 0.3f).SetEasing(Easing.OutBack))
			.Build().Play();
	}

	public void Destroy()
	{
		CardPressedEvent = null;
		InitiativePressedEvent = null;

		_tween?.Kill();

		_tween = GTweenSequenceBuilder.New()
			.AppendTime(Index * 0.03f)
			.Append(this.TweenPositionX(-600f, 0.15f).SetEasing(Easing.InBack))
			.AppendCallback(QueueFree)
			.Build().Play();
	}

	public void SetSelected(bool selected)
	{
		if(Selected == selected)
		{
			return;
		}

		Selected = selected;

		_container.TweenPositionX(Selected ? 30 : 0, 0.1f).SetEasing(Easing.OutBack).Play();
	}

	public void SetInitiativeSelected(bool initiativeSelected)
	{
		if(InitiativeSelected == initiativeSelected)
		{
			return;
		}

		InitiativeSelected = initiativeSelected;

		if(InitiativeSelected)
		{
			_initiativeIndicatorContainer.TweenScale(1f, 0.15f).SetEasing(Easing.OutBack).Play();
		}
		else
		{
			_initiativeIndicatorContainer.TweenScale(0f, 0.15f).SetEasing(Easing.InBack).Play();
		}
	}

	private void OnCardPressed()
	{
		CardPressedEvent?.Invoke(this);
	}

	private void OnInitiativePressed()
	{
		InitiativePressedEvent?.Invoke(this);
	}

	private void OnMouseEntered()
	{
		MouseEnteredEvent?.Invoke(this);
	}

	private void OnMouseExited()
	{
		MouseExitedEvent?.Invoke(this);
	}
}
﻿using Sandbox.UI;

namespace Boomer.UI;

public class HudRootPanel : RootPanel
{
	public static HudRootPanel Current;

	public Scoreboard Scoreboard { get; set; }

	public HudRootPanel()
	{
		Current = this;

		StyleSheet.Load( "/resource/styles/hud.scss" );
		SetTemplate( "/resource/templates/hud.html" );

		AddChild<DamageIndicator>();
		AddChild<HitIndicator>();

		AddChild<InventoryBar>();
		AddChild<PickupFeed>();

		AddChild<BoomerChatBox>();
		AddChild<Speedo>();
		AddChild<KillFeed>();
		Scoreboard = AddChild<Scoreboard>();
		AddChild<VoiceList>();
		AddChild<VoiceSpeaker>();
	}

	public override void Tick()
	{
		base.Tick();

		SetClass( "game-end", DeathmatchGame.CurrentState == DeathmatchGame.GameStates.GameEnd );
		SetClass( "game-warmup", DeathmatchGame.CurrentState == DeathmatchGame.GameStates.Warmup );
	}

	protected override void UpdateScale( Rect screenSize )
	{
		base.UpdateScale( screenSize );
	}
}

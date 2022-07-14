﻿
namespace Boomer.Movement;

class GroundDash : BaseMoveMechanic
{

	public override string HudName => "Ground Dash";
	public override string HudDescription => $"Press {InputActions.Walk.GetButtonOrigin()} in air";

	public override bool AlwaysSimulate => true;
	public override bool TakesOverControl => false;
	public TimeSince TimeSinceDash { get; private set; }
	public int AmountOfDash { get; private set; }
	public float DashAlpha => Math.Clamp( TimeSinceDash / .35f, 0, 1f );

	public bool IsAirDashing;
	private bool CanDash;

	public GroundDash( BoomerController controller )
		: base( controller )
	{

	}

	public override void PreSimulate()
	{
		base.PostSimulate();

		if ( ctrl.GroundEntity != null && ctrl.DashCount >= 0 )
		{
			CanDash = true;
			IsAirDashing = false;
		}

		if ( ctrl.GroundEntity != null && ctrl.DashCount <= 1 )
		{
			if ( TimeSinceDash > 2 )
			{
				ctrl.DashCount = 2;
				if ( Host.IsServer || !ctrl.Pawn.IsLocalPawn ) return;
				Sound.FromScreen( "dashrecharge" ).SetVolume( 1f );
			}
		}

		var result = new Vector3( Input.Forward, Input.Left, 0 ).Normal;
		result *= Input.Rotation;

		if ( ctrl.GroundEntity != null && InputActions.Walk.Pressed() && CanDash == true )
		{

			if ( ctrl.DashCount == 0 )
			{
				IsAirDashing = true;
				CanDash = false;
				return;
			}

			TimeSinceDash = 0;

			ctrl.DashCount--;

			float flGroundFactor = 1.75f;
			float flMul = 100f * 1.2f;
			float forMul = 585f * 2.2f;

			if ( result.IsNearlyZero() )
			{
				ctrl.Velocity = ctrl.Rotation.Forward * forMul * flGroundFactor;
				ctrl.Velocity = ctrl.Velocity.WithZ( flMul * flGroundFactor );
				ctrl.Velocity -= new Vector3( 0, 0, 800f * 0.5f ) * Time.Delta;
			}
			else
			{

				ctrl.Velocity = result * forMul * flGroundFactor;
				ctrl.Velocity = ctrl.Velocity.WithZ( flMul * flGroundFactor );
				ctrl.Velocity -= new Vector3( 0, 0, 800f * 0.5f ) * Time.Delta;
			}

			DashEffect();
		}
	}

	private void DashEffect()
	{
		ctrl.AddEvent( "jump" );

		if ( Host.IsServer || !ctrl.Pawn.IsLocalPawn ) return;

		Particles.Create( "particles/gameplay/screeneffects/dash/ss_dash.vpcf", ctrl.Pawn );
		Sound.FromWorld( "jump.double", ctrl.Pawn.Position );
	}

}

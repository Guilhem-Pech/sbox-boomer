﻿[Library( "dm_nailgun" ), HammerEntity]
[EditorModel( "weapons/rust_smg/rust_smg.vmdl" )]
[Title( "NailGun" ), Category( "Weapons" )]
partial class NailGun : DeathmatchWeapon
{
	public static readonly Model WorldModel = Model.Load( "weapons/rust_smg/rust_smg.vmdl" );
	public override string ViewModelPath => "weapons/rust_smg/v_rust_smg.vmdl";

	public override float PrimaryRate => 10;
	public override int Bucket => 2;
	public override AmmoType AmmoType => AmmoType.Nails;

	[Net, Predicted]
	public bool Zoomed { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
	}

	public override void AttackPrimary()
	{
		if ( !TakeAmmo( 1 ) )
		{
			DryFire();

			if ( AvailableAmmo() > 0 )
			{
				Reload();
			}
			return;
		}

		var tr = Trace.Ray( Owner.EyePosition + new Vector3( 0, 0, -10 ), Owner.EyePosition + new Vector3( 0, 0, -10 ) + Owner.EyeRotation.Forward * 48 )
		.UseHitboxes()
		//.HitLayer( CollisionLayer.Water, !InWater )
		.Ignore( Owner )
		.Ignore( this )
		.Size( 4.0f )
		.Run();

		if ( tr.Hit )
		{
			//
			//Push player back
			//
			float flGroundFactor = .75f;
			float flMul = 100f * 1.8f;
			float forMul = 585f * 1.4f;

			if ( Owner is BoomerPlayer player )
			{
				player.Velocity = player.EyeRotation.Backward * forMul * flGroundFactor;
				player.Velocity = player.Velocity.WithZ( flMul * flGroundFactor );
				player.Velocity -= new Vector3( 0, 0, 800f * 0.5f ) * Time.Delta;
			}
			var damageInfo = DamageInfo.FromBullet( tr.EndPosition, 50, 1 )
			.UsingTraceResult( tr )
			.WithAttacker( Owner )
			.WithWeapon( this );
			Owner.TakeDamage( damageInfo );
		}

		ShootEffects();
		PlaySound( "rust_crossbow.shoot" );

		// TODO - if zoomed in then instant hit, no travel, 120 damage

		if ( IsServer )
		{
			var bolt = new NailProjectile();
			bolt.Position = Owner.EyePosition + new Vector3(0,0,-10);
			bolt.Rotation = Owner.EyeRotation;
			bolt.Owner = Owner;
			bolt.Velocity = Owner.EyeRotation.Forward * 100;

		}
	}
	public override void BuildInput( InputBuilder owner )
	{
		if ( Zoomed )
		{
			owner.ViewAngles = Angles.Lerp( owner.OriginalViewAngles, owner.ViewAngles, 0.2f );
		}
	}

	[ClientRpc]
	protected override void ShootEffects()
	{
		Host.AssertClient();

		ViewModelEntity?.SetAnimParameter( "fire", true );
		CrosshairLastShoot = 0;

	}

	TimeSince timeSinceZoomed;

	public override void RenderCrosshair( in Vector2 center, float lastAttack, float lastReload )
	{
		var draw = Render.Draw2D;

		if ( Zoomed )
			timeSinceZoomed = 0;

		var zoomFactor = timeSinceZoomed.Relative.LerpInverse( 0.4f, 0 );

		var color = Color.Lerp( Color.Red, Color.Yellow, lastReload.LerpInverse( 0.0f, 0.4f ) );
		draw.BlendMode = BlendMode.Lighten;
		draw.Color = color.WithAlpha( 0.2f + CrosshairLastShoot.Relative.LerpInverse( 1.2f, 0 ) * 0.5f );

		// outer lines
		{
			var shootEase = Easing.EaseInOut( lastAttack.LerpInverse( 0.4f, 0.0f ) );
			var length = 10.0f;
			var gap = 40.0f + shootEase * 50.0f;

			gap -= zoomFactor * 20.0f;


			draw.Line( 0, center + Vector2.Up * gap, length, center + Vector2.Up * (gap + length) );
			draw.Line( 0, center - Vector2.Up * gap, length, center - Vector2.Up * (gap + length) );

			draw.Color = draw.Color.WithAlpha( draw.Color.a * zoomFactor );

			for ( int i = 0; i < 4; i++ )
			{
				gap += 40.0f;

				draw.Line( 0, center - Vector2.Left * gap, length, center - Vector2.Left * (gap + length) );
				draw.Line( 0, center + Vector2.Left * gap, length, center + Vector2.Left * (gap + length) );

				draw.Color = draw.Color.WithAlpha( draw.Color.a * 0.5f );
			}
		}
	}
}

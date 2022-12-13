﻿
using Sandbox;
using System;

namespace Boomer.Movement
{
	class FallDamage : BaseMoveMechanic
	{

		public override bool AlwaysSimulate => true;
		public override bool TakesOverControl => false;

		private float prevFallSpeed;
		private bool prevGrounded;

		public FallDamage( BoomerController controller )
			: base( controller )
		{

		}

		public override void PreSimulate()
		{
			base.PreSimulate();

			prevGrounded = ctrl.GroundEntity != null;
			prevFallSpeed = ctrl.Velocity.z;
		}

		public override void PostSimulate()
		{
			base.PostSimulate();

			if ( ctrl.GroundEntity == null || prevGrounded ) return;

			Sound.FromWorld( "player.land1", ctrl.Pawn.Position );

			if ( ctrl.Pawn is not BoomerPlayer p ) return;

			var dmg = GetFallDamage( prevFallSpeed );

			if ( dmg == 0 ) return;

			p.TakeDamage( new DamageInfo() { Damage = dmg } );

			FallDamageEffect();
		}

		private void FallDamageEffect()
		{
			if ( !Game.IsServer ) return;
			using var _ = Prediction.Off();

			Sound.FromWorld( "player.fall1", ctrl.Pawn.Position );
		}

		private int GetFallDamage( float fallspeed )
		{
			fallspeed = Math.Abs( fallspeed );

			if ( fallspeed < 700 ) return 0;
			if ( fallspeed < 1000 ) return 5;
			if ( fallspeed < 1300 ) return 15;
			if ( fallspeed < 1600 ) return 30;

			return 4;
		}

	}
}

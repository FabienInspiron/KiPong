﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace KiPong
{
    public class BatKinect : Bat
    {
        private KinectInput input;

        /// <summary>
        /// Initialise une batte
        /// </summary>
        /// <param name="game">Le jeu propriétaire</param>
        /// <param name="side">Si true alors à gauche sinon à droite de l'écran</param>
        public BatKinect(Game1 game, Side side, Difficulty d, KinectInput i)
            : base(game, side, d)
        {
            input = i;
        }

        public override void Update()
        {
            if (side == Side.LEFT)
                SetPosition(new Vector2(0, input.LeftY));
            else
                SetPosition(new Vector2(position.X, input.LeftY));
        }
    }
}

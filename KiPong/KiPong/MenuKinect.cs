﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KiPong
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;

    public class MenuKinect : Menu
    {

        KinectInput input;
        public MenuKinect(Game1 g, String title, List<string> items, KinectInput input)
            : base(g, title, items)
        {
            this.input = input;
        }

        public override void Update()
        {
            float count = (float)MenuItems.Count;
            float ratio = (float)input.LeftY / (float)game.ScreenHeight;
            for (int i = 1; i <= count; i++)
            {
                if (ratio <= (float)i / count && ratio > (float)(i-1) /count )
                {
                    Iterator = i - 1;
                    break;
                }
            }

            Back = input.Retour();
            Valid = input.Valider();
        }

    }
}

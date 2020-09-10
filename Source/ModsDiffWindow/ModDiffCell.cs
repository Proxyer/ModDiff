﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary_moddiff;
using RWLayout.moddiff;
using UnityEngine;
using Verse;

namespace ModDiff
{
    public struct CellStyleData
    {
        public string marker;
        public Color bgColor;
        public Color outlineColor;
        public EdgeInsets insets;
    }

    public class ModDiffCell : CElement
    {
        public const int MarkerWidth = 16;

        public CellStyleData style;
        private bool RepresentsModified;
        private string Title;

        public ModDiffCell(CellStyleData style, bool modified, string title) : base()
        {
            this.style = style;
            this.RepresentsModified = modified;
            this.Title = title;
   

            if (RepresentsModified)
            {
                var highlight = this.AddElement(new CWidget
                {
                    DoWidgetContent = (_, bounds) =>
                    {
                        Widgets.DrawBoxSolid(bounds, style.bgColor);
                        GuiTools.UsingColor(style.outlineColor, () => GuiTools.Box(bounds, style.insets));                        
                    }
                });
                this.Embed(highlight);
            }

            var iconSlot = this.AddElement(new CElement());
            if (RepresentsModified)
            {
                var icon = iconSlot.AddElement(new CLabel { Title = style.marker });
                iconSlot.StackTop(StackOptions.Create(constrainSides: false), icon);
                iconSlot.Solver.AddConstraint(iconSlot.centerX ^ icon.centerX);
                icon.Solver.AddConstraint(icon.width ^ icon.intrinsicWidth);
            }

            var text = this.AddElement(new CLabel
            {
                Title = Title
            });

            this.StackLeft(5, (iconSlot, 16), text, 2);

            this.Solver.AddConstraint(this.height ^ text.intrinsicHeight);
        }
    }
}

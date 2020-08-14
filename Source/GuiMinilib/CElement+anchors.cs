﻿using Cassowary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModDiff.GuiMinilib
{
    public partial class CElement
    {

        private void CreateAnchors()
        {
            string variableNameBase = NamePrefix();

            left = new ClVariable(variableNameBase + "_L");
            top = new ClVariable(variableNameBase + "_T");
            right = new ClVariable(variableNameBase + "_R");
            bottom = new ClVariable(variableNameBase + "_B");
        }

        private void AddImpliedConstraints(ClSimplexSolver solver)
        {
            if (width_ != null)
            {
                solver.AddConstraint(new ClLinearEquation(right, Cl.Plus(left, new ClLinearExpression(width))));
            }
            if (height_ != null)
            {
                solver.AddConstraint(new ClLinearEquation(bottom, Cl.Plus(top, new ClLinearExpression(height))));
            }
            if (centerX_ != null)
            {
                solver.AddConstraint(new ClLinearEquation(centerX, Cl.Divide(Cl.Plus(left, new ClLinearExpression(right)), new ClLinearExpression(2))));
            }
            if (centerY_ != null)
            {
                solver.AddConstraint(new ClLinearEquation(centerY, Cl.Divide(Cl.Plus(top, new ClLinearExpression(bottom)), new ClLinearExpression(2))));
            }
        }


        public ClVariable left;
        public ClVariable top;
        public ClVariable right;
        public ClVariable bottom;

        // non-esential variables
        public ClVariable width_;
        public ClVariable height_;
        private ClVariable centerX_;
        private ClVariable centerY_;
        private ClVariable intrinsicWidth_;
        private ClVariable intrinsicHeight_;

        public ClVariable width
        {
            get
            {
                if (width_ == null)
                {
                    width_ = new ClVariable(NamePrefix() + "_W");
                }
                return width_;
            }
        }
        public ClVariable height
        {
            get
            {
                if (height_ == null)
                {
                    height_ = new ClVariable(NamePrefix() + "_H");
                }
                return height_;
            }
        }

        public ClVariable centerX
        {
            get
            {
                if (centerX_ == null)
                {
                    centerX_ = new ClVariable(NamePrefix() + "_cX");
                }
                return centerX_;
            }
        }
        public ClVariable centerY
        {
            get
            {
                if (centerY_ == null)
                {
                    centerY_ = new ClVariable(NamePrefix() + "_cY");
                }
                return centerY_;
            }
        }
        public ClVariable intrinsicWidth
        {
            get
            {
                if (intrinsicWidth_ == null)
                {
                    intrinsicWidth_ = new ClVariable(NamePrefix() + "_iW");
                    solver.AddStay(intrinsicWidth_);
                }
                return intrinsicWidth_;
            }
        }
        public ClVariable intrinsicHeight
        {
            get
            {
                if (intrinsicHeight_ == null)
                {
                    intrinsicHeight_ = new ClVariable(NamePrefix() + "_iH");
                    solver.AddStay(intrinsicHeight_);
                }
                return intrinsicHeight_;
            }
        }

    }
}
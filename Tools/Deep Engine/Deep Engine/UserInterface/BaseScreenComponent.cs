﻿// Project:         Deep Engine
// Description:     3D game engine for Ruins of Hill Deep and Daggerfall Workshop projects.
// Copyright:       Copyright (C) 2012 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    http://code.google.com/p/daggerfallconnect/

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DeepEngine.Core;
using DeepEngine.World;
#endregion

namespace DeepEngine.UserInterface
{

    /// <summary>
    /// Provides features common to all screen components.
    /// </summary>
    public abstract class BaseScreenComponent :
        IDisposable
    {

        #region Fields

        protected DeepCore core;
        protected bool enabled;
        protected object tag;
        protected PanelScreenComponent parent;
        protected Vector2 position;
        protected Vector2 size;

        Rectangle rectangle;

        HoriztonalTextAlignment horizontalAlignment = HoriztonalTextAlignment.None;
        VerticalTextAlignment verticalAlignment = VerticalTextAlignment.None;

        #endregion

        #region Properties

        /// <summary>
        /// Gets engine core.
        /// </summary>
        public DeepCore Core
        {
            get { return core; }
        }

        /// <summary>
        /// Gets or sets enabled flag.
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        /// <summary>
        /// Gets or sets custom tag.
        /// </summary>
        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        /// <summary>
        /// Gets position relative to parent panel.
        /// </summary>
        public virtual Vector2 Position
        {
            get { return position; }
            internal set { position = value;}
        }

        /// <summary>
        /// Gets size of component.
        /// </summary>
        public virtual Vector2 Size
        {
            get { return size; }
            internal set { size = value; }
        }

        /// <summary>
        /// Gets parent panel.
        /// </summary>
        public virtual PanelScreenComponent Parent
        {
            get { return parent; }
            internal set { SetParent(value); }
        }

        /// <summary>
        /// Gets screen area occupied by component.
        /// </summary>
        public Rectangle Rectangle
        {
            get
            {
                rectangle.X = (int)position.X;
                rectangle.Y = (int)position.Y;
                rectangle.Width = (int)size.X;
                rectangle.Height = (int)size.Y;
                return rectangle;
            }
        }

        /// <summary>
        /// Gets or sets horizontal alignment.
        /// </summary>
        public HoriztonalTextAlignment HorizontalAlignment
        {
            get { return horizontalAlignment; }
            set { SetHorizontalAligment(value); }
        }

        /// <summary>
        /// Gets or sets vertical alignment.
        /// </summary>
        public VerticalTextAlignment VerticalAlignment
        {
            get { return verticalAlignment; }
            set { SetVerticalAlignment(value); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="core">Engine core.</param>
        public BaseScreenComponent(DeepCore core, Vector2 position, Vector2 size)
        {
            // Store values
            this.core = core;
            this.enabled = true;
            this.tag = null;
            this.position = position;
            this.size = size;
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Called when screen component should update itself.
        /// </summary>
        /// <param name="elapsedTime">Elapsed time since last update.</param>
        public abstract void Update(TimeSpan elapsedTime);

        /// <summary>
        /// Called when screen component should draw itself.
        ///  Must be called between SpriteBatch Begin() & End() methods.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to draw with.</param>
        internal abstract void Draw(SpriteBatch spriteBatch);

        #endregion

        #region IDisposable

        /// <summary>
        /// Called when component is to be disposed.
        ///  Override if special handling needed
        ///  to dispose of component resources.
        /// </summary>
        public virtual void Dispose()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Offsets position of component relative to another component.
        /// </summary>
        /// <param name="component">Component to offset against.</param>
        /// <param name="side">The side of the component to offset from.</param>
        /// <param name="distance">Distance between offset components.</param>
        public void OffsetFrom(BaseScreenComponent component, Sides side, int distance)
        {
            // Exit if invalid offset
            if (component == null || side == Sides.None)
                return;

            // Get rectangles
            Rectangle myRect = Rectangle;
            Rectangle otherRect = component.Rectangle;

            // Offset based on side
            switch (side)
            {
                case Sides.Left:
                    position.X = otherRect.Left - distance - myRect.Width;
                    break;
                case Sides.Right:
                    position.X = otherRect.Right + distance;
                    break;
                case Sides.Top:
                    position.Y = otherRect.Top - distance - myRect.Height;
                    break;
                case Sides.Bottom:
                    position.Y = otherRect.Bottom + distance;
                    break;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Update position based on alignment options.
        /// </summary>
        protected virtual void UpdatePosition()
        {
            // Do nothing if not attached to a panel
            if (parent == null)
                return;

            // Get rectangles
            Rectangle myRect = Rectangle;
            Rectangle parentRect = Parent.Rectangle;

            // Ensure position is inside margins
            if (myRect.Left < Parent.LeftMargin)
                position.X = Parent.LeftMargin;
            if (myRect.Right > parentRect.Width - Parent.RightMargin)
                position.X = parentRect.Width - Parent.RightMargin - Size.X;
            if (myRect.Top < Parent.TopMargin)
                position.Y = Parent.TopMargin;
            if (myRect.Bottom > parentRect.Height - Parent.BottomMargin)
                position.Y = parentRect.Height - Parent.BottomMargin - Size.Y;

            // Apply horizontal alignment
            switch (horizontalAlignment)
            {
                case HoriztonalTextAlignment.Left:
                    position.X = Parent.LeftMargin;
                    break;
                case HoriztonalTextAlignment.Right:
                    position.X = parentRect.Width - Parent.RightMargin - Size.X;
                    break;
                case HoriztonalTextAlignment.Center:
                    position.X = parentRect.Width / 2 - myRect.Width / 2;
                    break;
            }

            // Set vertical position based on alignment
            switch (verticalAlignment)
            {
                case VerticalTextAlignment.Top:
                    position.Y = Parent.TopMargin;
                    break;
                case VerticalTextAlignment.Bottom:
                    position.Y = parentRect.Bottom - Parent.BottomMargin - Size.Y;
                    break;
                case VerticalTextAlignment.Middle:
                    position.Y = parentRect.Height / 2 - myRect.Height / 2;
                    break;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets horiztonal alignment.
        /// </summary>
        /// <param name="alignment">Alignment.</param>
        private void SetHorizontalAligment(HoriztonalTextAlignment alignment)
        {
            horizontalAlignment = alignment;
            UpdatePosition();
        }

        /// <summary>
        /// Sets vertical aligment.
        /// </summary>
        /// <param name="alignment">Alignment.</param>
        private void SetVerticalAlignment(VerticalTextAlignment alignment)
        {
            verticalAlignment = alignment;
            UpdatePosition();
        }

        /// <summary>
        /// Sets parent panel.
        /// </summary>
        /// <param name="parent">Parent.</param>
        private void SetParent(PanelScreenComponent parent)
        {
            this.parent = parent;
            UpdatePosition();
        }

        #endregion

    }

}
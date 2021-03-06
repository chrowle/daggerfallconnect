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
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DeepEngine.Core;
using DeepEngine.Components;
using DeepEngine.World;
using DeepEngine.Utility;
using ProjectEditor.Documents;
#endregion

namespace ProjectEditor.Proxies
{

    /// <summary>
    /// Quad terrain proxy interface.
    /// </summary>
    internal interface IQuadTerrainProxy : IEditorProxy { }

    /// <summary>
    /// Encapsulates a quad terrain component for the editor.
    /// </summary>
    internal sealed class QuadTerrainProxy : BaseComponentProxy, IBaseComponentProxy
    {

        #region Fields

        const string defaultName = "Terrain";
        const string categoryName = "Terrain";
        const string transformCategoryName = "Transform";

        QuadTerrainComponent quadTerrain;

        new Matrix matrix = Matrix.Identity;
        new Vector3 scale = Vector3.One;
        new Vector3 position = Vector3.Zero;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the QuadTerrainComponent being proxied.
        /// </summary>
        [Browsable(false)]
        public QuadTerrainComponent Component
        {
            get { return quadTerrain; }
        }

        #endregion

        #region Editor Properties

        /// <summary>
        /// Gets map dimension.
        /// </summary>
        [Category(categoryName), Description("Horizontal dimension of map along each side. Set at creation.")]
        public int MapDimension
        {
            get { return quadTerrain.MapDimension; }
        }

        /// <summary>
        /// Gets or sets texture scale.
        /// </summary>
        [Category(categoryName), Description("How often textures tile per quad node.")]
        public float TextureRepeat
        {
            get { return quadTerrain.TextureRepeat; }
            set
            {
                base.SceneDocument.PushUndo(this, this.GetType().GetProperty("TextureRepeat"));
                quadTerrain.TextureRepeat = value;
            }
        }

        /// <summary>
        /// Gets or sets normal strength.
        /// </summary>
        [Category(categoryName), Description("Controls generation of surface normals.")]
        public float NormalStrength
        {
            get { return quadTerrain.NormalStrength; }
            set
            {
                base.SceneDocument.PushUndo(this, this.GetType().GetProperty("NormalStrength"));
                quadTerrain.NormalStrength = value;
                quadTerrain.UpdateNormalData();
                quadTerrain.UpdateTerrainVertexTexture();
            }
        }

        /// <summary>
        /// Gets or sets terrain scale.
        /// </summary>
        [Category(transformCategoryName), Description("Scaling of terrain.")]
        public new Vector3 Scale
        {
            get { return scale; }
            set
            {
                base.SceneDocument.PushUndo(this, this.GetType().GetProperty("Scale"));
                scale = value;
                UpdateMatrix();
            }
        }

        /// <summary>
        /// Gets or sets terrain position.
        /// </summary>
        [Category(transformCategoryName), Description("Position of terrain.")]
        public new Vector3 Position
        {
            get { return position; }
            set
            {
                base.SceneDocument.PushUndo(this, this.GetType().GetProperty("Position"));
                position = value;
                UpdateMatrix();
            }
        }

        /// <summary>
        /// Hide base rotation.
        /// </summary>
        [Browsable(false)]
        public new Vector3 Rotation
        {
            get { return base.Rotation; }
            set { base.Rotation = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="document">Scene document.</param>
        /// <param name="entity">Entity owning this proxy.</param>
        /// <param name="quadTerrain">Quad terrain to proxy.</param>
        public QuadTerrainProxy(SceneDocument document, EntityProxy entity, QuadTerrainComponent quadTerrain)
            : base(document, entity)
        {
            base.name = defaultName;
            this.quadTerrain = quadTerrain;

            // Add to entity
            base.Entity.Components.Add(quadTerrain);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Updates matrix using current scale and position.
        /// </summary>
        private new void UpdateMatrix()
        {
            // Create matrices
            Matrix scale = Matrix.CreateScale(Scale);
            Matrix translation = Matrix.CreateTranslation(Position);

            // Set matrix
            matrix = scale * translation;
            quadTerrain.Matrix = matrix;
        }

        #endregion

        #region BaseEditorProxy Overrides

        /// <summary>
        /// Removes this proxy from editor.
        /// </summary>
        public override void Remove()
        {
            // Remove from entity
            if (entity != null)
            {
                entity.Components.Remove(quadTerrain);
            }
        }

        #endregion

    }

}

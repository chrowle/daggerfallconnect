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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DeepEngine.Core;
using DeepEngine.Daggerfall;
using DeepEngine.World;
using DeepEngine.Primitives;
using DeepEngine.Utility;
using DeepEngine.Rendering;
#endregion

namespace DeepEngine.Components
{

    /// <summary>
    /// A drawable geometric primitive.
    /// </summary>
    public class GeometricPrimitiveComponent : DrawableComponent
    {

        #region Fields

        // Primitive
        GeometricPrimitive primitive;
        GeometricPrimitiveType type;
        Vector4 color = Vector4.One;

        // Effect
        Effect renderGeometryEffect;

        #endregion

        #region Structures

        /// <summary>
        /// Defines supported geometry primitives.
        /// </summary>
        public enum GeometricPrimitiveType
        {
            Cube,
            Cylinder,
            Sphere,
            Teapot,
            Torus,
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets geometry primitive type.
        /// </summary>
        public GeometricPrimitiveType Type
        {
            get { return type; }
        }

        /// <summary>
        /// Gets or sets the diffuse colour used to render this primitive.
        /// </summary>
        public Vector4 Color
        {
            get { return color; }
            set { color = value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="core">Engine core.</param>
        public GeometricPrimitiveComponent(DeepCore core)
            :base(core)
        {
            // Load effect
            renderGeometryEffect = core.ContentManager.Load<Effect>("Effects/RenderGeometry");

            // Create cube
            MakeCube(1f);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Makes primitive a cube.
        /// </summary>
        /// <param name="size">Size of cube.</param>
        public void MakeCube(float size)
        {
            // Create primitive
            primitive = new CubePrimitive(core.GraphicsDevice, size);
            type = GeometricPrimitiveType.Cube;
            BoundingSphere = primitive.BoundingSphere;
        }

        /// <summary>
        /// Makes primitive a cylinder.
        /// </summary>
        /// <param name="height">Height of cylinder.</param>
        /// <param name="diameter">Diameter of cylinder.</param>
        /// <param name="tessellation">Tessellation of cylinder.</param>
        public void MakeCylinder(float height, float diameter, int tessellation)
        {
            // Create primitive
            primitive = new CylinderPrimitive(core.GraphicsDevice, height, diameter, tessellation);
            type = GeometricPrimitiveType.Cylinder;
            BoundingSphere = primitive.BoundingSphere;
        }

        /// <summary>
        /// Makes primitive a sphere.
        /// </summary>
        /// <param name="radius">Radius of sphere.</param>
        /// <param name="tessellation">Tessellation of sphere.</param>
        public void MakeSphere(float radius, int tessellation)
        {
            // Create primitive
            primitive = new SpherePrimitive(core.GraphicsDevice, radius * 2, tessellation);
            type = GeometricPrimitiveType.Cube;
            BoundingSphere = primitive.BoundingSphere;
        }

        /// <summary>
        /// Makes primitive a teaport.
        /// </summary>
        /// <param name="size">Size of teapot.</param>
        /// <param name="tessellation">Tessellation of teapot.</param>
        public void MakeTeapot(float size, int tessellation)
        {
            // Create primitive
            primitive = new TeapotPrimitive(core.GraphicsDevice, size, tessellation);
            type = GeometricPrimitiveType.Teapot;
            BoundingSphere = primitive.BoundingSphere;
        }

        /// <summary>
        /// Makes primitive a torus.
        /// </summary>
        /// <param name="diameter">Diameter of torus.</param>
        /// <param name="radius">Radius of torus.</param>
        /// <param name="tessellation">Tessellation of torus.</param>
        public void MakeTorus(float radius, float thickness, int tessellation)
        {
            // Create primitive
            primitive = new TorusPrimitive(core.GraphicsDevice, radius * 2, thickness, tessellation);
            type = GeometricPrimitiveType.Torus;
            BoundingSphere = primitive.BoundingSphere;
        }

        /// <summary>
        /// Makes primitive into specified type using default values.
        /// </summary>
        /// <param name="type">Type of primitive to use.</param>
        public void MakePrimitive(GeometricPrimitiveType type)
        {
            // Create default primitive
            switch (type)
            {
                case GeometricPrimitiveType.Cube:
                    primitive = new CubePrimitive(core.GraphicsDevice);
                    break;
                case GeometricPrimitiveType.Cylinder:
                    primitive = new CylinderPrimitive(core.GraphicsDevice);
                    break;
                case GeometricPrimitiveType.Sphere:
                    primitive = new SpherePrimitive(core.GraphicsDevice);
                    break;
                case GeometricPrimitiveType.Teapot:
                    primitive = new TeapotPrimitive(core.GraphicsDevice);
                    break;
                case GeometricPrimitiveType.Torus:
                    primitive = new TorusPrimitive(core.GraphicsDevice);
                    break;
                default:
                    return;
            }

            this.type = type;
            this.BoundingSphere = primitive.BoundingSphere;
        }

        /// <summary>
        /// Gets static geometry.
        /// </summary>
        /// <returns>Static geometry builder.</returns>
        public StaticGeometryBuilder GetStaticGeometry()
        {
            // Create builder
            StaticGeometryBuilder builder = new StaticGeometryBuilder(core.GraphicsDevice);

            // Convert vertices to required format
            List<VertexPositionNormalTextureBump> vertices = new List<VertexPositionNormalTextureBump>(primitive.Vertices.Count);
            foreach (var vertex in primitive.Vertices)
            {
                vertices.Add(new VertexPositionNormalTextureBump(
                    vertex.Position,
                    vertex.Normal,
                    Vector2.Zero,
                    Vector3.Zero,
                    Vector3.Zero));
            }

            // Add geometry
            StaticGeometryBuilder.BatchData batchData = new StaticGeometryBuilder.BatchData
            {
                Vertices = vertices,
                Indices = primitive.Indices,
            };
            builder.AddToBuilder((uint)MaterialManager.NullTextureKey, batchData, this.Matrix);

            return builder;
        }

        #endregion

        #region DrawableComponent Overrides

        /// <summary>
        /// Draws component.
        /// </summary>
        /// <param name="caller">Entity calling the draw operation.</param>
        public override void Draw(BaseEntity caller)
        {
            // Do nothing if disabled
            if (!enabled)
                return;

            // Calculate world matrix
            Matrix worldMatrix = matrix * caller.Matrix;

            // Set technique
            renderGeometryEffect.CurrentTechnique = renderGeometryEffect.Techniques["Diffuse"];

            // Update effect
            renderGeometryEffect.Parameters["World"].SetValue(worldMatrix);
            renderGeometryEffect.Parameters["View"].SetValue(core.ActiveScene.Camera.ViewMatrix);
            renderGeometryEffect.Parameters["Projection"].SetValue(core.ActiveScene.Camera.ProjectionMatrix);
            renderGeometryEffect.Parameters["DiffuseColor"].SetValue(color);

            // Draw primitive
            primitive.Draw(renderGeometryEffect);
        }

        /// <summary>
        /// Frees resources used by this object when they are no longer needed.
        /// </summary>
        public override void Dispose()
        {
            primitive.Dispose();
        }

        #endregion

    }

}

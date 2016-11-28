using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TBOClient
{
    public sealed class View
    {
        #region Fields
        private Vector2 position;
        private Vector2 origin;

        private Viewport viewport;

        private float zoom;
        private float rotation;
        #endregion

        #region Properties
        public string Name
        {
            get;
            set;
        }
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;

                ComputeTransform();
            }
        }
        public Vector2 Origin
        {
            get
            {
                return origin;
            }
            set
            {
                origin = value;

                ComputeTransform();
            }
        }
        public float Zoom
        {
            get
            {
                return zoom;
            }
            set
            {
                zoom = value;

                ComputeTransform();
            }
        }
        public float Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;

                ComputeTransform();
            }
        }
        public Viewport Viewport
        {
            get
            {
                return viewport;
            }
            set
            {
                viewport = value;

                ComputeTransform();
            }
        }
        public Matrix Transform
        {
            get;
            private set;
        }

        public Vector2 VisibleArea
        {
            get
            {
                var inverseViewMatrix = Matrix.Invert(Transform);

                var tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
                var tr = Vector2.Transform(new Vector2(Viewport.Width, 0), inverseViewMatrix);
                var bl = Vector2.Transform(new Vector2(0, Viewport.Height), inverseViewMatrix);
                var br = Vector2.Transform(Vector2.Zero, inverseViewMatrix);

                var min = new Vector2(
                    MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
                    MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));

                var max = new Vector2(
                    MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
                    MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));

                return new Vector2(max.X - min.X, max.Y - min.Y);
            }
        }
        #endregion

        public View(Viewport viewport)
        {
            Viewport = viewport;
            Name = string.Empty;
            Position = Vector2.Zero;
            Origin = new Vector2(viewport.Width * 0.5f, viewport.Height * 0.5f);
            Zoom = 1.0f;
            Rotation = 0.0f;
        }

        public void FocuCenter()
        {
            Origin = Vector2.Zero;
        }
        public void FocusTopLeft()
        {
            Origin = new Vector2(Viewport.Width * 0.5f, Viewport.Height * 0.5f);
        }
        public void FocusBottomRight()
        {
            Origin = new Vector2(Viewport.Width, Viewport.Height);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ComputeTransform()
        {
            Transform = Matrix.CreateTranslation(new Vector3(-(Position.X + Origin.X), -(Position.Y + Origin.Y), 0.0f)) *
                        Matrix.CreateRotationZ(Rotation) *
                        Matrix.CreateScale(new Vector3(Zoom, Zoom, 1.0f)) *
                        Matrix.CreateTranslation(new Vector3(Viewport.Width * 0.5f, Viewport.Height * 0.5f, 0.0f));
        }

        public Vector2 ScreenToView(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, Matrix.Invert(Transform));
        }
        public Vector2 ScreenToView(Point screenPosition)
        {
            return ScreenToView(new Vector2(screenPosition.X, screenPosition.Y));
        }
    }
}

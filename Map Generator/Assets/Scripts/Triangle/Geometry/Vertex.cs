// -----------------------------------------------------------------------
// <copyright file="Vertex.cs" company="">
// Original Triangle code by Jonathan Richard Shewchuk, http://www.cs.cmu.edu/~quake/triangle.html
// Triangle.NET code by Christian Woltering, http://triangle.codeplex.com/
// </copyright>
// -----------------------------------------------------------------------

namespace TriangleNet.Geometry
{
    using System;
    using TriangleNet.Topology;
    using UnityEngine;

    /// <summary>
    /// The vertex data structure.
    /// </summary>
    public class Vertex : Point
    {
        // Hash for dictionary. Will be set by mesh instance.
        internal int hash;

#if USE_ATTRIBS
        internal double[] attributes;
#endif
        internal VertexType type;
        internal Otri tri;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vertex" /> class.
        /// </summary>
        public Vertex()
            : this(0, 0, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vertex" /> class.
        /// </summary>
        /// <param name="x">The x coordinate of the vertex.</param>
        /// <param name="y">The y coordinate of the vertex.</param>
        public Vertex(double x, double y)
            : this(x, y, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vertex" /> class.
        /// </summary>
        /// <param name="x">The x coordinate of the vertex.</param>
        /// <param name="y">The y coordinate of the vertex.</param>
        /// <param name="mark">The boundary mark.</param>
        public Vertex(double x, double y, int mark)
            : base(x, y, mark)
        {
            this.type = VertexType.InputVertex;
        }

#if USE_ATTRIBS
        /// <summary>
        /// Initializes a new instance of the <see cref="Vertex" /> class.
        /// </summary>
        /// <param name="x">The x coordinate of the vertex.</param>
        /// <param name="y">The y coordinate of the vertex.</param>
        /// <param name="mark">The boundary mark.</param>
        /// <param name="attribs">The number of point attributes.</param>
        public Vertex(double x, double y, int mark, int attribs)
            : this(x, y, mark)
        {
            if (attribs > 0)
            {
                this.attributes = new double[attribs];
            }
        }
#endif

        #region Public properties

#if USE_ATTRIBS
        /// <summary>
        /// Gets the vertex attributes (may be null).
        /// </summary>
        public double[] Attributes
        {
            get { return this.attributes; }
        }
#endif

        /// <summary>
        /// Gets the vertex type.
        /// </summary>
        public VertexType Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Gets the specified coordinate of the vertex.
        /// </summary>
        /// <param name="i">Coordinate index.</param>
        /// <returns>X coordinate, if index is 0, Y coordinate, if index is 1.</returns>
        public double this[int i]
        {
            get
            {
                if (i == 0)
                {
                    return x;
                }

                if (i == 1)
                {
                    return y;
                }

                throw new ArgumentOutOfRangeException("Index must be 0 or 1.");
            }
        }

        #endregion

        public override int GetHashCode()
        {
            return this.hash;
        }

        // Add vector math operators for convinience.
        public static Vertex operator+ (Vertex a, Vertex b) { 
            return new Vertex(a.x + b.x, a.y + b.y);
        }
        public static Vertex operator- (Vertex a, Vertex b) { 
            return new Vertex(a.x - b.x, a.y - b.y);
        }

        public static Vertex operator* (Vertex a, double b) { 
            return new Vertex(a.x * b, a.y * b);
        }

        public static Vertex operator* (double a, Vertex b) { 
            return new Vertex(b.x * a, b.y * a);
        }

        public static Vertex operator/ (Vertex a, double b) { 
            return new Vertex(a.x / b, a.y / b);
        }

        public static Vertex operator/ (double a, Vertex b) { 
            return new Vertex(b.x / a, b.y / a);
        }
    
        // dot product
        public static double operator* (Vertex a, Vertex b) { 
            return a.x * b.x + a.y * b.y;
        }

        // Add Vector2 and Vector3 conversion for convinience.
        // TODO: throw an error if double to float conversion fails 

        public Vector2 ToVector2 () {
            return new Vector2((float)x, (float)y);
        }

        public Vector3 ToVector3 () {
            return new Vector3((float)x, (float)y, 0);
        }

        public Vector3 ToVector3 (float z) {
            return new Vector3((float)x, (float)y, z);
        }
        
        public double Magnitude () { 
            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
        }

        public Vertex Normalize () { 
            return this / Magnitude();
        }

        public Vertex Perpendicular() { 
            return new Vertex(this.y, -this.x);
        }
        
        public double GetAngle() { 
            double angle = Math.Atan2(this.y, this.x);
            if(angle < 0 ) angle += Math.PI * 2;
            return angle;
        }

        public double GetAngle(Vertex v) { 
            return GetAngle(v - this);
        }
    }
}

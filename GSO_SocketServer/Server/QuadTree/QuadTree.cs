using System.Collections;
using System.Collections.Generic;
using System;
using Collision.Shapes;
using Debug = System.Diagnostics.Debug;
using Vector2 = System.Numerics.Vector2;


// https://referencesource.microsoft.com/#System.Data/cdf/src/NetFx40/Tools/System.Activities.Presentation/System/Activities/Presentation/View/QuadTree.cs
//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
namespace QuadTree
{
   /* public struct Circle
    {
        
        public float radius;

        

        public Circle(Vector2 position, float radius)
        {
            Position = position;
            this.radius = radius;
        }

        // For insertion
        public bool ContainedBy(Rect rect)
        {
            return rect.Left <= Position.X - radius &&
                   rect.Top <= Position.Y - radius &&
                   rect.Left + rect.Width >= Position.X + radius &&
                   rect.Top + rect.Height >= Position.Y + radius;
        }

        // The rest is for query 
        public bool Contains(Rect rect)
        {
            // The distance to the furthest corner is less than the radius 
            return new Vector2(
                Math.Max(Math.Abs(Position.X - rect.Left), Math.Abs(Position.X - (rect.Left + rect.Width))),
                Math.Max(Math.Abs(Position.Y - rect.Top), Math.Abs(Position.Y - (rect.Top + rect.Height)))
            ).LengthSquared() < radius * radius;
        }

        // https://stackoverflow.com/questions/401847/circle-rectangle-collision-detection-intersection/1879223#1879223
        public bool IntersectsWith(Rect rect)
        {
            return Vector2.DistanceSquared(Position, new Vector2(
                Math.Clamp(Position.X, rect.Left, rect.Left + rect.Width),
                Math.Clamp(Position.Y, rect.Top, rect.Top + rect.Height)
            )) < radius * radius;
        }

        public bool IntersectsWith(Circle circle)
        {
            return Vector2.DistanceSquared(Position, circle.Position) <
                   (radius + circle.radius) * (radius + circle.radius);
        }
    }
    
     public struct Rect
    {
        public float Top;
        public float Left;
        public float Width;
        public float Height;

        public Rect(float left, float top, float width, float height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }
    }
    
    */

   

    /// <summary>
    ///     This class efficiently stores and retrieves arbitrarily sized and positioned
    ///     objects in a quad-tree data structure.  This can be used to do efficient hit
    ///     detection or visiblility checks on objects in a virtualized canvas.
    ///     The object does not need to implement any special interface because the Rect Bounds
    ///     of those objects is handled as a separate argument to Insert.
    /// </summary>
    public class QuadTree<T> where T : class
    {
        private static readonly int _minSize = 2; //가장 작은 내부 node사각형 크기
        private readonly Rectangle bounds; // overall bounds we are indexing.
        private Quadrant root;

        public QuadTree(float top, float left, float width, float height)
        {
            bounds = new Rectangle(top, left, width, height);
        }

        /// <summary>
        ///     Insert a node with given bounds into this QuadTree.
        /// </summary>
        /// <param name="node">The node to insert</param>
        /// <param name="bounds">The bounds of this node</param>
        public void Insert(T node, Circle bounds)
        {
            if (this.bounds.Width == 0 || this.bounds.Height == 0)
                throw new ArgumentException("Bounds must be non zero");

            if (bounds.isEmpty) throw new ArgumentException("Bounds must be non zero");

            if (root == null) root = new Quadrant(null, this.bounds);

            var parent = root.Insert(node, bounds);
        }

        /// <summary>
        ///     Get a list of the nodes within the specified range of a target position.
        /// </summary>
        /// <param name="position">Vector2 position to check.</param>
        /// <param name="range">Distance to check.</param>
        /// <returns>List of zero or mode nodes found inside the given bounds</returns>
        public IEnumerable<T> GetNodesInside(Vector2 position, float range)
        {
            return GetNodesInside(new Circle(position.X,position.Y, range));
        }

        /// <summary>
        ///     Get a list of the nodes that intersect the given bounds.
        /// </summary>
        /// <param name="bounds">The bounds to test</param>
        /// <returns>List of zero or mode nodes found inside the given bounds</returns>
        public IEnumerable<T> GetNodesInside(Circle bounds)
        {
            foreach (var n in GetNodes(bounds)) yield return n.Node;
        }

        /// <summary>
        ///     Get list of nodes that intersect the given bounds.
        /// </summary>
        /// <param name="bounds">The bounds to test</param>
        /// <returns>The list of nodes intersecting the given bounds</returns>
        private IEnumerable<QuadNode> GetNodes(Circle bounds)
        {
            var result = new List<QuadNode>();
            if (root != null) root.GetIntersectingNodes(result, bounds);

            return result;
        }

        public void Clear()
        {
            root = null;
        }

        /// <summary>
        ///     Each node stored in the tree has a position, width & height.
        /// </summary>
        internal class QuadNode
        {
            public Circle Bounds;
            public QuadNode Next; // linked in a circular list.
            public T Node; // the actual visual object being stored here.

            /// <summary>
            ///     Construct new QuadNode to wrap the given node with given bounds
            /// </summary>
            /// <param name="node">The node</param>
            /// <param name="bounds">The bounds of that node</param>
            public QuadNode(T node, Circle bounds)
            {
                Node = node;
                Bounds = bounds;
            }
        }


        /// <summary>
        ///     The canvas is split up into four Quadrants and objects are stored in the quadrant that contains them
        ///     and each quadrant is split up into four child Quadrants recurrsively.  Objects that overlap more than
        ///     one quadrant are stored in the this.nodes list for this Quadrant.
        /// </summary>
        internal class Quadrant
        {
            private Quadrant bottomLeft;
            private Quadrant bottomRight;
            private readonly Rectangle bounds; // quadrant bounds.

            private QuadNode nodes; // nodes that overlap the sub quadrant boundaries.
            private Quadrant parent;

            // The quadrant is subdivided when nodes are inserted that are 
            // completely contained within those subdivisions.
            private Quadrant topLeft;
            private Quadrant topRight;


            /// <summary>
            ///     Construct new Quadrant with a given bounds all nodes stored inside this quadrant
            ///     will fit inside this bounds.
            /// </summary>
            /// <param name="parent">The parent quadrant (if any)</param>
            /// <param name="bounds">The bounds of this quadrant</param>
            public Quadrant(Quadrant parent, Rectangle bounds)
            {
                this.parent = parent;
                Debug.Assert(bounds.Width != 0 && bounds.Height != 0, "Cannot have empty bound");
                if (bounds.Width == 0 || bounds.Height == 0) throw new ArgumentException("Bounds must be non zero");

                this.bounds = bounds;
            }

            /// <summary>
            ///     Insert the given node
            /// </summary>
            /// <param name="node">The node </param>
            /// <param name="bounds">The bounds of that node</param>
            /// <returns></returns>
            internal Quadrant Insert(T node, Circle bounds)
            {
                Debug.Assert(!bounds.isEmpty, "Cannot have empty bound");
                if (bounds.isEmpty) throw new ArgumentException("Bounds must be non zero");

                var toInsert = this;
                while (true)
                { 
                     //할일 : QuadTreeAct.Instance.Draw(toInsert.bounds);

                    var w = toInsert.bounds.Width / 2;
                    if (w < 1) w = 1;

                    var h = toInsert.bounds.Height / 2;
                    if (h < 1) h = 1;

                    // assumption that the Rect struct is almost as fast as doing the operations
                    // manually since Rect is a value type.

                    var topLeft = new Rectangle(toInsert.bounds.Left, toInsert.bounds.Top, w, h);
                    var topRight = new Rectangle(toInsert.bounds.Left + w, toInsert.bounds.Top, w, h);
                    var bottomLeft = new Rectangle(toInsert.bounds.Left, toInsert.bounds.Top + h, w, h);
                    var bottomRight = new Rectangle(toInsert.bounds.Left + w, toInsert.bounds.Top + h, w, h);

                    Quadrant child = null;

                    // See if any child quadrants completely contain this node.
                    if (bounds.ContainedBy(topLeft))
                    {
                        if (toInsert.topLeft == null) toInsert.topLeft = new Quadrant(toInsert, topLeft);

                        child = toInsert.topLeft;
                    }
                    else if (bounds.ContainedBy(topRight))
                    {
                        if (toInsert.topRight == null) toInsert.topRight = new Quadrant(toInsert, topRight);

                        child = toInsert.topRight;
                    }
                    else if (bounds.ContainedBy(bottomLeft))
                    {
                        if (toInsert.bottomLeft == null) toInsert.bottomLeft = new Quadrant(toInsert, bottomLeft);

                        child = toInsert.bottomLeft;
                    }
                    else if (bounds.ContainedBy(bottomRight))
                    {
                        if (toInsert.bottomRight == null) toInsert.bottomRight = new Quadrant(toInsert, bottomRight);

                        child = toInsert.bottomRight;
                    }

                    if (child != null)
                    {
                        toInsert = child;
                    }
                    else
                    {
                        var n = new QuadNode(node, bounds);
                        if (toInsert.nodes == null)
                        {
                            n.Next = n;
                        }
                        else
                        {
                            // link up in circular link list.
                            var x = toInsert.nodes;
                            n.Next = x.Next;
                            x.Next = n;
                        }

                        toInsert.nodes = n;
                        return toInsert;
                    }
                }
            }

            /// <summary>
            ///     Returns all nodes in this quadrant that intersect the given bounds.
            ///     The nodes are returned in pretty much random order as far as the caller is concerned.
            /// </summary>
            /// <param name="nodes">List of nodes found in the given bounds</param>
            /// <param name="bounds">The bounds that contains the nodes you want returned</param>
            internal void GetIntersectingNodes(List<QuadNode> nodes, Circle bounds, bool doNotCheck = false)
            {
                doNotCheck = doNotCheck || bounds.Contains(this.bounds);

                var w = this.bounds.Width / 2;
                var h = this.bounds.Height / 2;

                // assumption that the Rect struct is almost as fast as doing the operations manually since Rect is a value type.

                // See if any child quadrants completely contain this node.
                if ((doNotCheck || bounds.IntersectsWith(new Rectangle(this.bounds.Left, this.bounds.Top, w, h))) &&
                    topLeft != null)
                    topLeft.GetIntersectingNodes(nodes, bounds, doNotCheck);

                if ((doNotCheck || bounds.IntersectsWith(new Rectangle(this.bounds.Left + w, this.bounds.Top, w, h))) &&
                    topRight != null)
                    topRight.GetIntersectingNodes(nodes, bounds, doNotCheck);

                if ((doNotCheck || bounds.IntersectsWith(new Rectangle(this.bounds.Left, this.bounds.Top + h, w, h))) &&
                    bottomLeft != null)
                    bottomLeft.GetIntersectingNodes(nodes, bounds, doNotCheck);

                if ((doNotCheck || bounds.IntersectsWith(new Rectangle(this.bounds.Left + w, this.bounds.Top + h, w, h))) &&
                    bottomRight != null)
                    bottomRight.GetIntersectingNodes(nodes, bounds, doNotCheck);

                GetIntersectingNodes(this.nodes, nodes, bounds, doNotCheck);
            }

            /// <summary>
            ///     Walk the given linked list of QuadNodes and check them against the given bounds.
            ///     Add all nodes that intersect the bounds in to the list.
            /// </summary>
            /// <param name="last">The last QuadNode in a circularly linked list</param>
            /// <param name="nodes">The resulting nodes are added to this list</param>
            /// <param name="bounds">The bounds to test against each node</param>
            private static void GetIntersectingNodes(QuadNode last, List<QuadNode> nodes, Circle bounds,
                bool doNotCheck = false)
            {
                if (last != null)
                {
                    var n = last;
                    do
                    {
                        n = n.Next; // first node.
                        if (doNotCheck || n.Bounds.IntersectsWith(bounds)) nodes.Add(n);
                    } while (n != last);
                }
            }
        }
    }


}
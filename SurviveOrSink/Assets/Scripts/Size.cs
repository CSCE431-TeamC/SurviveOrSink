using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//This is just System.Drawing.Size.cs
//Unity prevents using System.Drawing so I had to add this manually

namespace Battleship
{
    public class Size
    {
        private bool _isEmpty;
        private int _width;
        private int _height;

        public static Size Empty
        {
            get { return new Size(true); }
        }

        private Size(bool isEmpty)
        {
            _isEmpty = isEmpty;
            _width = 0;
            _height = 0;
        }

        /// <summary>
        /// Initializes a new instance of the System.Drawing.Size class from the specified
        /// System.Drawing.Point.
        /// </summary>
        /// <param name="pt">
        /// The System.Drawing.Point from which to initialize this System.Drawing.Size.
        /// </param>
        public Size(Point pt)
        {
            _isEmpty = false;
            _width = pt.X;
            _height = pt.Y;
        }

        /// <summary>
        /// Initializes a new instance of the System.Drawing.Size class from the specified
        /// dimensions.
        /// </summary>
        /// <param name="width">
        /// The width component of the new System.Drawing.Size.
        /// </param>
        /// <param name="height">
        /// The height component of the new System.Drawing.Size.
        /// </param>
        public Size(int width, int height)
        {
            _isEmpty = false;
            _width = width;
            _height = height;
        }

        /// <summary>
        /// Subtracts the width and height of one System.Drawing.Size structure from
        /// width and height of another System.Drawing.Size structure.
        /// </summary>
        /// <param name="sz1">
        /// The System.Drawing.Size structure on the left side of the subtraction operator.
        /// </param>
        /// <param name="sz2">
        /// The System.Drawing.Size structure on the right side of the subtraction operator.
        /// </param>
        /// <returns>
        /// A System.Drawing.Size structure that is the result of the subtraction operation.
        /// </returns>
        public static Size operator -(Size sz1, Size sz2)
        {
            return new Size(sz1._width - sz2._width, sz1._height - sz2._height);
        }

        /// <summary>
        /// Tests whether two System.Drawing.Size structures are different.
        /// </summary>
        /// <param name="sz1">
        /// The System.Drawing.Size structure on the left of the inequality operator.
        /// </param>
        /// <param name="sz2">
        /// The System.Drawing.Size structure on the right of the inequality operator.
        /// </param>
        /// <returns>
        /// true if sz1 and sz2 differ either in width or height; false if sz1 and sz2
        /// are equal.
        /// </returns>
        public static bool operator !=(Size sz1, Size sz2)
        {
            return !(sz1 == sz2);
        }

        /// <summary>
        /// Adds the width and height of one System.Drawing.Size structure to the width
        /// and height of another System.Drawing.Size structure.
        /// </summary>
        /// <param name="sz1">
        /// The first System.Drawing.Size to add.
        /// </param>
        /// <param name="sz2">
        /// The second System.Drawing.Size to add.
        /// </param>
        /// <returns>
        /// A System.Drawing.Size structure that is the result of the addition operation.
        /// </returns>
        public static Size operator +(Size sz1, Size sz2)
        {
            return new Size(sz1._width + sz2._width, sz1._height + sz2._height);
        }


        /// <summary>
        /// Tests whether two System.Drawing.Size structures are equal.
        /// </summary>
        /// <param name="sz1">
        /// The System.Drawing.Size structure on the left side of the equality operator.
        /// </param>
        /// <param name="sz2">
        /// The System.Drawing.Size structure on the right of the equality operator.
        /// </param>
        /// <returns>
        /// This operator returns true if sz1 and sz2 have equal width and height; otherwise,
        /// false.
        /// </returns>
        public static bool operator ==(Size sz1, Size sz2)
        {
            if (System.Object.ReferenceEquals(sz1, sz2)) return true;

            if (((object)sz1 == null) || ((object)sz2 == null)) return false;

            return (sz1._width == sz2._width && sz1._height == sz2._height);
        }

        /// <summary>
        /// Converts the specified System.Drawing.Size to a System.Drawing.Point.
        /// </summary>
        /// <param name="size">
        /// The System.Drawing.Size to convert.
        /// </param>
        /// <returns>
        /// The System.Drawing.Point structure to which this operator converts.
        /// </returns>
        public static explicit operator Point(Size size)
        {
            return new Point(size._width, size._height);
        }


        /// <summary>
        /// Gets or sets the vertical component of this System.Drawing.Size.
        /// The vertical component of this System.Drawing.Size, typically measured in pixels.
        /// </summary>
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        /// <summary>
        /// Tests whether this System.Drawing.Size has width and height of 0.
        /// This property returns true when this System.Drawing.Size has both a width
        /// and height of 0; otherwise, false.
        /// </summary>
        public bool IsEmpty
        {
            get { return _isEmpty; }
        }

        /// <summary>
        /// Gets or sets the horizontal component of this System.Drawing.Size.
        /// The horizontal component of this System.Drawing.Size, typically measured
        /// in pixels.
        /// </summary>
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        /// <summary>
        /// Adds the width and height of one System.Drawing.Size structure to the width
        /// and height of another System.Drawing.Size structure.
        /// </summary>
        /// <param name="sz1">
        /// The first System.Drawing.Size to add.
        /// </param>
        /// <param name="sz2">
        /// The second System.Drawing.Size to add.
        /// </param>
        /// <returns>
        /// A System.Drawing.Size structure that is the result of the addition operation.
        /// </returns>
        public static Size Add(Size sz1, Size sz2)
        {
            return new Size(sz1._width + sz2._width, sz1._height + sz2._height);
        }



        /// <summary>
        /// Tests to see whether the specified object is a System.Drawing.Size with the
        /// same dimensions as this System.Drawing.Size.
        /// </summary>
        /// <param name="obj">
        /// The System.Object to test.
        /// </param>
        /// <returns>
        /// true if obj is a System.Drawing.Size and has the same width and height as
        /// this System.Drawing.Size; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return (this._width == ((Size)obj)._width && this._height == ((Size)obj)._height);
        }

        /// <summary>
        /// Returns a hash code for this System.Drawing.Size structure.
        /// </summary>
        /// <returns>
        /// An integer value that specifies a hash value for this System.Drawing.Size
        /// structure.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Converts the specified System.Drawing.Size to a System.Drawing.SizeF.
        /// </summary>
        /// <param name="p">
        /// The System.Drawing.Size to convert.
        /// </param>
        /// <returns>
        /// The System.Drawing.SizeF structure to which this operator converts.
        /// </returns>
        //public static implicit operator SizeF(Size p)
        //{
            //return new SizeF(p._width, p._height);
        //}

        /// <summary>
        /// Converts the specified System.Drawing.SizeF structure to a System.Drawing.Size
        /// structure by rounding the values of the System.Drawing.Size structure to
        /// the next higher integer values.
        /// </summary>
        /// <param name="value">
        /// The System.Drawing.SizeF structure to convert.
        /// </param>
        /// <returns>
        /// The System.Drawing.Size structure this method converts to.
        /// </returns>
        //public static Size Ceiling(SizeF value)
        //{
            //throw new NotImplementedException();
        //}



        /// <summary>
        /// Converts the specified System.Drawing.SizeF structure to a System.Drawing.Size
        /// structure by rounding the values of the System.Drawing.SizeF structure to
        /// the nearest integer values.
        /// </summary>
        /// <param name="value">
        /// The System.Drawing.SizeF structure to convert.
        /// </param>
        /// <returns>
        /// The System.Drawing.Size structure this method converts to.
        /// </returns>
        //public static Size Round(SizeF value)
        //{
            //throw new NotImplementedException();
        //}


        /// <summary>
        /// Subtracts the width and height of one System.Drawing.Size structure from
        /// the width and height of another System.Drawing.Size structure.
        /// </summary>
        /// <param name="sz1">
        /// The System.Drawing.Size structure on the left side of the subtraction operator.
        /// </param>
        /// <param name="sz2">
        /// The System.Drawing.Size structure on the right side of the subtraction operator.
        /// </param>
        /// <returns>
        /// The System.Drawing.Size that is a result of the subtraction operation.
        /// </returns>
        public static Size Subtract(Size sz1, Size sz2)
        {
            return new Size(sz1._width - sz2._width, sz1._height - sz2._height);
        }

        /// <summary>
        /// Creates a human-readable string that represents this System.Drawing.Size.
        /// </summary>
        /// <returns>
        /// A string that represents this System.Drawing.Size.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} {1}", _width, _height);
        }

        /// <summary>
        /// Converts the specified System.Drawing.SizeF structure to a System.Drawing.Size
        /// structure by truncating the values of the System.Drawing.SizeF structure
        /// to the next lower integer values.
        /// </summary>
        /// <param name="value">
        /// The System.Drawing.SizeF structure to convert.
        /// </param>
        /// <returns>
        /// The System.Drawing.Size structure this method converts to.
        /// </returns>
        //public static Size Truncate(SizeF value)
        //{
            //return new Size((int)value.Width, (int)value.Height);
        //}
    }
}

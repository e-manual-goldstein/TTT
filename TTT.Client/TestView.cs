using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using CocosSharp;

namespace TTT.Client
{
    public class TestView : View
    {
        private float _width;
        private float _height;

        public TestView(Context context)
            : base(context) { }

        public TestView(Context context, float width, float height) : this(context)
        {
            _width = width;
            _height = height;
        }

        protected override void OnDraw(Canvas canvas)
        {
            DrawCenteredSquare(canvas);

            //canvas.DrawText(canvas.Height.ToString(), 400, 400, paint);
            
        }

        public void DrawBlueRectangle(Canvas canvas)
        {
            var paint = new Paint() { Color = Color.Blue, StrokeWidth = 15 };
            canvas.DrawRect(10, 10, _width - 10, _height - 10, paint);
        }

        public void DrawCenteredSquare(Canvas canvas)
        {
            var paint = new Paint() { Color = Color.Blue, StrokeWidth = 15 };
            var side = Math.Min(_width, _height) / 2;
            var top = (_height - side) / 2; 
            var left = (_width - side) / 2;
            var right = left + side;
            var bottom = top + side;
            canvas.DrawRect(left, top, right, bottom, paint);
        }
    }
}

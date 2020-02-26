using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace TTT.Client
{
    public class TestLayout : FrameLayout
    {
        Context _context;

        Action<object, EventArgs> _onClick;

        public TestLayout(Context context) : base(context)
        {
            _context = context;
        }



        #region .ctors

        public TestLayout(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public TestLayout(Context context, Action<object, EventArgs> onClick) : this(context)
        {
            _onClick = onClick;
        }

        public TestLayout(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public TestLayout(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        protected TestLayout(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }
        #endregion

        public void DrawButton()
        {
            var button = new Button(_context);
            var xy = new LayoutParams(500, 500);
            button.LayoutParameters = xy;
            button.SetX(100);
            button.SetY(100);
            button.SetBackgroundColor(Android.Graphics.Color.Brown);
            button.Click += OnClickButton;
            AddView(button);

        }

        private void OnClickButton(object sender, EventArgs e)
        {
            _onClick(sender, e);
        }
    }
}
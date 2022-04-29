﻿using System;
using Eto;
using Eto.Forms;

namespace EtoFormsUI
{
    [Handler(typeof(HScrollBar.IHandler))]
    public class HScrollBar : Control
    {
        new IHandler Handler { get { return (IHandler)base.Handler; } }

        static readonly object ValueChangedKey = new object();

        public event EventHandler<EventArgs> ValueChanged;

        protected virtual void OnValueChanged(EventArgs e)
        {
            if (ValueChanged != null)
                ValueChanged(this, EventArgs.Empty);
        }

        public override int Width
        {
            get { return Handler.Width; }
            set { Handler.Width = value; }
        }

        public int Value
        {
            get { return Handler.Value; }
            set { Handler.Value = value; }
        }

        public int Maximum
        {
            get { return Handler.Maximum; }
            set { Handler.Maximum = value; }
        }
        
        static readonly object callback = new Callback();

        protected override object GetCallback()
        {
            return callback;
        }

        public new interface ICallback : Control.ICallback
        {
            void OnValueChanged(HScrollBar widget, EventArgs e);
        }

        protected new class Callback : Control.Callback, ICallback
        {
            public void OnValueChanged(HScrollBar widget, EventArgs e)
            {
                using (widget.Platform.Context)
                    widget.OnValueChanged(e);
            }
        }

        public new interface IHandler : Control.IHandler
        {
            new int Width { get; set; }
            int Value { get; set; }
            int Maximum { get; set; }
        }
    }
}

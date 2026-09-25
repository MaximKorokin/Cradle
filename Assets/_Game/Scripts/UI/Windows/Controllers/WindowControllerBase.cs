using System;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public abstract class WindowControllerBase<TWindow, TArguments> : IWindowController
        where TWindow : UIWindowBase
        where TArguments : IWindowControllerArguments
    {
        public TWindow Window { get; private set; }
        public Type WindowType => typeof(TWindow);

        protected TArguments Arguments { get; private set; }

        protected abstract void Redraw();

        protected virtual void OnBind() { }

        protected virtual void OnUnbind() { }

        protected virtual void OnInitialize() { }

        public void Bind(UIWindowBase window)
        {
            if (window is TWindow w)
            {
                Window = w;
                OnBind();
                Redraw();
            }
            else
            {
                SLog.Error($"Cannot bind a window of type {window.GetType()}. Expected window type is {WindowType}");
            }
        }

        public void Unbind()
        {
            OnUnbind();
            Window = null;
        }

        public void Initialize(IWindowControllerArguments arguments)
        {
            if (arguments is TArguments args)
            {
                Arguments = args;
                OnInitialize();
            }
            else
            {
                SLog.Error($"Cannot initialize with arguments of type {arguments.GetType()}. Expected arguments type is {typeof(TArguments)}");
            }
        }

        public virtual void Dispose()
        {
            Unbind();
        }
    }

    public interface IWindowController : IDisposable
    {
        Type WindowType { get; }

        void Bind(UIWindowBase window);
        void Unbind();
        void Initialize(IWindowControllerArguments arguments);
    }

    public interface IWindowControllerArguments { }

    public readonly struct EmptyWindowControllerArguments : IWindowControllerArguments { }
}

using System;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public abstract class WindowControllerBase<TWindow, TArguments> : IWindowController<TWindow, TArguments>
        where TWindow : UIWindowBase
        where TArguments : IWindowControllerArguments
    {
        public Type WindowType => typeof(TWindow);

        protected TArguments Arguments { get; private set; }

        public abstract void Bind(TWindow window);
        public virtual void Unbind() { }
        public virtual void Initialize(TArguments arguments) => Arguments = arguments;

        public virtual void Dispose()
        {
            Unbind();
        }

        public void Bind(UIWindowBase window)
        {
            if (window is TWindow w) Bind(w);
        }

        public void Initialize(IWindowControllerArguments arguments)
        {
            if (arguments is TArguments args) Initialize(args);
        }
    }

    public interface IWindowController<TWindow, TArguments> : IWindowController
        where TWindow : UIWindowBase
        where TArguments : IWindowControllerArguments
    {
        void Bind(TWindow window);
        void Initialize(TArguments arguments);
    }

    public interface IWindowController : IDisposable
    {
        Type WindowType { get; }

        void Bind(UIWindowBase window);
        void Initialize(IWindowControllerArguments arguments);
    }

    public interface IWindowControllerArguments { }

    public readonly struct EmptyWindowControllerArguments : IWindowControllerArguments { }
}

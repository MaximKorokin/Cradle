using System;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    public abstract class WindowControllerBase<TWindow, TArguments> : IWindowController<TWindow, TArguments>
        where TWindow : UIWindowBase
        where TArguments : IWindowControllerArguments
    {
        public abstract void Bind(TWindow window);
        public virtual void Unbind() { }
        public virtual void Initialize(TArguments arguments) { }

        public virtual void Dispose()
        {
            Unbind();
        }
    }

    public interface IWindowController<TWindow, TArguments> : IDisposable
        where TWindow : UIWindowBase
        where TArguments : IWindowControllerArguments
    {
        void Bind(TWindow window);
        void Initialize(TArguments arguments);
    }

    public interface IWindowControllerArguments { }

    public readonly struct EmptyWindowControllerArguments : IWindowControllerArguments { }
}

using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    /// <summary>
    /// Base class for window controllers that own a single child view controller/view data pair.
    /// Built on top of <see cref="MultiViewWindowControllerBase{TWindow, TArguments}"/> (registering
    /// itself as its only view), while exposing <see cref="ViewController"/>/<see cref="ViewData"/> as
    /// strongly-typed members for convenience, since this is the most common case.
    /// </summary>
    public abstract class SingleViewWindowControllerBase<TWindow, TArguments, TView, TData, TViewController> : MultiViewWindowControllerBase<TWindow, TArguments>
        where TWindow : UIWindowBase
        where TArguments : IWindowControllerArguments
        where TView : UIViewBase<TData>
        where TData : IDataAggregator
        where TViewController : ViewControllerBase<TView, TData>
    {
        protected readonly TViewController ViewController;
        protected readonly TData ViewData;

        protected SingleViewWindowControllerBase(TViewController viewController, TData viewData)
        {
            ViewController = viewController;
            ViewData = viewData;

            RegisterView<TView, TData, TViewController>(viewController, viewData, GetView);
        }

        /// <summary>
        /// Resolves the sub-view hosted by <see cref="WindowControllerBase{TWindow, TArguments}.Window"/>.
        /// </summary>
        protected abstract TView GetView();
    }
}

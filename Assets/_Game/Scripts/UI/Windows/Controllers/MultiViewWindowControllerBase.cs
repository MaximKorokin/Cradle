using Assets._Game.Scripts.UI.DataAggregators;
using Assets._Game.Scripts.UI.Views;
using Assets._Game.Scripts.UI.Views.Controllers;
using System;
using System.Collections.Generic;

namespace Assets._Game.Scripts.UI.Windows.Controllers
{
    /// <summary>
    /// Non-generic handle used by <see cref="MultiViewWindowControllerBase{TWindow, TArguments}"/> to
    /// uniformly Bind/Unbind/Render/Dispose a <see cref="ViewControllerBase{TView, TData}"/> instance
    /// regardless of its concrete view/data types.
    /// </summary>
    public interface IBoundViewController : IDisposable
    {
        void Bind();
        void Unbind();
        void Render();
    }

    /// <summary>
    /// Pairs a <see cref="ViewControllerBase{TView, TData}"/> with its view (resolved lazily) and data,
    /// so it can be registered with <see cref="MultiViewWindowControllerBase{TWindow, TArguments}"/>.
    /// </summary>
    public sealed class BoundViewController<TView, TData, TViewController> : IBoundViewController
        where TView : UIViewBase<TData>
        where TData : IDataAggregator
        where TViewController : ViewControllerBase<TView, TData>
    {
        private readonly TViewController _viewController;
        private readonly TData _viewData;
        private readonly Func<TView> _getView;

        public BoundViewController(TViewController viewController, TData viewData, Func<TView> getView)
        {
            _viewController = viewController;
            _viewData = viewData;
            _getView = getView;
        }

        public void Bind()
        {
            _viewController.Initialize(_getView());
            _viewController.Bind(_viewData);
        }

        public void Unbind()
        {
            _viewController.Unbind();
        }

        public void Render()
        {
            _viewController.Render();
        }

        public void Dispose()
        {
            _viewData.Dispose();
            _viewController.Dispose();
        }
    }

    /// <summary>
    /// Base class for window controllers that own several child view controller/view data pairs.
    /// Register each child via <see cref="RegisterView{TView, TData, TViewController}"/> (typically in the
    /// constructor); the base class then handles the common Bind/Unbind/Redraw/Dispose wiring for all of them.
    /// </summary>
    public abstract class MultiViewWindowControllerBase<TWindow, TArguments> : WindowControllerBase<TWindow, TArguments>
        where TWindow : UIWindowBase
        where TArguments : IWindowControllerArguments
    {
        private readonly List<IBoundViewController> _boundViewControllers = new();
        private readonly List<IDataAggregator> _dataAggregators = new();

        protected void RegisterView<TView, TData, TViewController>(TViewController viewController, TData viewData, Func<TView> getView)
            where TView : UIViewBase<TData>
            where TData : IDataAggregator
            where TViewController : ViewControllerBase<TView, TData>
        {
            _boundViewControllers.Add(new BoundViewController<TView, TData, TViewController>(viewController, viewData, getView));

            viewData.Invalidated += RequestClose;
            _dataAggregators.Add(viewData);
        }

        protected override void OnBind()
        {
            base.OnBind();

            foreach (var boundViewController in _boundViewControllers)
            {
                boundViewController.Bind();
            }
        }

        protected override void OnUnbind()
        {
            base.OnUnbind();

            foreach (var boundViewController in _boundViewControllers)
            {
                boundViewController.Unbind();
            }
        }

        protected override void Redraw()
        {
            foreach (var boundViewController in _boundViewControllers)
            {
                boundViewController.Render();
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            foreach (var dataAggregator in _dataAggregators)
            {
                dataAggregator.Invalidated -= RequestClose;
            }

            foreach (var boundViewController in _boundViewControllers)
            {
                boundViewController.Dispose();
            }
        }
    }
}

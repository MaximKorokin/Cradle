using Assets._Game.Scripts.UI.Views;
using System;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public class DataAggregatorController<TView, TData> : IDisposable
        where TView : UIViewBase<TData>
        where TData : IDataAggregator
    {
        private readonly TView _view;
        private readonly TData _data;

        public DataAggregatorController(TView view, TData data)
        {
            _view = view;
            _data = data;

            _data.Changed -= HandleDataChanged;
            _data.Changed += HandleDataChanged;
            HandleDataChanged();
        }

        public void Dispose()
        {
            if (_data != null)
                _data.Changed -= HandleDataChanged;
        }

        private void HandleDataChanged()
        {
            _view.RequestRender(_data);
        }
    }
}

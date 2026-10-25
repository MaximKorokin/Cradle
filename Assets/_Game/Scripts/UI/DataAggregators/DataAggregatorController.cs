using Assets._Game.Scripts.UI.Views;
using System;

namespace Assets._Game.Scripts.UI.DataAggregators
{
    public class DataAggregatorController<TView, TData> : IDisposable
        where TView : UIViewBase<TData>
        where TData : IDataAggregator
    {
        public readonly TView View;
        public readonly TData Data;

        public DataAggregatorController(TView view, TData data)
        {
            View = view;
            Data = data;

            Data.Changed -= HandleDataChanged;
            Data.Changed += HandleDataChanged;
            Data.Invalidated -= HandleDataInvalidated;
            Data.Invalidated += HandleDataInvalidated;
            HandleDataChanged();
        }

        public void Dispose()
        {
            if (Data != null)
            {
                Data.Changed -= HandleDataChanged;
                Data.Invalidated -= HandleDataInvalidated;
                Data.Dispose();
            }
        }

        private void HandleDataInvalidated()
        {
            View.gameObject.SetActive(false);
        }

        private void HandleDataChanged()
        {
            View.RequestRender(Data);
        }
    }
}

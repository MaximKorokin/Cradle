using Assets._Game.Scripts.UI.Common;
using Assets._Game.Scripts.UI.DataAggregators;
using System;
using TMPro;
using UnityEngine;

namespace Assets._Game.Scripts.UI.Views
{
    public sealed class CompactEntityStatusView : UIViewBase<EntityStatusViewData>
    {
        [Header("HP")]
        [SerializeField]
        private FillBar _hpFillBar;
        [SerializeField]
        private TMP_Text _hpText;
        [Header("MP")]
        [SerializeField]
        private FillBar _mpFillBar;
        [SerializeField]
        private TMP_Text _mpText;
        [Header("Level & Experience")]
        [SerializeField]
        private FillBar _experienceFillBar;
        [SerializeField]
        private TMP_Text _experienceText;
        [SerializeField]
        private TMP_Text _levelText;

        // todo: optimize by only updating changed values instead of redrawing everything
        protected override void Render(EntityStatusViewData playerStateViewData)
        {
            // HP
            _hpFillBar.SetFillRatio(Data.CurrentHp / Data.MaxHp);
            _hpText.text = $"{MathF.Floor(Data.CurrentHp)} / {MathF.Floor(Data.MaxHp)}";

            // MP
            _mpFillBar.SetFillRatio(1);
            _mpText.text = "1 / 1";

            // Level & Experience
            _experienceFillBar.SetFillRatio(Data.NormalizedExperience);
            _experienceText.text = $"{Data.NormalizedExperience * 100:00.000}%";
            _levelText.text = $"{Data.Level}";
        }
    }
}

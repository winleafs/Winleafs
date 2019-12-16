namespace Winleafs.Wpf.Views.Effects
{
    /// <summary>
    /// Every user control containing <see cref="EffectComboBox"/>
    /// should implement this interface to provide methods when
    /// actions are performed with the ComboBox
    /// </summary>
    public interface IEffectComboBoxContainer
    {
        public void EffectComboBoxSelectionChanged(string selectedEffect);
    }
}

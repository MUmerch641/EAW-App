using Xamarin.Forms;

namespace EatWork.Mobile.Utils
{
    public class HideEmptyLabelBehavior : Behavior<Label>
    {
        protected override void OnAttachedTo(Label label)
        {
            base.OnAttachedTo(label);

            label.PropertyChanged += this.Label_OnPropertyChanged;
        }

        protected override void OnDetachingFrom(Label label)
        {
            base.OnDetachingFrom(label);

            label.PropertyChanged -= this.Label_OnPropertyChanged;
        }

        private void Label_OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Label.Text))
            {
                this.UpdateVisibility(sender as Label);
            }
        }

        private void UpdateVisibility(Label label)
        {
            if (label != null)
            {
                label.IsVisible = !string.IsNullOrWhiteSpace(label.Text);
            }
        }
    }
}
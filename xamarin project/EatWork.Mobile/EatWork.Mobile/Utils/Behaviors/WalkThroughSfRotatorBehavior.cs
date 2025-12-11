using EatWork.Mobile.Models;
using EatWork.Mobile.ViewModels;
using Syncfusion.SfRotator.XForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace EatWork.Mobile.Utils
{
    [Preserve(AllMembers = true)]
    public class WalkThroughSfRotatorBehavior : Behavior<SfRotator>
    {
        #region Fields

        private int previousIndex;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Invoke when initialize animate to view.
        /// </summary>
        /// <param name="rotator">The SfRotator</param>
        /// <param name="selectedIndex">Selected Index</param>
        public void Animation(SfRotator rotator, double selectedIndex)
        {
            var test = rotator.ItemsSource;
            //if (rotator != null && rotator.ItemsSource != null && rotator.ItemsSource.Count() > 0)
            if (rotator != null && rotator.ItemsSource != null)
            {
                int itemsCount = rotator.ItemsSource.Count();
                int.TryParse(selectedIndex.ToString(), out int index);

                var viewModel = rotator.BindingContext as WalkthroughViewModel;

                if (itemsCount == 1)
                {
                    viewModel.NextButtonText = "CONTINUE";
                    viewModel.ShowSkipButton = false;
                }
                else if (selectedIndex == itemsCount - 1 && itemsCount > 1)
                {
                    viewModel.NextButtonText = "FINISH";
                    viewModel.ShowSkipButton = false;
                }
                else
                {
                    viewModel.NextButtonText = "NEXT";
                    viewModel.ShowSkipButton = true;
                }

                if (Device.RuntimePlatform != Device.UWP)
                {
                    var items = (rotator.ItemsSource as IEnumerable<object>).ToList();

                    // Start animation to selected view.
                    var currentItem = items[index];
                    var childElement = (((currentItem as Boarding).RotatorItem as ContentView).Children[0] as StackLayout).Children.ToList();
                    if (childElement != null && childElement.Count > 0)
                    {
                        this.StartAnimation(childElement, currentItem as Boarding);
                    }

                    // Set default value to previous view.
                    if (index != this.previousIndex)
                    {
                        var previousItem = items[this.previousIndex];
                        var previousChildElement = (((previousItem as Boarding).RotatorItem as ContentView).Children[0] as StackLayout).Children.ToList();
                        if (previousChildElement != null && previousChildElement.Count > 0)
                        {
                            previousChildElement[0].FadeTo(0, 250);
                            previousChildElement[1].FadeTo(0, 250);
                            previousChildElement[1].TranslateTo(0, 60, 250);
                            previousChildElement[1].ScaleTo(1, 0);
                            previousChildElement[2].FadeTo(0, 250);
                            previousChildElement[2].TranslateTo(0, 60, 250);
                        }
                    }

                    this.previousIndex = index;
                }
            }
        }

        /// <summary>
        /// Animation start to view.
        /// </summary>
        /// <param name="childElement">The Child Element</param>
        /// <param name="item">The Item</param>
        public async void StartAnimation(List<View> childElement, Boarding item)
        {
            var fadeAnimationImage = childElement[0].FadeTo(1, 250);
            var fadeAnimationtaskTitleTime = childElement[1].FadeTo(1, 1000);
            var translateAnimation = childElement[1].TranslateTo(0, 0, 500);
            var scaleAnimationTitle = childElement[1].ScaleTo(1.5, 500, Easing.SinIn);
            var fadeAnimationTaskDescriptionTime = childElement[2].FadeTo(1, 1000);
            var translateDescriptionAnimation = childElement[2].TranslateTo(0, 0, 500);

            var animation = new Animation();
            var scaleDownAnimation = new Animation(v => childElement[0].Scale = v, 0.5, 1, Easing.SinIn);
            animation.Add(0, 1, scaleDownAnimation);
            animation.Commit((item as Boarding).RotatorItem as ContentView, "animation", 16, 500);

            await Task.WhenAll(fadeAnimationTaskDescriptionTime, fadeAnimationtaskTitleTime, translateAnimation, scaleAnimationTitle, translateDescriptionAnimation);
        }

        /// <summary>
        /// Invoke when adding rotator to view.
        /// </summary>
        /// <param name="rotator">The Rotator</param>
        protected override void OnAttachedTo(SfRotator rotator)
        {
            base.OnAttachedTo(rotator);
            rotator.SelectedIndexChanged += this.Rotator_SelectedIndexChanged;
            rotator.BindingContextChanged += this.Rotator_BindingContextChanged;
        }

        /// <summary>
        /// Invoke when exit from the view.
        /// </summary>
        /// <param name="rotator"></param>
        protected override void OnDetachingFrom(SfRotator rotator)
        {
            base.OnDetachingFrom(rotator);
            rotator.SelectedIndexChanged -= this.Rotator_SelectedIndexChanged;
            rotator.BindingContextChanged -= this.Rotator_BindingContextChanged;
        }

        /// <summary>
        /// Invoked when rotator binding context is changed.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The event args</param>
        private void Rotator_BindingContextChanged(object sender, EventArgs e)
        {
            Task.Delay(500).ContinueWith(t => this.Animation(sender as SfRotator, 0));
        }

        /// <summary>
        /// Invoked when selected index is changed.
        /// </summary>
        /// <param name="sender">The rotator</param>
        /// <param name="e">The selection changed event args</param>
        private void Rotator_SelectedIndexChanged(object sender, SelectedIndexChangedEventArgs e)
        {
            SfRotator rotator = sender as SfRotator;
            this.Animation(rotator, e.Index);
        }
    }

    #endregion Methods
}
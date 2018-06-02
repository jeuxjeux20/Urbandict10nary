using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using UrbanDictionnet;
using System.Threading.Tasks;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UrbanDict10nary
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
        private UrbanClient client = new UrbanClient();

        private async void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var saved = sender.Text.Clone();
                var autoComplete = await client.GetAutocompletionFor(sender.Text);
                if (sender.Text != (string)saved)
                {
                    return;
                }
                sender.ItemsSource = autoComplete;
            }
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = args.SelectedItem.ToString();
        }
        private List<Task> tasksWaiting = new List<Task>();
        private async void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            try
            {
                await SetResultsFor(args.QueryText);
            }
            catch (WordNotFoundException)
            {
                sender.ContextFlyout.ShowAt(sender);
            }
        }

        private async Task SetResultsFor(string args)
        {
            var items = await client.GetWordAsync(args);
            ResultContainer.Visibility = Visibility.Visible;
            Result.Text = $"Results for: {args}";
            WordItems.ItemsSource = items;
        }
        private async Task SetResultsFor(Task<WordDefine> task)
        {
            var items = await task;
            ResultContainer.Visibility = Visibility.Visible;
            Result.Text = $"Results for: {items.First().Word}";
            WordItems.ItemsSource = items;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NotFoundFlyout.Hide();
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            await SetResultsFor(client.GetWordAsync((await client.GetRandomWordAsync()).Word));
        }
    }
}
 
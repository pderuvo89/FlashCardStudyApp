using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.Json;
using Microsoft.Win32;
using System.IO;

namespace FlashCardStudyApp.windows
{
    /// <summary>
    /// Interaction logic for createFlashCardWindow.xaml
    /// </summary>
    public partial class createFlashCardWindow : Window
    {
        private MainWindow mainWindow;

        public createFlashCardWindow(MainWindow mainWindow)
        {
            InitializeComponent();

            this.mainWindow = mainWindow;
        }

        private void ToggleButtons()
        {
            moveFlashcardDownButton.IsEnabled = moveFlashcardUpButton.IsEnabled = flashCardDeckListBox.Items.Count > 1;
            removeSelectedFlashcardButton.IsEnabled = flashCardDeckListBox.Items.Count > 0;
        }

        private void AddFlashCard()

        {
            if (string.IsNullOrWhiteSpace(termText.Text) || string.IsNullOrWhiteSpace(definitionText.Text))
            {
                MessageBox.Show("Term or definition is empty! Please make sure both fields are filled out.");
            }
            else
            {
                FlashCard flashCard = new FlashCard(termText.Text, definitionText.Text);
                flashCardDeckListBox.Items.Add(flashCard);
                termText.Clear();
                definitionText.Clear();
                ToggleButtons();
            }


        }

        private void SwitchCards(int index1, int index2)
        {
            FlashCard temp = (FlashCard)flashCardDeckListBox.Items[index1];
            flashCardDeckListBox.Items[index1] = flashCardDeckListBox.Items[index2];
            flashCardDeckListBox.Items[index2] = temp;
        }



        private void AddToDeckButton_Click(object sender, RoutedEventArgs e)
        {
            AddFlashCard();
        }

        private void MoveFlashcardUpButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = flashCardDeckListBox.SelectedIndex;
            int previousIndex = selectedIndex - 1;

            if (previousIndex < 0)
            {
                previousIndex = flashCardDeckListBox.Items.Count-1;
            }

            SwitchCards(selectedIndex, previousIndex);
            flashCardDeckListBox.SelectedItem = flashCardDeckListBox.Items.GetItemAt(previousIndex);
            
        }
        private void MoveFlashcardDownButton_Click(object sender, RoutedEventArgs e)
        {

            int selectedIndex = flashCardDeckListBox.SelectedIndex;
            int nextIndex = selectedIndex + 1;

            if (nextIndex >= flashCardDeckListBox.Items.Count)
            {
                nextIndex = 0;
            }

            SwitchCards(selectedIndex, nextIndex);
            flashCardDeckListBox.SelectedItem = flashCardDeckListBox.Items.GetItemAt(nextIndex);
        }

        private void RemoveSelectedFlashcardButton_Click(object sender, RoutedEventArgs e)
        {
            int index = flashCardDeckListBox.SelectedIndex;
            flashCardDeckListBox.Items.RemoveAt(index);

            ToggleButtons();
        }

        private void SaveFlashcardDeckButton_Click(object sender, RoutedEventArgs e)
        {

            if (flashCardDeckListBox.Items.Count == 0)
            {
                MessageBox.Show("There are no FlashCards in the current deck! Add some cards to the deck and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            if (string.IsNullOrWhiteSpace(deckNameText.Text))
            {
                MessageBox.Show("Invalid deck name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            List<FlashCard> flashCards = new List<FlashCard>();

            foreach (FlashCard flashcard in flashCardDeckListBox.Items)
            {
                flashCards.Add(flashcard);
            }

            FlashCardDeck deck = new FlashCardDeck(deckNameText.Text, flashCards);

            string jsonString = JsonSerializer.Serialize(deck);

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = HelperClass.DialogFilter;

            if (saveDialog.ShowDialog() == true) // Ok button was pressed on the save dialog, save the JSON text
            {
                try
                {
                    File.WriteAllText(saveDialog.FileName, jsonString);

                    MessageBox.Show("Deck Saved!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occured while trying to save the flashcard deck.\n\nException message: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (flashCardDeckListBox.Items.Count > 0)
            {
                if (MessageBox.Show("Leave without saving? All cards in the current Deck will be lost.", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    e.Cancel = true;

                    return;
                }
            }

            mainWindow.Show();
        }
    }
}

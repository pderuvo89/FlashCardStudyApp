using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FlashCardStudyApp.windows
{
 

    public partial class studyFlashCardsWindow : Window
    {
        private MainWindow mainWindow;

        private FlashCardDeck currentDeck;

        private int flashcardsListCurrentIndex = 0;

        private FlashCardState currentFlashCardState;

        public studyFlashCardsWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            UpdateFlashcardUI();

            this.mainWindow = mainWindow;

            currentFlashCardState = FlashCardState.Term;
        }


        /// <summary>
        /// Updates the progress text below the flashcard interactive area.
        /// </summary>
        private void UpdateProgressText()
        {
            string progress = (currentDeck == null) ? "0/0" : (flashcardsListCurrentIndex + 1) + "/" + currentDeck.FlashcardsList.Count;
            progressText.Text = progress;
        }

      
        private void UpdateFlashcardUI()
        {
            UpdateProgressText();
            ToggleNavigationButtons();
            ToggleunloadCurrentDeckButton();

            if (currentDeck == null || currentDeck.FlashcardsList.Count == 0) 
            {
                flashcardContainer.Cursor = Cursors.No;
                deckTitle.Text = "Load a Deck to begin";
                flashcardText.Text = "";

                return;
            }
            else 
            {
                flashcardContainer.Cursor = Cursors.Hand;
            }

            deckTitle.Text = currentDeck.Name;

          
            FlashCard currentFlashcard = currentDeck.FlashcardsList[flashcardsListCurrentIndex];
            flashcardText.Text = (currentFlashCardState == FlashCardState.Term) ? currentFlashcard.Term : currentFlashcard.Definition;
            flashcardText.FontWeight = (currentFlashCardState == FlashCardState.Term) ? FontWeights.Bold : FontWeights.Normal; 
        }

        private void ToggleNavigationButtons()
        {
            previousFlashcardButton.IsEnabled = (flashcardsListCurrentIndex > 0);
            nextFlashcardButton.IsEnabled = (flashcardsListCurrentIndex < (currentDeck == null ? -1 : currentDeck.FlashcardsList.Count - 1));
        }

   
        private void ToggleunloadCurrentDeckButton()
        {
            unloadCurrentDeckButton.IsEnabled = (flashcardDeckListBox.Items.Count > 0);
        }

     
        private bool AlreadyLoaded(FlashCardDeck FlashCardDeck)
        {
            foreach (FlashCardDeck fs in flashcardDeckListBox.Items)
            {
                if (FlashCardDeck.Equals(fs)) 
                {
                    return true;
                }
            }

            return false;
        }

        private void PreviousFlashcard()
        {
            flashcardsListCurrentIndex--;
            if (flashcardsListCurrentIndex < 0)
            {
                flashcardsListCurrentIndex = 0;

                return;
            }

            currentFlashCardState = FlashCardState.Term;

            UpdateFlashcardUI();
        }

     
        private void NextFlashcard()
        {
            flashcardsListCurrentIndex++;
            if (flashcardsListCurrentIndex >= currentDeck.FlashcardsList.Count)
            {
                flashcardsListCurrentIndex = currentDeck.FlashcardsList.Count - 1;

                return;
            }

            currentFlashCardState = FlashCardState.Term;

            UpdateFlashcardUI();
        }

        private void ToggleFlashcardFace()
        {
            if (currentDeck == null)
            {
                return;
            }

            currentFlashCardState = (currentFlashCardState == FlashCardState.Definition) ? FlashCardState.Term : FlashCardState.Definition;

            UpdateFlashcardUI();
        }


        private void loadDeckButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = HelperClass.DialogFilter;

            if (openDialog.ShowDialog() == true) 
            {
                try
                {
                    string fileText = File.ReadAllText(openDialog.FileName);

                
                    using (JsonDocument document = JsonDocument.Parse(fileText))
                    {
                        JsonElement root = document.RootElement;

                        string setName = root.GetProperty(HelperClass.DeckNameJsonPropertyName).ToString();

                        // Get the flashcards in the JSON
                        List<FlashCard> flashcardsList = new List<FlashCard>();
                        foreach (JsonElement element in root.GetProperty(HelperClass.FlashcardsListJsonPropertyName).EnumerateArray())
                        {
                            string term = element.GetProperty(HelperClass.TermJsonPropertyName).ToString();
                            string definition = element.GetProperty(HelperClass.DefinitionJsonPropertyName).ToString();

                            FlashCard flashcard = new FlashCard(term, definition);
                            flashcardsList.Add(flashcard);
                        }

                        FlashCardDeck set = new FlashCardDeck(setName, flashcardsList);

                        if (AlreadyLoaded(set)) // If the flashcard set is already loaded, prompt the user to make sure that that want to load in the flashcard set again
                        {
                            MessageBoxResult msgResult = MessageBox.Show("Flashcard set has already been loaded into the program! Are you sure you still want to load in flashcard set?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);

                            if (msgResult == MessageBoxResult.No)
                            {
                                return;
                            }
                        }

                        flashcardDeckListBox.Items.Add(set);
                        flashcardDeckListBox.SelectedItem = set;
                        flashcardsListCurrentIndex = 0;

                        UpdateFlashcardUI();

                        MessageBox.Show("Flashcard Deck loaded succesfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occured while trying to parse the file '" + openDialog.FileName + "' which resulted in the flashcard Deck not being loaded into the program.\n\nException message: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Event handler for when the "Unload Current Set" button is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments for when the button is clicked.</param>
        private void unloadCurrentDeckButton_Click(object sender, RoutedEventArgs e)
        {
            if (flashcardDeckListBox.Items.Count > 0)
            {
                int selectedIndex = flashcardDeckListBox.Items.IndexOf(currentDeck);
                flashcardDeckListBox.Items.RemoveAt(selectedIndex);

                if (flashcardDeckListBox.Items.Count > 0)
                {
                    flashcardDeckListBox.SelectedIndex = 0;
                }
                else
                {
                    currentDeck = null;
                }

                currentFlashCardState = FlashCardState.Term;
                flashcardsListCurrentIndex = 0;

                UpdateFlashcardUI();
            }
        }

        /// <summary>
        /// Event handler for when the user clicks on the flashcard container with the left mouse button.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments for flashcard container is clicked.</param>
        private void FlashcardContainer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            flashcardContainer.Focus();

            ToggleFlashcardFace();
        }

        /// <summary>
        /// Event handler for when the "Previous Flashcard" button is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments for when the button is clicked.</param>
        private void PreviousFlashcardButton_Click(object sender, RoutedEventArgs e)
        {
            PreviousFlashcard();
        }

        /// <summary>
        /// Event handler for when the "Next Flashcard" button is clicked.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments for when the button is clicked.</param>
        private void NextFlashcardButton_Click(object sender, RoutedEventArgs e)
        {
            NextFlashcard();
        }

        /// <summary>
        /// Event handler for when the item selection changes in the flashcard set list box.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments for when the selection is changed.</param>
        private void flashcardDeckListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Update the current flashcard to the one selected
            currentDeck = (FlashCardDeck)flashcardDeckListBox.SelectedItem;
            currentFlashCardState = FlashCardState.Term;
            flashcardsListCurrentIndex = 0;

            UpdateFlashcardUI();
        }

        /// <summary>
        /// Event handler for when the window is closing.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments for when the window is closing.</param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (currentDeck != null && currentDeck.FlashcardsList.Count > 0) // Show warning message if the user has a flashcard set currently loaded
            {
                string loadedFlashcardMessage = (currentDeck.FlashcardsList.Count > 0) ? "You have flashcard sets" : "You have a flashcard set";

                if (MessageBox.Show("Are you sure you want to exit the window? " + loadedFlashcardMessage + " loaded in the program.", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    e.Cancel = true; // Set this to true to cancel the window from closing

                    return;
                }
            }

            mainWindow.Show();
        }
    }
}

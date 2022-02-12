using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlashCardStudyApp
{

    class FlashCardDeck
    {

        [JsonPropertyName(HelperClass.DeckNameJsonPropertyName)]
        public string Name { get; set; }

        [JsonPropertyName(HelperClass.FlashcardsListJsonPropertyName)]
        public List<FlashCard> FlashcardsList { get; private set; }



        public FlashCardDeck(string name, List<FlashCard> flashCardsList)
        {
            this.Name = name;
            this.FlashcardsList = flashCardsList;
        }

        public override string ToString()
        {
            return Name + " [" + FlashcardsList.Count + " flashcard" + (FlashcardsList.Count == 1 ? "" : "s") + "]";
        }
    }
}

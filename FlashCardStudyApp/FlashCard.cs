using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlashCardStudyApp
{
    public class FlashCard
    {

        [JsonPropertyName(HelperClass.TermJsonPropertyName)]
        public string Term { get; set; }
        [JsonPropertyName(HelperClass.DefinitionJsonPropertyName)]
        public string Definition { get; set; }
  


        public FlashCard(string term, string definition)
        {
           
            this.Term = term;
            this.Definition = definition;
            
        }

        public override string ToString()
        {
            return Term + " : " + Definition;
        }


    }
}

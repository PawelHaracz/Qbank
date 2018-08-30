using System;
using Qbank.Core.Command;

namespace Qbank.Questions.Commands
{
    public class CreateAnswer : ICommand
    { 
        public string Answer { get; set; }
        public bool IsCorrect { get; set; }
        public Guid QuestionId { get; set; }
        public string CreatedOn { get; set; }
    }
}
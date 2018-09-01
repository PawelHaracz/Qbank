using System;
using Qbank.Core.Command;

namespace Qbank.Questions.Commands
{
    public class CreateTag :  ICommand
    {
        public string TagName { get; set; }
        public string User{ get; set; }
        public Guid QuestionId { get; set; }
    }
}
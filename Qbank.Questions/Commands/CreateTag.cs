using Qbank.Core.Command;

namespace Qbank.Questions.Commands
{
    public class CreateTag :  ICommand
    {
        public string TagName { get; set; }
    }
}
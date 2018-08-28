using System;
using System.Collections.Generic;
using System.Text;
using Qbank.Core.Command;

namespace Qbank.Questions.Commands
{
    public class CreateQuestion : ICommand
    {
        public string Tag { get; set; }
        public string CreatedOn { get; set; }
        public string Question { get; set; }
    }
}

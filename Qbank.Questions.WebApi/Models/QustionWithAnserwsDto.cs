using System.Collections.Generic;

namespace Qbank.Questions.WebApi.Models
{
    public class QustionWithAnserwsDto
    {
        public string Question { get; set; }
        public IEnumerable<AnswerDto> Answers { get; set; }
    }
}
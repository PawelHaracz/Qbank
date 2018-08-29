using System;
using NUnit.Framework;
using Qbank.Questions;
using Qbank.Questions.Events;
using Qbank.Questions.Events.Questions;

namespace Qbank.Test.Questions
{
    [TestFixture]
    public class QuestionTest : BaseAggregateTest<QuestionState>
    {
        [Test]
        public void when_question_created_should_properly_create()
        {
            var qustionId = Guid.NewGuid();
            var question = "this is the simple question";

            Given();
            When(s => QuestionActions.Create(s, qustionId, question));
            Then(new QuestionCreated(qustionId, question));
        }

        [Test]
        public void when_question_already_exist_should_do_nothing()
        {
            var qustionId = Guid.NewGuid();
            var question = "this is the simple question";

            Given(new QuestionCreated(qustionId, question));
            When(s => QuestionActions.Create(s, qustionId, question));
            Then();
        }

        [Test]
        public void one_question_exist_add_another_should_properly_emmited_event()
        {
            var qustionId1 = Guid.NewGuid();
            var question1 = "this is the simple question 1";

            var qustionId2 = Guid.NewGuid();
            var question2 = "this is the simple question 2";

            Given(new QuestionCreated(qustionId1, question1));
            When(s => QuestionActions.Create(s, qustionId2, question2));
            Then(new QuestionCreated(qustionId2, question2));
        }

        [Test]
        public void Created_question_create_answer_should_emmited_event()
        {
            var qustionId = Guid.NewGuid();
            var question = "this is the simple question";
            var answerId = Guid.NewGuid();
            var answer = "this is the test answer";

            Given(new QuestionCreated(qustionId, question));
            When(s => QuestionActions.Create(s, qustionId,answerId,answer,false));
            Then(new AnswerCreated(answerId,qustionId,answer,false));
        }

        [Test]
        public void Not_created_question_want_to_add_answer_should_do_nothing()
        {
            var qustionId = Guid.NewGuid();          
            var answerId = Guid.NewGuid();
            var answer = "this is the test answer";

            Given();
            When(s => QuestionActions.Create(s, qustionId, answerId, answer, false));
            Then();
        }

        [Test]
        public void Given_Two_Question_connect_one_answer_to_both_should_emmited_events()
        {
            var qustionId1 = Guid.NewGuid();
            var question1 = "this is the simple question 1";
            var qustionId2 = Guid.NewGuid();
            var question2 = "this is the simple question 2";
            var answerId1 = Guid.NewGuid();
            var answer1 = "this is the test answer";


            Given(new QuestionCreated(qustionId1, question1));
            Given(new QuestionCreated(qustionId2, question2));
            When(s => QuestionActions.Create(s, qustionId1, answerId1, answer1, true));
            When(s => QuestionActions.Create(s, qustionId2, answerId1, answer1, false));
            Then(new AnswerCreated(answerId1, qustionId1, answer1, true), new AnswerCreated(answerId1, qustionId2, answer1, false));
        }
    }
}
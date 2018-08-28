using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using NUnit.Framework;
using Qbank.Core;
using Qbank.Core.Event;
using Qbank.Core.Projections;

namespace Qbank.Test.Core.Projections
{
    public class AverageBookRatingTests : ProjectionBaseTest<AverageBookRatingTests.AverageBookRating, AverageBookRatingTests.State>
    {
        [Test]
        public void When_many_users_rate_same_book_Should_calculate_statistics_properly()
        {
            Given("user-A", new BookRated(0, "The Goal"));
            Given("user-B", new BookRated(1, "The Goal"));
            Given("user-C", new BookRated(2, "The Goal"));

            Then("The Goal", new State { RatingCount = 3, RatingSum = 3 });
        }

        public class AverageBookRating : ProjectionBase<AverageBookRating, State>
        {
            public override Guid Id { get; } = new Guid("74E9842A-48B0-4ABF-AF29-8FE0970BC75E");

            protected override void Map(IEventMapping mapping)
            {
                mapping.For<BookRated>((meta, e) => e.Title, Apply);
            }

            static Task Apply(Metadata meta, BookRated bookRated, State s)
            {
                s.Apply(bookRated);
                return Task.CompletedTask;
            }
        }

        [DataContract]
        public class State
        {
            [DataMember(Order = 1)]
            public long RatingSum;
            [DataMember(Order = 2)]
            public long RatingCount;

            public double AverageRating => (double)RatingSum / RatingCount;

            public void Apply(BookRated bookRated)
            {
                RatingSum += bookRated.Rating;
                RatingCount += 1;
            }

            public override string ToString()
            {
                return $"{nameof(RatingSum)}: '{RatingSum}', {nameof(RatingCount)}: '{RatingCount}'";
            }
        }

        [EventTypeId("A5A3502D-2DC2-4E73-8D12-DF08F8F1A0E1")]
        public class BookRated : IEvent
        {
            public readonly byte Rating;
            public readonly string Title;

            public BookRated(byte rating, string title)
            {
                Rating = rating;
                Title = title;
            }

            public override string ToString()
            {
                return $"Book '{Title}' rated with '{Rating}'";
            }
        }
    }
}
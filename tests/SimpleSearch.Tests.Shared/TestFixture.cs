using System;
using AutoFixture;
using AutoFixture.AutoNSubstitute;

namespace SimpleSearch.Tests.Shared
{
    public class TestFixture<T> : Fixture
    {
        public TestFixture() => Customize(new AutoNSubstituteCustomization());

        public T Sut => this.Create<T>();
    }
}

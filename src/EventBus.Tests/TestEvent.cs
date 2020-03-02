namespace EventBus.Tests
{
    public class TestEvent : Event
    {
        public string Name { get; set; }

        public TestEvent(string name)
        {
            Name = name;
        }
    }
}
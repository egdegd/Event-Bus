namespace EventBus.src.UI
{
    class Message
    {
        public Service From { get; set; }
        public Service To { get; set; }
        public string Text { get; set; }
    }
}

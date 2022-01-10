namespace McatBot.Entities
{
    using System;

    public class McatPost
    {
        public long Id { get; set; }

        public string Text { get; set; } = null!;

        public DateTime PostDateTime { get; set; }

        // status, attachments
    }
}
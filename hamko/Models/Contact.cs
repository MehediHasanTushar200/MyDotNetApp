﻿namespace hamko.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Project { get; set; } = "";
        public string Subject { get; set; } = "";
        public string Message { get; set; } = "";
    }
}

﻿namespace Users.Domain
{
    public class SimpleUser
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int DepartmentId { get; set; }
        public bool IsActive { get; set; }
    }
}

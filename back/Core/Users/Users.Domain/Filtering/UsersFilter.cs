﻿using Tools;

namespace Users.Domain.Filtering
{
    public class UsersFilter
    {
        public BoolCombination IsActive { get; set; } = BoolCombination.Both;
        public string Search { get; set; } = null;
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWL.Core.Models
{
    public class News
    {

        public int Id { get; private set; }

        public string Title { get; private set; }

        public string Description { get; private set; }

        public string Image { get; private set; }

        public enum Category 
        { 
        Updates,
        Characters,
        Merchandise,
        Events
        }

        public Category category { get; private set; }
    }
}
using System;
using System.Collections.Generic;


namespace EgoraMap.Models
{
    public class ViewRoute
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageMap { get; set; }
        public IEnumerable<string> PhotoPath { get; set; }
    }
}
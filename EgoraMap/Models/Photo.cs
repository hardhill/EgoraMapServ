using System;
using System.ComponentModel.DataAnnotations;

namespace EgoraMap.Models
{
    public class Photo
    {
        public int Id { get; set; }
        [Required()]
        public DateTime Photocreated { get; set; }
        public string Description { get; set; }
        [Required()]
        public string PhotoName { get; set; }

        public int RouteId { get; set; }
        public Route Route { get; set; }
    }
}
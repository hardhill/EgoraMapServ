using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;


namespace EgoraMap.Models
{
    public class Route
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Название маршрута является обязательным значением!")]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "Необходимо указать файл карты маршрута")]
        public string RouteImage { get; set; }
        [Required(ErrorMessage = "Необходимо указать файл KML маршрута")]
        public string RouteKML { get; set; }
        public List<Photo> Photos { get; set; }

    }
}
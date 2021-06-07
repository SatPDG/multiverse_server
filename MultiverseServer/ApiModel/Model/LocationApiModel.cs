using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiverseServer.ApiModel.Model
{
    public class LocationApiModel
    {
        public double longitude { get; set; }
        public double latitude { get; set; }

        public LocationApiModel(double longitude, double latitude)
        {
            this.longitude = longitude;
            this.latitude = latitude;
        }
    }
}

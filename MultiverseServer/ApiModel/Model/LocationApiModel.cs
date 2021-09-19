using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;

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

        public Point ToDbModel()
        {
            return new Point(longitude, latitude);
        }

        public static LocationApiModel ToApiModel(Point dbModel)
        {
            return new LocationApiModel(dbModel.X, dbModel.Y); 
        }
    }
}

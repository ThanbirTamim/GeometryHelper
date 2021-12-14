// ***********************************************************************
// Assembly         : GeometryHelpherProject
// Author           : AWGC (Prog. Thanbir)
// Created          : 12-05-2021
//
// Last Modified By : User
// Last Modified On : 12-08-2021
// ***********************************************************************
// <copyright file="GeometryHelper.cs" company="">
//     Copyright ©  2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using DotSpatial.Data;
using DotSpatial.Projections; 
using DotSpatial.Topology;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryHelperProject
{
    /// <summary>
    /// Class GeometryHelper.
    /// </summary>
    public static class GeometryHelper
    {
        /// <summary>
        /// Gets the extent from center coordinate.
        /// </summary>
        /// <param name="center">The center position of required Extent</param>
        /// <param name="distance_km">The square root value of  requied area square</param>
        /// <returns>The method will return the 4 coordinate point which represent a extent area</returns>
        public static List<Coordinate> GetExtentFromCenterCoordinate(Coordinate center, double distance_km)
        {
            double hypotenuse_each_quardilator = Math.Sqrt(Math.Pow((distance_km / 2), 2) + Math.Pow((distance_km / 2), 2)); //Hy = Sqrt(L2 + h2)
            double rad_angle = (Math.PI / 180) * 45;
            double distance = hypotenuse_each_quardilator / 100;

            //left-top
            double x1 = center.X - distance * Math.Cos(rad_angle);
            double y1 = center.Y + distance * Math.Sin(rad_angle);
            //right-top
            double x2 = center.X + distance * Math.Cos(rad_angle);
            double y2 = center.Y + distance * Math.Sin(rad_angle);
            //right-bottom
            double x3 = center.X + distance * Math.Cos(rad_angle);
            double y3 = center.Y - distance * Math.Sin(rad_angle);
            //left-bottom
            double x4 = center.X - distance * Math.Cos(rad_angle);
            double y4 = center.Y - distance * Math.Sin(rad_angle);

            List<Coordinate> extent_area = new List<Coordinate>();
            extent_area.Add(new Coordinate(x1, y1));//left-top
            extent_area.Add(new Coordinate(x2, y2));//right-top
            extent_area.Add(new Coordinate(x3, y3));//right-bottom
            extent_area.Add(new Coordinate(x4, y4));//left-bottom

            return extent_area;
        }

        /// <summary>
        /// Creates the extent shape file.
        /// </summary>
        /// <param name="coordinates">The coordinates or extent point</param>
        /// <param name="shape_file_name">Name of the shape file which will be saved as your desire vector shape file</param>
        /// <param name="text_file_name">Name of the text file which will contain extents points info into text file</param>
        /// <returns>Return only boolean status</returns>
        public static bool CreateExtentShapeFile(List<Coordinate> coordinates, string shape_file_name, string text_file_name = null)
        {
            try
            {
                FeatureSet fs = new FeatureSet(FeatureType.Polygon);
                fs.Projection = ProjectionInfo.FromEpsgCode(4326);

                Coordinate[] coord = new Coordinate[]
                {
                coordinates[0],
                coordinates[1],
                coordinates[2],
                coordinates[3],
                coordinates[0]
                };

                fs.Features.Add(new Polygon(new LinearRing(coord)));
                fs.SaveAs(shape_file_name, true);

                if (text_file_name == null)
                    File.WriteAllText(Path.GetDirectoryName(shape_file_name) + @"/Extent.txt", $"{coordinates[3].X},{coordinates[3].Y},{coordinates[1].X},{coordinates[1].Y}");
                else
                    File.WriteAllText(text_file_name, $"{coordinates[3].X},{coordinates[3].Y},{coordinates[1].X},{coordinates[1].Y}");
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Creates the extent shape file.
        /// </summary>
        /// <param name="minx">The minx of extent</param>
        /// <param name="miny">The miny  of extent</param>
        /// <param name="maxx">The maxx  of extent</param>
        /// <param name="maxy">The maxy  of extent</param>
        /// <param name="shape_file_name">Name of your shape file name which will be saved as your desire location</param>
        /// <returns>Boolean</returns>
        public static bool CreateExtentShapeFile(double minx, double miny, double maxx, double maxy, string shape_file_name)
        {
            List<Coordinate> extent_area = new List<Coordinate>();
            extent_area.Add(new Coordinate(minx, maxy));//left-top
            extent_area.Add(new Coordinate(maxx, maxy));//right-top
            extent_area.Add(new Coordinate(maxx, miny));//right-bottom
            extent_area.Add(new Coordinate(minx, miny));//left-bottom
            CreateExtentShapeFile(extent_area, shape_file_name);
            return true;
        }

        /// <summary>
        /// Creates the polygon from extent.
        /// </summary>
        /// <param name="minx">The minx.</param>
        /// <param name="miny">The miny.</param>
        /// <param name="maxx">The maxx.</param>
        /// <param name="maxy">The maxy.</param>
        /// <returns>List of Coordinate</returns>
        public static List<Coordinate> CreatePolygonFromExtent(double minx, double miny, double maxx, double maxy)
        {
            List<Coordinate> extent_polygon = new List<Coordinate>();

            extent_polygon.Add(new Coordinate(minx, maxy)); //left-top
            extent_polygon.Add(new Coordinate(maxx, maxy)); //right-top
            extent_polygon.Add(new Coordinate(maxx, miny)); //right-bottom
            extent_polygon.Add(new Coordinate(minx, miny)); //left-bottom
            extent_polygon.Add(new Coordinate(minx, maxy)); //left-top

            return extent_polygon;
        }


        /// <summary>
        /// Creates the grid.
        /// </summary>
        /// <param name="extent">The extent of area</param>
        /// <param name="area_square_in_km">The root of area square in km for each grid cell</param>
        /// <param name="shape_file_name">Name of the grid shape file which will saved into your desire location</param>
        /// <returns>Boolean</returns>
        public static bool CreateGrid(Extent extent, double area_square_in_km, string shape_file_name)
        {
            double xMax = extent.MaxX;
            double xMin = extent.MinX;
            double yMax = extent.MaxY;
            double yMin = extent.MinY;

            int distance_width_extent = (int)Math.Round(new Coordinate(xMin, yMin).Distance(new Coordinate(xMax, yMin)) * 100);//km
            int loop_iteration_width = (int)(distance_width_extent / area_square_in_km);

            int distance_height_extent = (int)Math.Round(new Coordinate(xMin, yMin).Distance(new Coordinate(xMin, yMax)) * 100);//km
            int loop_iteration_height = (int)(distance_height_extent / area_square_in_km);

            double hypotenuse_each_quardilator = Math.Sqrt(Math.Pow(area_square_in_km, 2) + Math.Pow(area_square_in_km, 2)); //Hy = Sqrt(L2 + h2)
            double rad_angle = (Math.PI / 180) * 45;
            double distance = hypotenuse_each_quardilator / 100;

            double bxMin = xMin;
            double byMin = yMin;

            double txMin = xMin;
            double tyMin = yMin;

            FeatureSet grid = new FeatureSet(FeatureType.Polygon);
            grid.Projection = ProjectionInfo.FromEpsgCode(4326);
            for (int i = 0; i < loop_iteration_height; i++)
            {
                for (int j = 0; j < loop_iteration_width; j++)
                {
                    var fs = CreatePolygonFromExtent(txMin, tyMin, txMin + distance * Math.Cos(rad_angle), tyMin + distance * Math.Sin(rad_angle));
                    if (j == 0)
                        bxMin = fs[0].X; byMin = fs[0].Y;
                    grid.Features.Add(new Polygon(new LinearRing(fs.ToArray())));
                    txMin = fs[2].X; tyMin = fs[2].Y;
                }
                txMin = bxMin; tyMin = byMin;
            }
            grid.SaveAs(shape_file_name, true);
            return true;
        }
    }
}

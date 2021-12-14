// ***********************************************************************
// Assembly         : GeometryHelpherProject
// Author           : AWGC (Prog. Thanbir)
// Created          : 12-05-2021
//
// Last Modified By : AWGC (Prog. Thanbir)
// Last Modified On : 12-08-2021
// ***********************************************************************
// <copyright file="Program.cs" company="Army War Game Centre">
//     Copyright ©  2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using DotSpatial.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryHelperProject
{
    /// <summary>
    /// Class Program.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            //////provide center coord and get the 4 points sequentially of extent layer
            //var extent = GeometryHelper.GetExtentFromCenterCoordinate(new DotSpatial.Topology.Coordinate(90.399555, 23.811016), 2.0);
            ////provide 4 points sequentially of extent layer & shape file & text file name and get the output shape file
            //var status = GeometryHelper.CreateExtentShapeFile(
            //                extent,
            //                @"E:\Terrain Generation\IWGSS_AREA\Extent.shp",
            //                @"E:\Terrain Generation\IWGSS_AREA\Extent.txt"
            //            );

            var extent_feature = FeatureSet.Open(@"D:\3DTerrainInfoGenerationProject\GridExtentCreate\Resource\Extent.shp");
            GeometryHelper.CreateGrid(extent_feature.Extent, .5, @"D:\3DTerrainInfoGenerationProject\GridExtentCreate\Resource\Grid.shp");
        }
    }
}

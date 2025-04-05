using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using BikeRentalSystem.Models;
using Microsoft.Extensions.Configuration;
using Dapper;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BikeRentalSystem.DAL
{
    public class DatabaseHelperDapper
    {
        private readonly string? _connectionString;

        public DatabaseHelperDapper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public Bike? GetBikeByID(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                return conn.QuerySingleOrDefault<Bike>("GetBikeById", new { BikeID = id },
                                                            commandType: CommandType.StoredProcedure);
            }
        }

        public IList<Bike> GetAllBikes()
        {
            using var conn = new SqlConnection(_connectionString);
            return conn.Query<Bike>("GetAllBikes",
                                        commandType: CommandType.StoredProcedure
                ).ToList();
        }

        public void CreateBike(Bike bike)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                conn.Execute("CreateBike", new
                {
                    bike.Type,
                    bike.Status,
                    bike.RentalPricePerHour,
                    bike.Model,
                    bike.Image,
                    bike.HasInsurance
                },
                commandType: CommandType.StoredProcedure);
            }
        }

        public void UpdateBike(Bike bike)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                var existingBike = conn.QueryFirstOrDefault<Bike>(
                    "SELECT * FROM Bike WHERE BikeID = @BikeID",
                    new { bike.BikeID });

                if (existingBike == null)
                {
                    throw new KeyNotFoundException("Bike not found.");
                }

                conn.Execute("UpdateBike", new
                {
                    BikeID = bike.BikeID,
                    Type = bike.Type,
                    Status = bike.Status,
                    RentalPricePerHour = bike.RentalPricePerHour,
                    Model = bike.Model,
                    Image = bike.Image,
                    HasInsurance = bike.HasInsurance
                }, commandType: CommandType.StoredProcedure);
            }
        }

        public void DeleteBike(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                conn.Execute("DeleteBike", new { BikeID = id }, commandType: CommandType.StoredProcedure);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Http;
using ticket_vendor_demo.Models;

namespace ticket_vendor_demo.Controllers.Api
{
    [RoutePrefix("api/data")]
    public class DataController : ApiController
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        [HttpGet]
        [Route("passengers")]
        public IHttpActionResult GetPassengers(int page = 1, int pageSize = 5)
        {
            try
            {
                List<Passenger> passengers = new List<Passenger>();
                int totalRecords = 0;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Đếm tổng số bản ghi
                    string countQuery = "SELECT COUNT(*) FROM Passengers";
                    using (SqlCommand countCommand = new SqlCommand(countQuery, connection))
                    {
                        totalRecords = (int)countCommand.ExecuteScalar();
                    }

                    // Lấy dữ liệu phân trang
                    string query = @"
                        SELECT PassengerID, FullName, Email, PhoneNumber, IsGuest 
                        FROM Passengers 
                        ORDER BY PassengerID 
                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
                        command.Parameters.AddWithValue("@PageSize", pageSize);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                passengers.Add(new Passenger
                                {
                                    PassengerID = reader.GetInt32(0),
                                    FullName = reader.GetString(1),
                                    Email = reader.IsDBNull(2) ? null : reader.GetString(2),
                                    PhoneNumber = reader.IsDBNull(3) ? null : reader.GetString(3),
                                    IsGuest = reader.GetBoolean(4)
                                });
                            }
                        }
                    }
                }

                return Ok(new
                {
                    data = passengers,
                    totalRecords = totalRecords
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("trains")]
        public IHttpActionResult GetTrains(int page = 1, int pageSize = 5)
        {
            try
            {
                List<Train> trains = new List<Train>();
                int totalRecords = 0;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Đếm tổng số bản ghi
                    string countQuery = "SELECT COUNT(*) FROM Trains";
                    using (SqlCommand countCommand = new SqlCommand(countQuery, connection))
                    {
                        totalRecords = (int)countCommand.ExecuteScalar();
                    }

                    // Lấy dữ liệu phân trang
                    string query = @"
                        SELECT TrainID, TrainNumber, TrainName, DepartureStation, ArrivalStation, 
                               DepartureTime, ArrivalTime, TravelDate, TicketType, Price 
                        FROM Trains 
                        ORDER BY TrainID 
                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
                        command.Parameters.AddWithValue("@PageSize", pageSize);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                trains.Add(new Train
                                {
                                    TrainID = reader.GetInt32(0),
                                    TrainNumber = reader.GetString(1),
                                    TrainName = reader.GetString(2),
                                    DepartureStation = reader.GetString(3),
                                    ArrivalStation = reader.GetString(4),
                                    DepartureTime = reader.GetTimeSpan(5).ToString(@"hh\:mm"),
                                    ArrivalTime = reader.GetTimeSpan(6).ToString(@"hh\:mm"),
                                    TravelDate = reader.GetDateTime(7).ToString("yyyy-MM-dd"),
                                    TicketType = reader.GetString(8),
                                    Price = reader.GetDecimal(9)
                                });
                            }
                        }
                    }
                }

                return Ok(new
                {
                    data = trains,
                    totalRecords = totalRecords
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("tickets")]
        public IHttpActionResult GetTickets(int page = 1, int pageSize = 5)
        {
            try
            {
                List<Ticket> tickets = new List<Ticket>();
                int totalRecords = 0;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Đếm tổng số bản ghi
                    string countQuery = "SELECT COUNT(*) FROM Tickets";
                    using (SqlCommand countCommand = new SqlCommand(countQuery, connection))
                    {
                        totalRecords = (int)countCommand.ExecuteScalar();
                    }

                    // Lấy dữ liệu phân trang
                    string query = @"
                        SELECT TicketID, PassengerID, TrainID, SeatNumber 
                        FROM Tickets 
                        ORDER BY TicketID 
                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
                        command.Parameters.AddWithValue("@PageSize", pageSize);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tickets.Add(new Ticket
                                {
                                    TicketID = reader.GetInt32(0),
                                    PassengerID = reader.GetInt32(1),
                                    TrainID = reader.GetInt32(2),
                                    SeatNumber = reader.GetString(3)
                                });
                            }
                        }
                    }
                }

                return Ok(new
                {
                    data = tickets,
                    totalRecords = totalRecords
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("ticketpassengers")]
        public IHttpActionResult GetTicketPassengers(int page = 1, int pageSize = 5)
        {
            try
            {
                List<TicketPassenger> ticketPassengers = new List<TicketPassenger>();
                int totalRecords = 0;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Đếm tổng số bản ghi
                    string countQuery = "SELECT COUNT(*) FROM TicketPassengers";
                    using (SqlCommand countCommand = new SqlCommand(countQuery, connection))
                    {
                        totalRecords = (int)countCommand.ExecuteScalar();
                    }

                    // Lấy dữ liệu phân trang
                    string query = @"
                        SELECT TicketPassengerID, PassengerID, TicketID, QRCodeData 
                        FROM TicketPassengers 
                        ORDER BY TicketPassengerID 
                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
                        command.Parameters.AddWithValue("@PageSize", pageSize);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ticketPassengers.Add(new TicketPassenger
                                {
                                    TicketPassengerID = reader.GetInt32(0),
                                    PassengerID = reader.GetInt32(1),
                                    TicketID = reader.GetInt32(2),
                                    QRCodeData = reader.GetString(3)
                                });
                            }
                        }
                    }
                }

                return Ok(new
                {
                    data = ticketPassengers,
                    totalRecords = totalRecords
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
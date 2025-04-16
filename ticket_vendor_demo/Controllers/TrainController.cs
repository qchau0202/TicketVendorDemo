using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Mvc;
using ticket_vendor_demo.Models;
using System.Linq;

namespace ticket_vendor_demo.Controllers
{
    public class TrainController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // GET: Train/Index
        public ActionResult Index()
        {
            try
            {
                List<Train> trains = new List<Train>();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT TrainID, TrainNumber, TrainName, DepartureStation, ArrivalStation, DepartureTime, ArrivalTime, TravelDate, TicketType, Price FROM Trains";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
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
                return View(trains);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi khi tải danh sách chuyến tàu: " + ex.Message + " | StackTrace: " + ex.StackTrace;
                return View(new List<Train>());
            }
        }

        // GET: Train/Details/5
        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(400, "Bad Request: Missing train ID.");
            }

            try
            {
                Train train = null;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // Lấy thông tin chuyến tàu
                    string query = "SELECT TrainID, TrainNumber, TrainName, DepartureStation, ArrivalStation, DepartureTime, ArrivalTime, TravelDate, TicketType, Price FROM Trains WHERE TrainID = @TrainID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TrainID", id.Value);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                train = new Train
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
                                };
                            }
                        }
                    }

                    if (train == null)
                    {
                        return HttpNotFound("Không tìm thấy chuyến tàu.");
                    }

                    // Lấy danh sách ghế đã được đặt
                    List<string> bookedSeats = new List<string>();
                    string seatsQuery = "SELECT SeatNumber FROM Tickets WHERE TrainID = @TrainID";
                    using (SqlCommand seatsCommand = new SqlCommand(seatsQuery, connection))
                    {
                        seatsCommand.Parameters.AddWithValue("@TrainID", id.Value);
                        using (SqlDataReader seatsReader = seatsCommand.ExecuteReader())
                        {
                            while (seatsReader.Read())
                            {
                                bookedSeats.Add(seatsReader.GetString(0));
                            }
                        }
                    }

                    // Giả lập danh sách ghế
                    List<string> allSeats = new List<string> { "A1", "A2", "A3", "A4", "B1", "B2", "B3", "B4" };
                    List<string> availableSeats = allSeats.Except(bookedSeats).ToList();

                    // Truyền dữ liệu vào View
                    ViewBag.AvailableSeats = availableSeats;
                }

                return View(train);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi khi tải chi tiết chuyến tàu: " + ex.Message + " | StackTrace: " + ex.StackTrace;
                return View();
            }
        }


        // POST: Train/SelectSeat
        [HttpPost]
        public ActionResult SelectSeat(int trainId, string seatNumber)
        {
            TempData["TrainID"] = trainId;
            TempData["SeatNumber"] = seatNumber;
            return RedirectToAction("PassengerInfo", "Booking");
        }
    }
}
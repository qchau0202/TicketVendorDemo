using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Mvc;
using ticket_vendor_demo.Models;
using Newtonsoft.Json;

namespace ticket_vendor_demo.Controllers
{
    public class PaymentController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // GET: Payment/ProcessPayment
        public ActionResult ProcessPayment()
        {
            if (TempData["TrainID"] == null || TempData["SeatNumber"] == null || TempData["PassengerID"] == null)
            {
                return RedirectToAction("Index", "Train");
            }

            int trainId = Convert.ToInt32(TempData["TrainID"]);
            string seatNumber = TempData["SeatNumber"]?.ToString();
            int passengerId = Convert.ToInt32(TempData["PassengerID"]);

            Train train = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT TrainID, TrainNumber, TrainName, Price, DepartureStation, ArrivalStation, DepartureTime, ArrivalTime, TravelDate, TicketType " +
                              "FROM Trains WHERE TrainID = @TrainID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TrainID", trainId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            train = new Train
                            {
                                TrainID = reader.GetInt32(0),
                                TrainNumber = reader.GetString(1),
                                TrainName = reader.GetString(2),
                                Price = reader.GetDecimal(3),
                                DepartureStation = reader.GetString(4),
                                ArrivalStation = reader.GetString(5),
                                DepartureTime = reader.GetTimeSpan(6).ToString(@"hh\:mm"),
                                ArrivalTime = reader.GetTimeSpan(7).ToString(@"hh\:mm"),
                                TravelDate = reader.GetDateTime(8).ToString("yyyy-MM-dd"),
                                TicketType = reader.GetString(9)
                            };
                        }
                    }
                }
            }

            if (train == null)
            {
                return HttpNotFound("Không tìm thấy thông tin chuyến tàu.");
            }

            ViewBag.Train = train;
            ViewBag.SeatNumber = seatNumber;
            ViewBag.PassengerID = passengerId;

            TempData["TrainID"] = trainId;
            TempData["SeatNumber"] = seatNumber;
            TempData["PassengerID"] = passengerId;
            TempData.Keep("TrainID");
            TempData.Keep("SeatNumber");
            TempData.Keep("PassengerID");

            return View();
        }

        // POST: Payment/ProcessPayment
        [HttpPost]
        public ActionResult ProcessPayment(string paymentMethod)
        {
            if (string.IsNullOrEmpty(paymentMethod))
            {
                ViewBag.ErrorMessage = "Vui lòng chọn phương thức thanh toán.";
                int trainId = Convert.ToInt32(TempData["TrainID"]);
                string seatNumber = TempData["SeatNumber"]?.ToString();
                int passengerId = Convert.ToInt32(TempData["PassengerID"]);
                Train train = null;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT TrainID, TrainNumber, TrainName, Price, DepartureStation, ArrivalStation, DepartureTime, ArrivalTime, TravelDate, TicketType " +
                                  "FROM Trains WHERE TrainID = @TrainID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TrainID", trainId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                train = new Train
                                {
                                    TrainID = reader.GetInt32(0),
                                    TrainNumber = reader.GetString(1),
                                    TrainName = reader.GetString(2),
                                    Price = reader.GetDecimal(3),
                                    DepartureStation = reader.GetString(4),
                                    ArrivalStation = reader.GetString(5),
                                    DepartureTime = reader.GetTimeSpan(6).ToString(@"hh\:mm"),
                                    ArrivalTime = reader.GetTimeSpan(7).ToString(@"hh\:mm"),
                                    TravelDate = reader.GetDateTime(8).ToString("yyyy-MM-dd"),
                                    TicketType = reader.GetString(9)
                                };
                            }
                        }
                    }
                }
                ViewBag.Train = train;
                ViewBag.SeatNumber = seatNumber;
                ViewBag.PassengerID = passengerId;
                TempData.Keep("TrainID");
                TempData.Keep("SeatNumber");
                TempData.Keep("PassengerID");
                return View();
            }

            try
            {
                int trainId = Convert.ToInt32(TempData["TrainID"]);
                string seatNumber = TempData["SeatNumber"]?.ToString();
                int passengerId = Convert.ToInt32(TempData["PassengerID"]);

                bool paymentSuccess = true; // Giả lập thanh toán
                if (!paymentSuccess)
                {
                    ViewBag.ErrorMessage = "Thanh toán thất bại. Vui lòng thử lại.";
                    ViewBag.TrainID = trainId;
                    ViewBag.SeatNumber = seatNumber;
                    ViewBag.PassengerID = passengerId;
                    TempData.Keep("TrainID");
                    TempData.Keep("SeatNumber");
                    TempData.Keep("PassengerID");
                    return View();
                }

                int ticketId;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string ticketQuery = "INSERT INTO Tickets (PassengerID, TrainID, SeatNumber) OUTPUT INSERTED.TicketID " +
                                        "VALUES (@PassengerID, @TrainID, @SeatNumber)";
                    using (SqlCommand command = new SqlCommand(ticketQuery, connection))
                    {
                        command.Parameters.AddWithValue("@PassengerID", passengerId);
                        command.Parameters.AddWithValue("@TrainID", trainId);
                        command.Parameters.AddWithValue("@SeatNumber", seatNumber);
                        ticketId = (int)command.ExecuteScalar();
                    }

                    string qrCodeData = "";
                    string dataQuery = @"
                        SELECT t.TicketID, p.FullName, tr.TrainName, tr.DepartureStation, tr.ArrivalStation, 
                               tr.TravelDate, tr.DepartureTime, tr.ArrivalTime, t.SeatNumber
                        FROM Tickets t
                        JOIN Passengers p ON t.PassengerID = p.PassengerID
                        JOIN Trains tr ON t.TrainID = tr.TrainID
                        WHERE t.TicketID = @TicketID";
                    using (SqlCommand dataCommand = new SqlCommand(dataQuery, connection))
                    {
                        dataCommand.Parameters.AddWithValue("@TicketID", ticketId);
                        using (SqlDataReader reader = dataCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var ticketData = new
                                {
                                    TicketID = reader.GetInt32(0),
                                    Passenger = reader.GetString(1),
                                    Train = reader.GetString(2),
                                    Departure = reader.GetString(3),
                                    Arrival = reader.GetString(4),
                                    TravelDate = reader.GetDateTime(5).ToString("yyyy-MM-dd"),
                                    DepartureTime = reader.GetTimeSpan(6).ToString(@"hh\:mm"),
                                    ArrivalTime = reader.GetTimeSpan(7).ToString(@"hh\:mm"),
                                    SeatNumber = reader.GetString(8)
                                };
                                qrCodeData = JsonConvert.SerializeObject(ticketData);
                            }
                        }
                    }

                    string ticketPassengerQuery = "INSERT INTO TicketPassengers (PassengerID, TicketID, QRCodeData) " +
                                                "VALUES (@PassengerID, @TicketID, @QRCodeData)";
                    using (SqlCommand tpCommand = new SqlCommand(ticketPassengerQuery, connection))
                    {
                        tpCommand.Parameters.AddWithValue("@PassengerID", passengerId);
                        tpCommand.Parameters.AddWithValue("@TicketID", ticketId);
                        tpCommand.Parameters.AddWithValue("@QRCodeData", qrCodeData);
                        tpCommand.ExecuteNonQuery();
                    }
                }

                TempData["TicketID"] = ticketId;
                return RedirectToAction("BookingSuccess", "Booking");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi khi xử lý thanh toán: " + ex.Message;
                ViewBag.TrainID = TempData["TrainID"];
                ViewBag.SeatNumber = TempData["SeatNumber"];
                ViewBag.PassengerID = TempData["PassengerID"];
                TempData.Keep("TrainID");
                TempData.Keep("SeatNumber");
                TempData.Keep("PassengerID");
                return View();
            }
        }
    }
}
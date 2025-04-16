using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Mvc;
using ticket_vendor_demo.Models;

namespace ticket_vendor_demo.Controllers
{
    public class BookingController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // GET: Booking/PassengerInfo
        public ActionResult PassengerInfo()
        {
            if (TempData["TrainID"] == null || TempData["SeatNumber"] == null)
            {
                return RedirectToAction("Index", "Train");
            }

            ViewBag.TrainID = TempData["TrainID"];
            ViewBag.SeatNumber = TempData["SeatNumber"];
            TempData.Keep("TrainID");
            TempData.Keep("SeatNumber");

            return View();
        }

        // GET: Booking/Guest
        public ActionResult Guest()
        {
            if (TempData["TrainID"] == null || TempData["SeatNumber"] == null)
            {
                return RedirectToAction("Index", "Train");
            }
            TempData.Keep("TrainID");
            TempData.Keep("SeatNumber");
            return View();
        }

        // POST: Booking/Guest
        [HttpPost]
        public ActionResult Guest(string fullName, string phoneNumber, string email)
        {
            try
            {
                if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(phoneNumber))
                {
                    ViewBag.ErrorMessage = "Vui lòng nhập đầy đủ họ tên và số điện thoại.";
                    return View();
                }

                // Lưu thông tin khách vào bảng Passengers
                int passengerId;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Passengers (FullName, Email, PhoneNumber, IsGuest) OUTPUT INSERTED.PassengerID " +
                                   "VALUES (@FullName, @Email, @PhoneNumber, @IsGuest)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FullName", fullName);
                        command.Parameters.AddWithValue("@Email", (object)email ?? DBNull.Value);
                        command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        command.Parameters.AddWithValue("@IsGuest", true); // Guest mode

                        passengerId = (int)command.ExecuteScalar();
                    }
                }

                TempData["PassengerID"] = passengerId;
                TempData.Keep("TrainID");
                TempData.Keep("SeatNumber");

                return RedirectToAction("Confirm");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi: " + ex.Message;
                return View();
            }
        }

        // GET: Booking/Confirm
        public ActionResult Confirm()
        {
            if (TempData["TrainID"] == null || TempData["SeatNumber"] == null || TempData["PassengerID"] == null)
            {
                return RedirectToAction("Index", "Train");
            }

            int trainId = Convert.ToInt32(TempData["TrainID"]);
            string seatNumber = TempData["SeatNumber"]?.ToString();
            int passengerId = Convert.ToInt32(TempData["PassengerID"]);

            try
            {
                Train train = null;
                Passenger passenger = null;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string trainQuery = "SELECT TrainID, TrainNumber, TrainName, DepartureStation, ArrivalStation, DepartureTime, ArrivalTime, TravelDate, TicketType, Price " +
                                        "FROM Trains WHERE TrainID = @TrainID";
                    using (SqlCommand command = new SqlCommand(trainQuery, connection))
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

                    string passengerQuery = "SELECT PassengerID, FullName, Email, PhoneNumber, IsGuest FROM Passengers WHERE PassengerID = @PassengerID";
                    using (SqlCommand command = new SqlCommand(passengerQuery, connection))
                    {
                        command.Parameters.AddWithValue("@PassengerID", passengerId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                passenger = new Passenger
                                {
                                    PassengerID = reader.GetInt32(0),
                                    FullName = reader.GetString(1),
                                    Email = reader.IsDBNull(2) ? null : reader.GetString(2),
                                    PhoneNumber = reader.GetString(3),
                                    IsGuest = reader.GetBoolean(4)
                                };
                            }
                        }
                    }

                    if (train == null || passenger == null)
                    {
                        return HttpNotFound("Không tìm thấy thông tin chuyến tàu hoặc khách.");
                    }

                    ViewBag.Train = train;
                    ViewBag.Passenger = passenger;
                    ViewBag.SeatNumber = seatNumber;

                    TempData["PassengerID"] = passengerId;
                    TempData.Keep("TrainID");
                    TempData.Keep("SeatNumber");
                }

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi khi tải thông tin xác nhận: " + ex.Message;
                return View();
            }
        }

        // POST: Booking/Confirm
        [HttpPost]
        public ActionResult Confirm(string confirm)
        {
            if (TempData["TrainID"] == null || TempData["SeatNumber"] == null || TempData["PassengerID"] == null)
            {
                return RedirectToAction("Index", "Train");
            }

            // Logic lưu vé đã được chuyển sang PaymentController, chỉ cần chuyển hướng
            TempData.Keep("TrainID");
            TempData.Keep("SeatNumber");
            TempData.Keep("PassengerID");

            return RedirectToAction("ProcessPayment", "Payment");
        }

        // GET: Booking/BookingSuccess
        public ActionResult BookingSuccess()
        {
            if (TempData["TicketID"] == null)
            {
                return RedirectToAction("Index", "Train");
            }

            int ticketId = Convert.ToInt32(TempData["TicketID"]);

            try
            {
                Train train = null;
                Passenger passenger = null;
                string seatNumber = null;
                string qrCodeData = null;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT t.TicketID, t.SeatNumber, tr.TrainID, tr.TrainNumber, tr.TrainName, 
                               tr.DepartureStation, tr.ArrivalStation, tr.DepartureTime, tr.ArrivalTime, 
                               tr.TravelDate, tr.TicketType, tr.Price, p.PassengerID, p.FullName, 
                               p.Email, p.PhoneNumber, p.IsGuest, tp.QRCodeData
                        FROM Tickets t
                        JOIN Trains tr ON t.TrainID = tr.TrainID
                        JOIN Passengers p ON t.PassengerID = p.PassengerID
                        LEFT JOIN TicketPassengers tp ON t.TicketID = tp.TicketID
                        WHERE t.TicketID = @TicketID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TicketID", ticketId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                train = new Train
                                {
                                    TrainID = reader.GetInt32(2),
                                    TrainNumber = reader.GetString(3),
                                    TrainName = reader.GetString(4),
                                    DepartureStation = reader.GetString(5),
                                    ArrivalStation = reader.GetString(6),
                                    DepartureTime = reader.GetTimeSpan(7).ToString(@"hh\:mm"),
                                    ArrivalTime = reader.GetTimeSpan(8).ToString(@"hh\:mm"),
                                    TravelDate = reader.GetDateTime(9).ToString("yyyy-MM-dd"),
                                    TicketType = reader.GetString(10),
                                    Price = reader.GetDecimal(11)
                                };

                                passenger = new Passenger
                                {
                                    PassengerID = reader.GetInt32(12),
                                    FullName = reader.GetString(13),
                                    Email = reader.IsDBNull(14) ? null : reader.GetString(14),
                                    PhoneNumber = reader.GetString(15),
                                    IsGuest = reader.GetBoolean(16)
                                };

                                seatNumber = reader.GetString(1);
                                qrCodeData = reader.IsDBNull(17) ? null : reader.GetString(17);
                            }
                        }
                    }
                }

                if (train == null || passenger == null)
                {
                    return HttpNotFound("Không tìm thấy thông tin vé.");
                }

                ViewBag.TicketID = ticketId;
                ViewBag.Train = train;
                ViewBag.Passenger = passenger;
                ViewBag.SeatNumber = seatNumber;
                ViewBag.QRCodeData = qrCodeData;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Lỗi khi tải thông tin vé: " + ex.Message;
                return View();
            }
        }
    }
}